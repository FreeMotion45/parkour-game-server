using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Shared
{
    class PlayerGameInformation : MonoBehaviour
    {
        public int health = 100;

        public int Damage(int points)
        {
            health = health - points < 0 ? 0 : health - points;
            return health;
        }

        public bool IsDead()
        {
            return health == 0;
        }
    }
}
