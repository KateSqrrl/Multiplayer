using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject coinPrefab;
    public VictoryWindow window;

    private int numCoins = 25;
    private bool coinsSpawned;

    private PhotonView _PhotonView;
    private Player myPlayer;

    void Start()
    {
        if (!coinsSpawned)
        {
            SpawnCoins();
            coinsSpawned = true;
        }

        SpawnPlayer();

    }


    void SpawnCoins()
    {
        for (int i = 0; i < numCoins; i++)
        {
            Vector3 coinPosition = new Vector3(Random.Range(-8, 8), Random.Range(-4, 4));
            PhotonNetwork.InstantiateRoomObject(coinPrefab.name, coinPosition, Quaternion.identity);
        }
    }

    void SpawnPlayer()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-8, 8), Random.Range(-4, 2));
        GameObject _player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        _PhotonView = _player.GetComponent<PhotonView>();
        myPlayer = _player.GetComponent<Player>();
    }
    public void Leave()
    {

        _PhotonView.RPC("Die", RpcTarget.AllBuffered);
        _PhotonView.RPC("DisableMovement", RpcTarget.AllBuffered);
        PhotonNetwork.LeaveRoom();

    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

        Debug.LogFormat("Player {0} entered the room ", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        
        Debug.LogFormat("Player {0} left the room ", otherPlayer.NickName);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            window.ShowVictoryWindow(PhotonNetwork.LocalPlayer.NickName, myPlayer.score);
        }
    }
}
