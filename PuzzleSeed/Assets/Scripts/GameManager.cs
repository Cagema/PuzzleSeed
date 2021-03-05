using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eGamePhase
{
    waiting,
    playerOneTurn,
    playerTwoTurn,
    gameOver
}

public class GameManager : MonoBehaviour
{
    static public GameManager S;
    public static Player CURRENT_PLAYER;
    Player lastPlayer;

    public Match3 match;

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
        match = GetComponent<Match3>();

        InitializePlayers();
    }

    void InitializePlayers()
    {
        players = new List<Player>() { 
            new Player(1, 100), 
            new Player(2, 100) };
        WhyFirst();
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
