using UnityEngine;

namespace Enemy
{
    public class RunState : BaseState
    {

        public RunState(Animator animator,Transform handPos, LayerMask whatIsSolid, float attackRange, int dmg, Rigidbody2D rb,
            float movespeed, Transform zombie,float distanceForAttack,IStationStateSwitcher stateSwitcher) 
            : base(animator,handPos, whatIsSolid, attackRange, dmg, rb, movespeed, zombie,distanceForAttack,stateSwitcher)
        {
        }

        public override void Run(GameObject cashPlayer)
        {
            
            if (cashPlayer!=null)
            { 
                _animator.Play("ZombieRun");
                var turn = Quaternion.Lerp (_zombie.rotation, Quaternion.LookRotation (Vector3.forward, cashPlayer.transform.position - _zombie.position), Time.deltaTime * 5f);
                _rb.MoveRotation (turn.eulerAngles.z);
            
                _rb.AddForce(_zombie.transform.up * _movespeed);
            } else _animator.Play("ZombieIdle");
        }
        
        public override void Attack(GameObject cashPlayer, ref StationBehavior.Attacking attacking)
        {
            if (cashPlayer!=null)
            {
                var dist = Vector3.Distance(cashPlayer.transform.position, _zombie.position);
                if (dist<=_distanceForAttack)
                {        
                    _stateSwitcher.SwitchState<AttackState>();
                }
            }

        }

        public override void Idle(ref StationBehavior.Attacking attacking)
        {
        }
    }
}