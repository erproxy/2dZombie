using UnityEngine;
using UnityEngine.EventSystems;
using Player;


namespace Ui 
{
    public class DropWeapon: MonoBehaviour, IPointerClickHandler
    {
        private EventPlayer eventPlayer = new EventPlayer();
        

        public void OnPointerClick(PointerEventData eventData)
        {
            // EventPlayer eventPlayer = new EventPlayer();
            // eventPlayer.ClickChangeWeapon += new SomeAction(Test);
            //  eventPlayer.InvokeEventChangeWeapon();

            StationBehavior stationBehavior = new StationBehavior();
            stationBehavior.DropWeapon();
        }


    }
}