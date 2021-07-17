using UnityEngine;

namespace Enemy
{
    public abstract class BaseState
    {
        protected readonly IStationStateSwitcher _stateSwitcher;
        protected readonly Animator _animator;

        protected BaseState( Animator animator,
            IStationStateSwitcher stateSwitcher)
        {
            _animator = animator;
            _stateSwitcher = stateSwitcher;
        }

        public abstract void Run(Rigidbody2D transform, float moveSpeed);

        public abstract void Attack();
    }
}