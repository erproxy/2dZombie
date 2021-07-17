using UnityEngine;

namespace Bullet
{
    public class PistolBullet: Bullet
    {
        [SerializeField] private Weapons _weapon;
        
        private void Update()
        {
            Moving(_weapon.Dmg);
        }
    }
}