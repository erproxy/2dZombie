using UnityEngine;
using Enemy;

namespace Player
{
    public class PlayerHp : MonoBehaviour
    {
        [SerializeField] private UnitsSettings _unitsSettings;

        private int _hp;
        private int _armor;

        private void Start()
        {
            _hp = _unitsSettings.Hp;
            _armor = _unitsSettings.Armor;
        }

        //Временное решение
        public void TakeDamage(int damage){
            if (_armor>0)
            {
                _armor -= damage;
                if (_armor<0)
                {
                    _hp += _armor;
                }
            }
            else
            {
                _hp -= damage;
                _armor = 0;
            }
            
            if(_hp <= 0)
            {
                Enemy.StationBehavior._listPlayers.Remove(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}