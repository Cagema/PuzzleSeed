using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    public static OnlineManager S;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveRoom();
        }
    }
    public void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level");
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }
}
