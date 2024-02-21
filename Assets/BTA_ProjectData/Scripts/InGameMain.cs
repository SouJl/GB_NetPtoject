using BTAPlayer;
using MultiplayerService;
using Photon.Pun;
using Photon.Realtime;
using Prefs;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Configs;
using Enemy;
using System.Collections;
using Abstraction;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class InGameMain : MonoBehaviourPun, IDisposable
{
    [SerializeField]
    private bool _isTesting = false;

    [SerializeField]
    private int _randomSeed = 1111;
    [SerializeField]
    private GameObject _playerMasterPrefab;
    [SerializeField]
    private GameObject _playerClientPrefab;
    [SerializeField]
    private PlayerConfig _playerConfig;
    [SerializeField]
    private Transform[] _spawnPoints;
    [SerializeField]
    private GameUIController _gameUI;
    [SerializeField]
    private GameNetManager _netManager;
    [SerializeField]
    private EnemySpawnController _enemySpawner;

    private Camera _mainCamera;
    private List<IPlayerController> _playerControllers = new List<IPlayerController>();

    private DataServerService _dataServerService;

    private IGamePrefs _gamePrefs;


    private void Start()
    {
        Random.InitState(_randomSeed);

        _mainCamera = Camera.main;

        _dataServerService = new DataServerService();

        if (_isTesting)
        {
            TestStart();
        }
        else
        {
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

                Subscribe();

                _gamePrefs.LoadUser();

                _dataServerService.GetPlayerData();
            }
            else
            {
                PhotonNetwork.LoadLevel(0);
            }
        }
    }

    #region TestMode

    private void TestStart()
    {
        _gamePrefs = new GamePrefs();

        _gamePrefs.LoadUser();
        _netManager.OnConnectedToServer += ConnectedToServer;
        _netManager.OnJoinInLobby += JoindedInLobby;
        _netManager.OnJoinInRoom += JoinedInRoom;
        _netManager.Connect(_gamePrefs.GetUser().Name);
    }

    private void ConnectedToServer()
    {
        _netManager.JoinLobby();
    }

    private void JoindedInLobby()
    {
        _netManager.CreateRoom(new CreationRoomData
        {
            RoomName = "TestRoom",
            MaxPlayers = 5
        });
    }

    private void JoinedInRoom(Room room)
    {
        SpawnPlayerNew("test", 0);

        SpawnEmemy();
    }

    #endregion

    private void Subscribe()
    { 
        _dataServerService.OnGetUserData += UserDataLoaded;
        _netManager.OnDisconnectedFromServer += Disconnected;

        _gameUI.OnReturnMainMenu += ReturnToMainMenu;
        _gameUI.OnExitGame += ExitFromGame;
    }

    private void Unsubscribe()
    {
        _dataServerService.OnGetUserData -= UserDataLoaded;
        _netManager.OnDisconnectedFromServer -= Disconnected;

        _gameUI.OnReturnMainMenu -= ReturnToMainMenu;
        _gameUI.OnExitGame -= ExitFromGame;
    }


    private void UserDataLoaded(PlayfabPlayerData userData)
    {
        Debug.Log($"Getted User : {userData.Nickname} Lvl[{userData.CurrentLevel}] with progress {userData.CurrentLevelProgress}");

        SpawnPlayerNew(userData.Nickname, userData.CurrentLevel);

        SpawnEmemy();
    }

    private void Disconnected()
    {
        Dispose();

        SceneManager.LoadScene(0);
    }

    private void ReturnToMainMenu()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(DisconnectFromGame), RpcTarget.AllViaServer);
        }
        else
        {
            DisconnectFromGame();
        }
    }

    private void ExitFromGame()
    {
        _dataServerService.LogOut();

        Dispose();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void SpawnPlayerNew(string name, int level)
    {
        var playerNum = _netManager.CurrentPlayer.ActorNumber - 1;

        Vector3 spawnPosition = _spawnPoints[0].position;

        if (playerNum > 0)
        {
            spawnPosition = _spawnPoints[playerNum].position;
        }

        var playerGo
            = PhotonNetwork.Instantiate(
                $"Player/{_playerMasterPrefab.name}",
                spawnPosition, Quaternion.identity,
                0, new object[] { name, level });

        var player = playerGo.GetComponent<PlayerController>();

        _enemySpawner.AddPlayer(player.SelfTransform);
    }

    private void SpawnPlayer()
    {
        if (_netManager.IsConnected == false)
            return;
        if (_playerMasterPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'InGameMain'", this);
        }
        else
        {
            var playerNum = _netManager.CurrentPlayer.ActorNumber - 1;

            Vector3 spawnPosition = _spawnPoints[0].position;
            var rotation = Quaternion.identity;

            if (playerNum > 0)
            {
                spawnPosition = _spawnPoints[playerNum].position;
            }

            var playerObject = Instantiate(_playerMasterPrefab, spawnPosition, rotation);

            var playerPhoton = playerObject.GetComponent<PhotonView>();

            PhotonNetwork.AllocateViewID(playerPhoton);
            PhotonNetwork.RegisterPhotonView(playerPhoton);

            var playerView = playerObject.GetComponent<PlayerView>();

            var playerController
                = new PlayerMasterController(
                    _netManager.CurrentPlayer.UserId, 
                    _playerConfig, 
                    playerView, 
                    _gameUI.PlayerViewUI, 
                    _mainCamera);

            _enemySpawner.AddPlayer(playerController);

            _playerControllers.Add(playerController);

            photonView.RPC(
               nameof(InstantiatePlayer),
               RpcTarget.Others,
               playerPhoton.ViewID,
               _netManager.CurrentPlayer,
               spawnPosition,
               rotation);

            _dataServerService.GetPlayerData();
        }
    }
    
    [PunRPC]
    private void DisconnectFromGame()
    {
        _netManager.Disconnect();
    }

    [PunRPC]
    private void InstantiatePlayer(int viewId, Player player, Vector3 position, Quaternion rotation)
    {
        /*var playerObject = Instantiate(_playerClientPrefab, position, rotation);

        var playerPhoton = playerObject.GetComponent<PhotonView>();

        playerPhoton.ViewID = viewId;

        playerPhoton.TransferOwnership(player);

        var playerView = playerObject.GetComponent<PlayerView>();

        if (_playerControllers != null)
        {
            var playerController
                = new PlayerClientController(player.UserId, _playerConfig, playerView, _mainCamera);

            _playerControllers.Add(playerController);

            _enemySpawner.AddPlayer(playerController);
        }*/
    }

    private void SpawnEmemy()
    {
        StartCoroutine(StartSpawnEnemies());
    }

    private IEnumerator StartSpawnEnemies()
    {
        yield return new WaitForSeconds(3f);

        _enemySpawner.StartEnemySpawn();
    }

    private void OnDestroy()
    {
        Dispose();
    }

    #region IDisposable

    private bool _isDisposed = false;

    public void Dispose()
    {
        if (_isDisposed)
            return;
        _isDisposed = true;

        Unsubscribe();
    }

    #endregion

}
