using UnityEngine;

namespace Enemy
{
    public class AnimationAttack : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        
        
        //Ивент для конца анимации атаки
        public void EndAnimationAttack()
        {
            _animator.Play("ZombieIdle");
        }
    }
}