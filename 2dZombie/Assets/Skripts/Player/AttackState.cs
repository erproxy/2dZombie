using UnityEngine;

namespace Player
{
    public class AttackState : BaseState
    {
        public AttackState(Joystick attackJoystick, Joystick movementJoystick, Animator animator,
            IStationStateSwitcher stateSwitcher) 
            : base(attackJoystick, movementJoystick, animator, stateSwitcher)
        {
        }

        public override void Idle(ref StationBehavior.Weapon weapon)
        {
           
        }

        public override void Run(Rigidbody2D rb, float moveSpeed, ref StationBehavior.Weapon weapon)
        {
           
        }

        public override void Attack()
        {
          
        }
    }
}