using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;


namespace NetCore
{
    public class LvlNetCore : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _playerPrefab;
        private void Start()
        {
            PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(new Random().Next((int) -2f, (int) 2f),
                new Random().Next((int) -2f, (int) 2f)), Quaternion.identity);
        }

        public void Leave()
        {
            PhotonNetwork.LeaveRoom();
        }
        public override void OnLeftRoom()
        {
            //Когда игрок покидает комнату
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.LogFormat("Player{0} entered room", newPlayer.NickName);
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.LogFormat("Player{0} entered room", otherPlayer.NickName);
        }
    }
}