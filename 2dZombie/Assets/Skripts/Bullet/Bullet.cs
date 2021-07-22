using Enemy;
using UnityEngine;
using Player;

namespace Bullet
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _whatIsSolid;


        protected void Moving(int _damage)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, _distance, _whatIsSolid);
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Enemy"))
                {
                    hitInfo.collider.GetComponent<EnemyHp>().TakeDamage(_damage);
                    Destroy(gameObject);
                }
                else if (hitInfo.collider.CompareTag("Wall"))
                {
                    Destroy(gameObject);
                } else if (hitInfo.collider.CompareTag("Player"))
                {
                    Destroy(gameObject);
                }
                
             //   Player.StationBehavior._listBulletsReadyForShot.Add(gameObject);
                gameObject.SetActive(false);
            }

            transform.Translate(Vector2.up * (_speed * Time.deltaTime));
        }
    }
}