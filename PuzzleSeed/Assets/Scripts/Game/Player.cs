// Класс Player.cs описывает игрока
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int playerNum { get; set; }
    float hp;
    public int cybeMana { get; set; }
    public int sphereMana { get; set; }
    public int cylinderMana { get; set; }
    public int pyramidMana { get; set; }
    public int experience { get; set; }
    public string name { get; private set; }
    // List of spells

    public Player(int number, float hPoints = 100f)
    {
        playerNum = number;
        hp = hPoints;
        name = "Player " + playerNum.ToString();
        cybeMana = 0;
        sphereMana = 0;
        cylinderMana = 0;
        pyramidMana = 0;
        experience = 0;
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
