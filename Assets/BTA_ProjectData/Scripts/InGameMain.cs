using BTAPlayer;
using MultiplayerService;
using Photon.Pun;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class InGameMain : MonoBehaviour
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
    private List<PlayerController> _playerControllers = new List<PlayerController>();

    private void Start()
    {
        Random.InitState(_randomSeed);

        _mainCamera = Camera.main;

        if (_netManager.IsConnected)
        {
            SpawnPlayer();
            return;
        }
        else
        {
            PhotonNetwork.LoadLevel(0);
        }
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

            if(playerNum > 0)
            {
                spawnPosition = _spawnPoints[playerNum].position;
            }

            var playerObject = _netManager.CreatePlayer(_playerPrefab, spawnPosition);

            var playerView = playerObject.GetComponent<PlayerView>();

            _playerControllers.Add(new PlayerController(_playerConfig, playerView, _gameSceneUI, _mainCamera));
        }
    }

    private void Update()
    {
        if (_playerControllers == null && _playerControllers.Count == 0)
            return;

        for(int i =0; i < _playerControllers.Count; i++)
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
}
