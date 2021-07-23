using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetCore;

namespace Ui
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject holder, item;

        public void UpdateRooms()
        {
            var rooms = FindObjectOfType<PhotonLobby>().rooms;
            foreach (Transform item in holder.transform)
            {
                Destroy(item.gameObject);
            }

            print("Rooms: " + rooms.Count);
            for (int i = 0; i < rooms.Count; i++)
            {
                var h = Instantiate(item, holder.transform);
                h.GetComponent<ConnectToRoom>().text.text = rooms[i].Name;
                h.SetActive(true);
            }
        }
    }
}
