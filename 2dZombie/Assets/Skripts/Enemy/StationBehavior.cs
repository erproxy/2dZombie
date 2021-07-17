using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Enemy
{
    public class StationBehavior : MonoBehaviour, IStationStateSwitcher
    {

        [Header("Zombie")] 
        [SerializeField] private Transform _zombieModel;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _zombieRb;
        [SerializeField] private float _moveSpeed;
        private bool flipRot = true;
          
        [Header("Player")] 
        public static List<GameObject> _listPlayers = new List<GameObject>();
        private float _dist;
        private float _minDist;
        private GameObject _cashEnemy;

        [Header("Attack")]
        [SerializeField] private GameObject _handPos;
        [SerializeField] private LayerMask _whatIsSolid;
        [SerializeField] private float _attackRange;
        [SerializeField] private Weapons _weaponData;
        
        private float _attackCouldown=0.2f;
        private float _couldown = 0;

        
        private BaseState _currentState;
        private List<BaseState> _allStates;


        private void Start()
        {

            
            _allStates = new List<BaseState>()
            {
                new AttackState(_animator, this),
                new RunState(_animator,this)
            };

            _currentState = _allStates[0];
        }

        private void FixedUpdate()
        {
            Run();
        }
        

        //перезарядка оружия
        private void Recharge()
        {
            _couldown = _attackCouldown;
        }
 
        private void Update()
        {
            if (_listPlayers.Count>0)
            {
                _minDist = Vector3.Distance(_listPlayers[0].transform.position, gameObject.transform.position);
                foreach (var GO in _listPlayers)
                {
                    _dist = Vector3.Distance(GO.transform.position, gameObject.transform.position);
                    if (_dist<=_minDist)
                    {
                        _cashEnemy = GO;
                        _minDist = _dist;
                    }
                }
            }

            Attack();
        }
        

        public void Run()
        {
       //     _currentState.Run();
        }

        public void Attack()
        {
           
        }


        
        
        //Удобная настройка дальности атаки через инспектор
        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_handPos.transform.position, _attackRange);
        }
        
        public void SwitchState<T>() where T : BaseState
        {
            var state = _allStates.FirstOrDefault(s => s is T);
            _currentState = state;
        }
    }
}