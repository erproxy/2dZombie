﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace NetCore
{
    public class ConnectToRoom : MonoBehaviour
    {
        public TMPro.TMP_Text text;



        public void Connect()
        {
            FindObjectOfType<PhotonLobby>().JoinRoom(text);
        }
    }
}