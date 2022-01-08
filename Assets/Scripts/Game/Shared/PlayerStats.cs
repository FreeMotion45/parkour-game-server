using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    public int Heal(int points)
    {
        health = health + points > 100 ? 100 : health + points;
        return health;
    }

    public int Damage(int points)
    {
        health = health - points < 0 ? 0 : health - points;
        return health;
    }

    public bool IsDead()
    {
        return health == 0;
    }

    public void RevivePlayer(int revivedHealth)
    {
        health = revivedHealth;
    }
}
