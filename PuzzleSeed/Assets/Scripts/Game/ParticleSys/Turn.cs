using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Turn : MonoBehaviour
{
    // turn particle effect
    GameManager game;

    void Start()
    {
        game = GameManager.S;
        //Invoke("DestroyTurnPart", 3f);
    }

    private void Update()
    {
        if (game.phase == eGamePhase.playerOneTurn)
        {
            if (PhotonNetwork.IsMasterClient)
                transform.position = new Vector2(-4f, 2.7f);
            else
                transform.position = new Vector2(4f, 2.7f);
        }
        else if (game.phase == eGamePhase.playerTwoTurn)
        {
            if (PhotonNetwork.IsMasterClient)
                transform.position = new Vector2(4f, 2.7f);
            else
                transform.position = new Vector2(-4f, 2.7f);
        }
    }

    void DestroyTurnPart()
    {
        Destroy(this.gameObject);
    }
}
