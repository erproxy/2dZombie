using UnityEngine;

namespace Enemy
{
    public class IdleState : BaseState
    {
        public IdleState(Animator animator, Transform handPos, LayerMask whatIsSolid, float attackRange, int dmg, Rigidbody2D rb, float movespeed, Transform zombie, float distanceForAttack, IStationStateSwitcher stateSwitcher) : base(animator, handPos, whatIsSolid, attackRange, dmg, rb, movespeed, zombie, distanceForAttack, stateSwitcher)
        {
        }

        public override void Run(GameObject cashPlayer)
        {
        }

        public override void Attack(GameObject cashPlayer, ref StationBehavior.Attacking attacking)
        {
            if (attacking == StationBehavior.Attacking.RDY)
            {
                _stateSwitcher.SwitchState<RunState>();
            } 
        }

        public override void Idle(ref StationBehavior.Attacking attacking)
        {
            
        }
    }
}