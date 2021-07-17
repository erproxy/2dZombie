using System;
using UnityEngine;

namespace Bullet
{
    public class Ak47Bullet : Bullet
    {
        [SerializeField] private Weapons _weapon;
        
        private void Update()
        {
            Moving(_weapon.Dmg);
        }
    }
}