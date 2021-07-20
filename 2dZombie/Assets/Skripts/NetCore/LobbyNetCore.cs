using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace NetCore
{
    public class LobbyNetCore : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Text _logText;

        private void Start()
        {
            PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
            Log("Player name is set to" + PhotonNetwork.NickName);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Log("Connected to Master");
        }

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions {MaxPlayers = 3});
            
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRandomRoom();

        }

        public override void OnJoinedRoom()
        {
            Log("Joined the room");
            
            PhotonNetwork.LoadLevel("1Lvl");
        }
        private void Log(string text)
        {
            Debug.Log(text);
            _logText.text += "\n";
            _logText.text += text;
        }
    }
}
