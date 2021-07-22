using UnityEngine;
using Photon.Pun;
using Random = System.Random;

namespace Enemy 
{
    public class SpawnEnemy : MonoBehaviourPunCallbacks
    {
        
        [SerializeField] private GameObject _enemyPrefab;
        private void Start()
        {
            PhotonNetwork.Instantiate(_enemyPrefab.name, new Vector3(new Random().Next((int) -2f, (int) 2f),
                new Random().Next((int) -2f, (int) 2f)), Quaternion.identity);
        }
    }
}