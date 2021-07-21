using UnityEngine;

namespace Player
{
    public delegate void SomeAction();
    public class EventPlayer : MonoBehaviour
    {
        public event SomeAction ClickChangeWeapon;
        
        public void InvokeEventChangeWeapon()
        {
            ClickChangeWeapon?.Invoke();
        }
        
    }
}