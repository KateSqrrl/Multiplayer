using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class VictoryWindow : MonoBehaviourPunCallbacks
{
    public GameObject victoryWindow;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerScoreText;


    public void ShowVictoryWindow(string playername, int score)
    {
        playerNameText.text = playername;
        playerScoreText.text = score.ToString();

        victoryWindow.transform.localScale = Vector3.one;

        photonView.RPC("RPC_ShowVictoryWindow", RpcTarget.OthersBuffered, playername, score);
    }


     [PunRPC]
     private void RPC_ShowVictoryWindow(string playername, int score)
     {
         playerNameText.text = playername;
         playerScoreText.text = score.ToString();

         victoryWindow.transform.localScale = Vector3.one;
     }

}

