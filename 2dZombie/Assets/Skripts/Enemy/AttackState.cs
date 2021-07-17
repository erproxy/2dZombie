using UnityEngine;

namespace Enemy
{
    public class AttackState : BaseState
    {
        public AttackState(Animator animator, IStationStateSwitcher stateSwitcher) : base(animator, stateSwitcher)
        {
        }

        public override void Run(Rigidbody2D transform, float moveSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override void Attack()
        {
            throw new System.NotImplementedException();
        }
    }
}