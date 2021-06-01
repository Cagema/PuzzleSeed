// Класс SkillButtonScript.cs предназначен для управления кнопкой интерфейса игрока
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SkillButtonScript : MonoBehaviour
{
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.GetPlayer(GameManager.CURRENT_PLAYER.playerNum).IsLocal)
        {
            if (transform.parent.localPosition.x < 0)
                button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
