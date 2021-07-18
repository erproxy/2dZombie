using UnityEngine;

namespace Enemy
{
    public abstract class BaseState
    {

        protected readonly Transform _handPos;
        protected readonly LayerMask _whatIsSolid;
        protected readonly float _attackRange;
        protected readonly int _dmg;
        protected readonly IStationStateSwitcher _stateSwitcher;
        protected readonly Animator _animator;
        protected readonly Rigidbody2D _rb;
        protected readonly float _movespeed;
        protected readonly Transform _zombie;
        protected readonly float _distanceForAttack;
        protected BaseState( Animator animator, Transform handPos, LayerMask whatIsSolid, float attackRange, int dmg, 
            Rigidbody2D rb, float movespeed,Transform zombie, float distanceForAttack, IStationStateSwitcher stateSwitcher)
        {
            _distanceForAttack = distanceForAttack;
            _zombie = zombie;
            _rb = rb;
            _movespeed = movespeed;
            _handPos = handPos;
            _whatIsSolid = whatIsSolid;
            _attackRange = attackRange;
            _dmg = dmg;
            _animator = animator;
            _stateSwitcher = stateSwitcher;
        }

        public abstract void Run(GameObject cashPlayer);

        public abstract void Attack(GameObject cashPlayer,ref StationBehavior.Attacking attacking);

        public abstract void Idle(ref StationBehavior.Attacking attacking);
    }
}