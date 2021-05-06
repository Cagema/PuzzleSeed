using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int playerNum { get; set; }
    float hp;
    public string name { get; private set; }
    // List of spells

    public Player(int number, float hPoints = 100f)
    {
        playerNum = number;
        hp = hPoints;
        name = "Player " + playerNum.ToString();
    }

    public void GetDamage(float damage)
    {
        hp -= damage;
    }

    //If hp > 0 then true
    public bool CheckHP()
    {
        bool flag = false;
        if (hp > 0)
            flag = true;
        return flag;
    }
}
