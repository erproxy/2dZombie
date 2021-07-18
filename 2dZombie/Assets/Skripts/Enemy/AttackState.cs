using UnityEngine;
using Player;
namespace Enemy
{
    public class AttackState : BaseState
    {
        public AttackState(Animator animator,Transform handPos, LayerMask whatIsSolid, float attackRange, int dmg, Rigidbody2D rb,
            float movespeed,Transform zombie, float distanceForAttack,IStationStateSwitcher stateSwitcher) 
            : base(animator,handPos, whatIsSolid, attackRange, dmg, rb, movespeed, zombie,distanceForAttack,stateSwitcher)
        {
        }

        public override void Run(GameObject cashPlayer)
        {
            
        }

        public override void Attack( GameObject cashPlayer,ref StationBehavior.Attacking attacking)
        {

  
                 Collider2D[] enemies= Physics2D.OverlapCircleAll(_handPos.transform.position,
                    _attackRange, _whatIsSolid);
                if (enemies != null)
                {
                    Debug.Log("damage");
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        enemies[i].GetComponent<PlayerHp>().TakeDamage(_dmg);
                    }
                } else _stateSwitcher.SwitchState<RunState>();
                attacking = StationBehavior.Attacking.KILLING;
                _stateSwitcher.SwitchState<IdleState>();

        }

        public override void Idle(ref StationBehavior.Attacking attacking)
        {
        }
    }
}