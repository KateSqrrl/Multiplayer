using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_InputField nameInput;


    public void CreateRoom()
    {
        PhotonNetwork.NickName = nameInput.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        roomOptions.CleanupCacheOnLeave = false;
    }

    public void JoinRoom()
    {
        PhotonNetwork.NickName = nameInput.text;
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
