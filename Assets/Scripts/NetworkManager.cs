using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    string _room = "My_Room";

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        if (PhotonNetwork.connected)
        {
            Debug.Log("Connected");
        }
    }

    void OnJoinedLobby()
    {
        Debug.Log("joined lobby");

        RoomOptions roomOptions = new RoomOptions() { };
        PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.playerList.Length);

        if (PhotonNetwork.playerList.Length < 2)
        {
            PhotonNetwork.Instantiate("NetworkedGameMaster", new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity, 0);
        } else if (PhotonNetwork.playerList.Length < 3)
        {
			PhotonNetwork.Instantiate("NetworkedWarrior", new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity, 0);
        }
        else if (PhotonNetwork.playerList.Length < 4)
        {
            PhotonNetwork.Instantiate("NetworkedMagus", new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity, 0);
        }
        else 
        {
            PhotonNetwork.Instantiate("NetworkedRogue", new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity, 0);
        }
    }
}
