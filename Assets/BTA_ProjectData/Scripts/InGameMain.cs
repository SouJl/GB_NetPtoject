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
    private float _extractionTime = 5f;
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

    private bool _isReady;
    private bool _isGameOver;

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

        if (!_netManager.IsConnected)
        {
            _netManager.OnConnectedToServer += ConnectedToServer;
            _netManager.OnJoinInLobby += JoindedInLobby;
            _netManager.OnJoinInRoom += JoinedInRoom;
            _netManager.Connect(_gamePrefs.GetUser().Name);
        }
        else
        {
            TestGameStart();

            _isReady = true;
        }
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
        TestGameStart();

        _isReady = true;
    }

    private void TestGameStart()
    {
        GameStateManager.PlayersCount = _netManager.CurrentRoom.PlayerCount;

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

        playerController.OnDead += PlayerDead;
        playerController.OnRevive += PlayerRevive;

        _enemySpawner.AddPlayer(playerController);

        _playerControllers.Add(playerController);

        GameStateManager.AddPlayer(playerController.View);

        _mainTaskId = TaskManager.AddNewTask("REACH CENTRAL ROOM");
    }

    #endregion

    private void Subscribe()
    { 
        _netManager.OnPlayerLeftFromRoom += PlayerLeftedGame;
        _dataServerService.OnGetUserData += UserDataLoaded;
        _netManager.OnDisconnectedFromServer += Disconnected;

        _gameUI.OnRestartGame += RestartGame;
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
        
        _gameUI.OnRestartGame -= RestartGame;
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

            StartCoroutine(WaitForExtraction());
        }
    }

    private IEnumerator WaitForExtraction()
    {
        _gameUI.PlayerViewUI.StartExitCountDown(_extractionTime);

        yield return new WaitForSeconds(_extractionTime);

        _gameUI.PlayerViewUI.StopExitCountDown();

        photonView.RPC(nameof(ExtractionPlayers), RpcTarget.All);
    }

    [PunRPC]
    private void ExtractionPlayers()
    {
        GameStateManager.GameWon();

        _isReady = false;
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

        _isReady = true;
    }

    private void Disconnected()
    {
        Dispose();

        SceneManager.LoadScene(0);
    }

    private void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RestartScene), RpcTarget.AllViaServer);
        }
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
        GameStateManager.PlayersCount = _netManager.CurrentRoom.PlayerCount;

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

        playerController.OnDead += PlayerDead;
        playerController.OnRevive += PlayerRevive;

        _enemySpawner.AddPlayer(playerController);

        _playerControllers.Add(playerController);

        GameStateManager.AddPlayer(playerController.View);

        photonView.RPC(
           nameof(InstantiatePlayer),
           RpcTarget.Others,
           playerPhoton.ViewID,
           _netManager.CurrentPlayer,
           spawnPosition,
           rotation);
    }

    private void PlayerDead()
    {
        GameStateManager.PlayersCount--;
        
        Debug.Log($"PlayersCount = {GameStateManager.PlayersCount}");

        photonView.RPC(nameof(UpdatePlayersCount), RpcTarget.Others, new object[] { GameStateManager.PlayersCount });
    }

    private void PlayerRevive()
    {
        GameStateManager.PlayersCount++;

        Debug.Log($"PlayersCount = {GameStateManager.PlayersCount}");

        photonView.RPC(nameof(UpdatePlayersCount), RpcTarget.Others, new object[] { GameStateManager.PlayersCount });
    }

    [PunRPC]
    private void UpdatePlayersCount(int value)
    {
        GameStateManager.PlayersCount = value;
        
        Debug.Log($"PlayersCount = {GameStateManager.PlayersCount}");
    }

    [PunRPC]
    private void RestartScene()
    {
        Dispose();

        SceneManager.LoadScene(1);
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

            playerController.OnDead += PlayerDead;
            playerController.OnRevive += PlayerRevive;

            _playerControllers.Add(playerController);

            _enemySpawner.AddPlayer(playerController);

            GameStateManager.AddPlayer(playerController.View);
        }
    }

    private void SpawnEmemy()
    {
        StartCoroutine(StartSpawnEnemies());
    }

    private IEnumerator StartSpawnEnemies()
    {
        yield return new WaitForSeconds(1f);

        _enemySpawner.StartEnemySpawn();
    }

    private void Update()
    {
        if (_isGameOver)
            return;

        if (_isPaused)
            return;

        if (!_isReady)
            return;

        if (GameStateManager.PlayersCount == 0)
        {
            _isGameOver = true;
            GameStateManager.GameOver();
            return;
        }

        if (_playerControllers == null || _playerControllers.Count == 0)
            return;

        for (int i = 0; i < _playerControllers.Count; i++)
        {
            _playerControllers[i].ExecuteUpdate(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {

        if (_isGameOver)
            return;

        if (_isPaused)
            return;
        
        if (!_isReady)
            return;

        if (_playerControllers == null || _playerControllers.Count == 0)
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

        for (int i = 0; i < _playerControllers.Count; i++)
        {
            var player = _playerControllers[i];
            
            player.OnDead -= PlayerDead;
            player.OnDead -= PlayerRevive;
        }

        _playerControllers.Clear();

        TaskManager.Release();

        GameStateManager.Release();

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
