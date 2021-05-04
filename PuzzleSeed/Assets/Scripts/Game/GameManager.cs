using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
        players = new List<Player>() { 
            new Player(1), 
            new Player(2) };
        WhyFirst();

        GameObject uiGO = Instantiate(PlayerUiPrefab, canvasTr);
        RectTransform rect = uiGO.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(-430, 200);
        uiGO.GetComponent<PlayerUI>().SetName(PhotonNetwork.CurrentRoom.GetPlayer(1).NickName);
        uiGO = Instantiate(PlayerUiPrefab, canvasTr);
        rect = uiGO.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(430, 200);
        uiGO.GetComponent<PlayerUI>().SetName(PhotonNetwork.CurrentRoom.GetPlayer(2).NickName);

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    GameObject uiGO = PhotonNetwork.Instantiate(PlayerUiPrefab.name, new Vector2(-480, 200), Quaternion.identity, 0);
        //    uiGO.GetComponent<PlayerUI>().SetName(PhotonNetwork.CurrentRoom.GetPlayer(1).NickName);
        //}
        //else
        //{
        //    GameObject uiGO = PhotonNetwork.Instantiate(PlayerUiPrefab.name, new Vector2(480, 200), Quaternion.identity, 0);
        //    uiGO.GetComponent<PlayerUI>().SetName(PhotonNetwork.CurrentRoom.GetPlayer(2).NickName);
        //}

    }

    void WhyFirst()
    {
        int randomNum = Random.Range(0, 100) / 50;
        CURRENT_PLAYER = players[randomNum];
        ChangePhase();
    }

    void ChangePhase()
    {
        if (CURRENT_PLAYER.playerNum == 1)
            phase = eGamePhase.playerOneTurn;
        else
            phase = eGamePhase.playerTwoTurn;
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

}
