using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = System.Object;
using Player;


namespace Ui 
{
    
    
    public class ChangeWeapon : MonoBehaviour, IPointerClickHandler
    {
        private EventPlayer eventPlayer = new EventPlayer();

        private void Test()
        {
            Debug.Log("Test");
        }
        private void Start()
        {
            EventPlayer eventPlayer = new EventPlayer();
            eventPlayer.ClickChangeWeapon += new SomeAction(Test);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // EventPlayer eventPlayer = new EventPlayer();
            // eventPlayer.ClickChangeWeapon += new SomeAction(Test);
          //  eventPlayer.InvokeEventChangeWeapon();

          StationBehavior stationBehavior = new StationBehavior();
          stationBehavior.ChangeWeapon();
        }


    }
}