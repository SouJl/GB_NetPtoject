using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonNetManager : MonoBehaviourPunCallbacks
{
    public event Action OnConnectedToServer;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {

            Debug.LogWarning("Can't execute connect while still connected to Photon");
            return;
        }


        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        OnConnectedToServer?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconenctedFromMaster");
    }
}


