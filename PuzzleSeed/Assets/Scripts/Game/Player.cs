using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int playerNum;
    public float hp;
    // List of spells

    public Player(int number, float hPoints = 1f)
    {
        playerNum = number;
        hp = hPoints;
    }
}
