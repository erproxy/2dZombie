using UnityEngine;

namespace Enemy
{
    public class EnemyHp : MonoBehaviour
    {
        [SerializeField] private UnitsSettings _unitsSettings;

        private int _hp;

        private void Start()
        {
            _hp = _unitsSettings.Hp;
        }

        //Временное решение, в будущем необходимо создат пул Врагов, по аналогии с пулями
        public void TakeDamage(int damage){
            Debug.Log(damage);
            _hp -= damage;
            if(_hp <= 0){
                gameObject.SetActive(false);
            }
        }
    }
}