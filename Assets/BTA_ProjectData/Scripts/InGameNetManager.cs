using BTAPlayer;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;

public class InGameNetManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _nickName = "player 0";
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private Transform _spawnPoint;
    [SerializeField]
    private float _spawnRadius;
    [SerializeField]
    private GameSceneUI _gameSceneUI;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log($"OnConnectedToMaster");

        PhotonNetwork.LocalPlayer.NickName = _nickName;

        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinedRoom()
    {
        Debug.Log($"OnJoinedRoom");

        SpawnPlayer();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 });
    }


    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected == false)
            return;

        if (_playerPrefab == null)
        { 
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'InGameNetManager'", this);
        }
        else
        {
            if (PlayerController.LocalPlayerInstance == null)
            {
                PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPoint.position, Quaternion.identity, 0);

                PlayerController.LocalPlayerInstance.GetComponent<PlayerController>().SetGameUI(_gameSceneUI);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"OnPlayerEnteredRoom");

        SpawnPlayer();
    }
}
