// Класс GameManager.cs предназначен для слежения за состоянием игры и игроков
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public enum eGamePhase
{
    waiting,
    playerOneTurn,
    playerTwoTurn,
    gameOver
}

public class GameManager : MonoBehaviourPun
{
    static public GameManager S;
    public static Player CURRENT_PLAYER;
    Player lastPlayer;

    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    public GameObject PlayerUiPrefab;
    public GameObject ParticaleTurnPrefab;
    GameObject ui1GO;
    GameObject ui2GO;
    RectTransform rect1;
    RectTransform rect2;
    public List<PlayerUI> uiList;

    public Transform canvasTr;

    [Header("Set Dynamically")]
    public List<Player> players;
    public eGamePhase phase = eGamePhase.waiting;
    public int comboCount = 0;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        InitializePlayers();
    }

    void InitializePlayers()
    {
        players = new List<Player>() 
        { 
            new Player(1), 
            new Player(2) 
        };
        uiList = new List<PlayerUI>()
        {
            new PlayerUI(),
            new PlayerUI()
        };
        
        InitializeUI();
        if (PhotonNetwork.IsMasterClient)
            WhyFirst();
    }

    void WhyFirst()
    {
        int randomNum = Random.Range(0, 100) / 50;
        this.photonView.RPC("SetFirstTurn", RpcTarget.All, randomNum);
        
    }

    [PunRPC]
    void SetFirstTurn(int playerNum)
    {
        CURRENT_PLAYER = players[playerNum];
        ChangePhase();
        Instantiate(ParticaleTurnPrefab);
    }

    void InitializeUI()
    {
        ui1GO = Instantiate(PlayerUiPrefab, canvasTr);
        rect1 = ui1GO.GetComponent<RectTransform>();
        uiList[0] = ui1GO.GetComponent<PlayerUI>();
        uiList[0].SetName(PhotonNetwork.CurrentRoom.GetPlayer(1).NickName);
        
        ui2GO = Instantiate(PlayerUiPrefab, canvasTr);
        rect2 = ui2GO.GetComponent<RectTransform>();
        uiList[1] = ui2GO.GetComponent<PlayerUI>();
        uiList[1].SetName(PhotonNetwork.CurrentRoom.GetPlayer(2).NickName);

        if (PhotonNetwork.CurrentRoom.GetPlayer(1).IsLocal)
            rect1.anchoredPosition = new Vector2(-450, 0);
        else
            rect1.anchoredPosition = new Vector2(450, 0);
        if (PhotonNetwork.CurrentRoom.GetPlayer(2).IsLocal)
            rect2.anchoredPosition = new Vector2(-450, 0);
        else
            rect2.anchoredPosition = new Vector2(450, 0);
    }

    void ChangePhase()
    {
        if (CURRENT_PLAYER.playerNum == 1)
        {
            phase = eGamePhase.playerOneTurn;
            //if (rect1.anchoredPosition == new Vector2(-430, 200))
            //    Instantiate(ParticaleTurnPrefab, new Vector2(-7f, 0f), Quaternion.identity);
            //else
            //    Instantiate(ParticaleTurnPrefab, new Vector2(7f, 0f), Quaternion.identity);
        }
        else
        {
            phase = eGamePhase.playerTwoTurn;
            //if (rect2.anchoredPosition == new Vector2(-430, 200))
            //    Instantiate(ParticaleTurnPrefab, new Vector2(-7f, 0f), Quaternion.identity);
            //else
            //    Instantiate(ParticaleTurnPrefab, new Vector2(7f, 0f), Quaternion.identity);
        }
    }

    internal void CylinderManaAdd()
    {
        CURRENT_PLAYER.cylinderMana += 1;
        uiList[CURRENT_PLAYER.playerNum - 1].EditCylinderMana(CURRENT_PLAYER.cylinderMana);
    }

    internal void PyramidManaAdd()
    {
        CURRENT_PLAYER.pyramidMana += 1;
        uiList[CURRENT_PLAYER.playerNum - 1].EditPyramidMana(CURRENT_PLAYER.pyramidMana);
    }

    internal void SphereManaAdd()
    {
        CURRENT_PLAYER.sphereMana += 1;
        uiList[CURRENT_PLAYER.playerNum - 1].EditSphereMana(CURRENT_PLAYER.sphereMana);
    }

    internal void ExpAdd()
    {
        CURRENT_PLAYER.experience += 1;
        uiList[CURRENT_PLAYER.playerNum - 1].EditExp(CURRENT_PLAYER.experience);
    }

    internal void CybeManaAdd()
    {
        CURRENT_PLAYER.cybeMana += 1;
        uiList[CURRENT_PLAYER.playerNum - 1].EditCybeMana(CURRENT_PLAYER.cybeMana);
    }


    public void FinishTurn()
    {
        if (phase == eGamePhase.playerOneTurn ||
            phase == eGamePhase.playerTwoTurn)
            phase = eGamePhase.waiting;
    }

    public void ResetTurn()
    {
        if (phase == eGamePhase.waiting)
        {
            if (CURRENT_PLAYER == players[0])
                phase = eGamePhase.playerOneTurn;
            else
                phase = eGamePhase.playerTwoTurn;
        }
        Debug.Log("reset turn: " + phase.ToString());
    }

    internal void Damage(int damage)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (CURRENT_PLAYER == players[0])
        {
            this.photonView.RPC("ToDamage", RpcTarget.All, 1, (float)damage);
        }
        else
        {
            this.photonView.RPC("ToDamage", RpcTarget.All, 0, (float)damage);
        }
        CheckGameOver();
            
    }

    [PunRPC]
    void ToDamage(int id, float damage)
    {
        players[id].GetDamage(damage);
        uiList[id].EditHealthSlider(damage);
    }

    public void CheckGameOver()
    {
        foreach (var p in players)
            if (!p.CheckHP())
                this.photonView.RPC("GameOver", RpcTarget.All, p.name);
    }

    [PunRPC]
    private void GameOver(string name)
    {
        Debug.Log("Player " + name + " lose!");
        //if (PhotonNetwork.IsMasterClient)
        //    PhotonNetwork.LeaveRoom();
        phase = eGamePhase.gameOver;
    }

    public void FinishMatch()
    {
        comboCount++;
    }

    public void FinishCombo()
    {
        lastPlayer = CURRENT_PLAYER;
        if (CURRENT_PLAYER == players[0])
            CURRENT_PLAYER = players[1];
        else
            CURRENT_PLAYER = players[0];
        ChangePhase();

        Debug.Log("Combo:" + comboCount.ToString() + ", change player");
        comboCount = 0;
    }

    public void SkillShot()
    {
        if (!(phase == eGamePhase.waiting))
        {
            if (CURRENT_PLAYER.cybeMana >= 5 && CURRENT_PLAYER.sphereMana >= 5 && CURRENT_PLAYER.cylinderMana >= 5 && CURRENT_PLAYER.pyramidMana >= 5)
            {
                Damage(20);
                CURRENT_PLAYER.cybeMana -= 5;
                uiList[CURRENT_PLAYER.playerNum - 1].EditCylinderMana(CURRENT_PLAYER.cybeMana);
                CURRENT_PLAYER.sphereMana -= 5;
                uiList[CURRENT_PLAYER.playerNum - 1].EditCylinderMana(CURRENT_PLAYER.sphereMana);
                CURRENT_PLAYER.cylinderMana -= 5;
                uiList[CURRENT_PLAYER.playerNum - 1].EditCylinderMana(CURRENT_PLAYER.cylinderMana);
                CURRENT_PLAYER.pyramidMana -= 5;
                uiList[CURRENT_PLAYER.playerNum - 1].EditCylinderMana(CURRENT_PLAYER.pyramidMana);
                ChangePhase();
            }
            else
            {
                Debug.Log("not mana from " + CURRENT_PLAYER.name);
            }
        }
        
    }
}
