using Abstraction;
using BTAPlayer;
using MultiplayerService;
using ParrelSync;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Prefs;
using System.Collections.Generic;
using UI;
using UnityEditor.PackageManager;
using UnityEngine;

public class InGameMain : MonoBehaviourPun
{
    [SerializeField]
    private int _randomSeed = 1111;
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private PlayerConfig _playerConfig;
    [SerializeField]
    private Transform[] _spawnPoints;
    [SerializeField]
    private GameSceneUI _gameSceneUI;
    [SerializeField]
    private GameNetManager _netManager;

    private Camera _mainCamera;
    private List<IPlayerController> _playerControllers = new List<IPlayerController>();

    private DataServerService _dataServerService;

    private IGamePrefs _gamePrefs;

    private void Start()
    {
        Random.InitState(_randomSeed);

        _mainCamera = Camera.main;

        _dataServerService = new DataServerService();

        _netManager.OnPlayerLeftFromRoom += PlayerLeftedGame;

        if (_netManager.IsConnected)
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                _gamePrefs = new ClonedGamePrefs();
            }
            else
            {
                _gamePrefs = new GamePrefs();
            }
#else
            _gamePrefs = new GamePrefs();
#endif
            _gamePrefs.Load();

            Subscribe();

            SpawnPlayer();

            return;
        }
        else
        {
            PhotonNetwork.LoadLevel(0);
        }
    }

    private void Subscribe()
    {
        _netManager.OnPlayerLeftFromRoom += PlayerLeftedGame;
        _dataServerService.OnGetUserData += UserDataLoaded;
    }


    private void Unsubscribe()
    {
        _netManager.OnPlayerLeftFromRoom -= PlayerLeftedGame;
        _dataServerService.OnGetUserData -= UserDataLoaded;
    }

    private void PlayerLeftedGame(Player player)
    {
        var playerController = _playerControllers.Find(p => p.PlayerId == player.UserId);
        if (playerController != null)
        {
            playerController?.Dispose();
            _playerControllers.Remove(playerController);
        }
    }

    private void UserDataLoaded(PlayfabPlayerData userData)
    {
        Debug.Log($"Getted User : {userData.Nickname} Lvl[{userData.CurrentLevel}] with progress {userData.CurrentLevelProgress}");

        _gameSceneUI.ChangePlayerLevel(userData.CurrentLevel);

        _playerControllers[0].PlayerLevel = userData.CurrentLevel;
    }


    private void SpawnPlayer()
    {
        if (_netManager.IsConnected == false)
            return;
        if (_playerPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'InGameMain'", this);
        }
        else
        {
            var playerNum = _netManager.CurrentPlayer.ActorNumber - 1;

            Vector3 spawnPosition = _spawnPoints[0].position;

            if (playerNum > 0)
            {
                spawnPosition = _spawnPoints[playerNum].position;
            }

            var playerObject = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);

            var playerPhoton = playerObject.GetComponent<PhotonView>();

            PhotonNetwork.AllocateViewID(playerPhoton);
            PhotonNetwork.RegisterPhotonView(playerPhoton);

            var playerView = playerObject.GetComponent<PlayerView>();

            _playerControllers.Add(
                new PlayerMasterController(
                    _netManager.CurrentPlayer.UserId,
                    _playerConfig,
                    playerView,
                    _gameSceneUI,
                    _mainCamera));


            _dataServerService.GetUserData(_gamePrefs.PlayFabId);

            photonView.RPC(
                nameof(InstantiatePlayer),
                RpcTarget.Others,
                playerPhoton.ViewID,
                _netManager.CurrentPlayer,
                spawnPosition,
                Quaternion.identity);
        }
    }

    [PunRPC]
    private void InstantiatePlayer(int viewId, Player player, Vector3 position, Quaternion rotation)
    {
        var playerObject = Instantiate(_playerPrefab, position, rotation);

        var playerPhoton = playerObject.GetComponent<PhotonView>();

        playerPhoton.ViewID = viewId;

        playerPhoton.TransferOwnership(player);

        var playerView = playerObject.GetComponent<PlayerView>();

        if (_playerControllers != null)
        {
            _playerControllers.Add(new PlayerClientController(player.UserId, _playerConfig, playerView, _mainCamera));
        }
    }

    private void Update()
    {
        if (_playerControllers == null && _playerControllers.Count == 0)
            return;

        for (int i = 0; i < _playerControllers.Count; i++)
        {
            _playerControllers[i].ExecuteUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (_playerControllers == null && _playerControllers.Count == 0)
            return;

        for (int i = 0; i < _playerControllers.Count; i++)
        {
            _playerControllers[i].ExecuteFixedUpdate(Time.fixedDeltaTime);
        }
    }


    private void OnDestroy()
    {
        Unsubscribe();
    }
}
