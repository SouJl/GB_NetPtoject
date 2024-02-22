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
using GameTask;
using Tools;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class InGameMain : MonoBehaviourPun, IPaused, IDisposable
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
    [SerializeField]
    private CollisionDetector _centralRoomCollision;
    [SerializeField]
    private CollisionDetector _storageRoomCollision;


    private Camera _mainCamera;
    private List<IPlayerController> _playerControllers = new List<IPlayerController>();

    private DataServerService _dataServerService;

    private IGamePrefs _gamePrefs;

    private int _mainTaskId;
    private int _killEnemyTaskId;

    private void Awake()
    {
        Random.InitState(_randomSeed);

        _mainCamera = Camera.main;

        _dataServerService = new DataServerService();

        Subscribe();
    }

    private void Start()
    {
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
                _gamePrefs.LoadUser();

                _dataServerService.GetPlayerData();

                return;
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
        _netManager.CreateRoom(new Abstraction.CreationRoomData
        {
            RoomName = "TestRoom",
            MaxPlayers = 5
        });
    }

    private void JoinedInRoom(Room room)
    {

        var playerNum = _netManager.CurrentPlayer.ActorNumber - 1;

        Vector3 spawnPosition = _spawnPoints[0].position;

        if (playerNum > 0)
        {
            spawnPosition = _spawnPoints[playerNum].position;
        }

        var playerObject = Instantiate(_playerMasterPrefab, spawnPosition, Quaternion.identity);

        var playerPhoton = playerObject.GetComponent<PhotonView>();

        PhotonNetwork.AllocateViewID(playerPhoton);
        PhotonNetwork.RegisterPhotonView(playerPhoton);

        var playerView = playerObject.GetComponent<PlayerView>();

        var playerController
                 = new PlayerMasterController(
                     _netManager.CurrentPlayer.UserId, 
                     _playerConfig, 
                     playerView, 
                     _gameUI, 
                     _mainCamera);

        _enemySpawner.AddPlayer(playerController);

        _playerControllers.Add(playerController);

        _mainTaskId = TaskManager.AddNewTask("REACH CENTRAL ROOM");
    }

    #endregion

    private void Subscribe()
    { 
        _netManager.OnPlayerLeftFromRoom += PlayerLeftedGame;
        _dataServerService.OnGetUserData += UserDataLoaded;
        _netManager.OnDisconnectedFromServer += Disconnected;

        _gameUI.OnReturnMainMenu += ReturnToMainMenu;
        _gameUI.OnExitGame += ExitFromGame;

        _centralRoomCollision.OnEnter += SomoneEnterCentralRoom;
        _storageRoomCollision.OnEnter += SomoneEnterStorageRoom;

        _enemySpawner.AllEnemiesDestored += AllEnemyDead;
    }



    private void Unsubscribe()
    {
        _netManager.OnPlayerLeftFromRoom -= PlayerLeftedGame;
        _dataServerService.OnGetUserData -= UserDataLoaded;
        _netManager.OnDisconnectedFromServer -= Disconnected;

        _gameUI.OnReturnMainMenu -= ReturnToMainMenu;
        _gameUI.OnExitGame -= ExitFromGame;

        _centralRoomCollision.OnEnter -= SomoneEnterCentralRoom;
        _storageRoomCollision.OnEnter -= SomoneEnterStorageRoom;

        _enemySpawner.AllEnemiesDestored -= AllEnemyDead;
    }

    private void SomoneEnterCentralRoom(Collider coollider)
    {
        if (coollider.gameObject.tag == "Player")
        {
            TaskManager.TaskCompeleted(_mainTaskId);
        }
    }

    private void SomoneEnterStorageRoom(Collider coollider)
    {
        if (coollider.gameObject.tag == "Player")
        {
            _killEnemyTaskId =  TaskManager.AddNewTask("KILL ALL ENEMIES!");
            SpawnEmemy();

            _storageRoomCollision.gameObject.SetActive(false);
        }
    }

    private void AllEnemyDead()
    {
        TaskManager.TaskCompeleted(_killEnemyTaskId);
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

        SpawnPlayer();

        _gameUI.PlayerViewUI.ChangePlayerLevel(userData.CurrentLevel);

        _playerControllers[0].PlayerLevel = userData.CurrentLevel;

        _mainTaskId = TaskManager.AddNewTask("REACH CENTRAL ROOM");
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


    private void SpawnPlayer()
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
                _gameUI,
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
    }
    
    [PunRPC]
    private void DisconnectFromGame()
    {
        _netManager.Disconnect();
    }

    [PunRPC]
    private void InstantiatePlayer(int viewId, Player player, Vector3 position, Quaternion rotation)
    {
        var playerObject = Instantiate(_playerClientPrefab, position, rotation);

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
        }
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

    private void Update()
    {
        if (_isPaused)
            return;

        if (_playerControllers == null && _playerControllers.Count == 0)
            return;

        for (int i = 0; i < _playerControllers.Count; i++)
        {
            _playerControllers[i].ExecuteUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (_isPaused)
            return;

        if (_playerControllers == null && _playerControllers.Count == 0)
            return;

        for (int i = 0; i < _playerControllers.Count; i++)
        {
            _playerControllers[i].ExecuteFixedUpdate(Time.fixedDeltaTime);
        }
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

    #region IPaused

    private bool _isPaused;

    public void OnPause(bool state)
    {
        _isPaused = state;

        if (!photonView.IsMine)
            return;

        //Time.timeScale = state ? 0 : 1;
    }

    #endregion
}
