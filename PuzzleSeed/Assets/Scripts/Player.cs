using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int playerNum;
    public int hp;
    // List of spells
    public int moveCount;
    public Player(int number, int hPoints = 100, int count = 1)
    {
        playerNum = number;
        hp = hPoints;
        moveCount = count;
    }
}
