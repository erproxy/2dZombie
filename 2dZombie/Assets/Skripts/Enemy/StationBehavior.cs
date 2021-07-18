using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Enemy
{
    public class StationBehavior : MonoBehaviour, IStationStateSwitcher
    {

        [Header("Zombie")] 
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _zombieRb;
        [SerializeField] private float _moveSpeed;
        private bool flipRot = true;
          
        [Header("Player")] 
        public static List<GameObject> _listPlayers = new List<GameObject>();
        private float _dist;
        private float _minDist;
        private GameObject _cashPlayer;

        [Header("Attack")]
        [SerializeField] private Transform _handPos;
        [SerializeField] private LayerMask _whatIsSolid;
        [SerializeField] private float _attackRange;
        [SerializeField] private Weapons _weaponData;
        [SerializeField] private float _distanceForAttack;

        private float _attackStartCouldown=0.4f;
        private float _couldown = 0;


        public enum Attacking{COULDOWN, RDY, KILLING}

        private Attacking _attacking = Attacking.RDY;
        
        private BaseState _currentState;
        private List<BaseState> _allStates;


        private void Start()
        {

            
            _allStates = new List<BaseState>()
            {
                new AttackState(_animator,_handPos, _whatIsSolid, _attackRange, _weaponData.Dmg, _zombieRb, _moveSpeed,
                    gameObject.transform,_distanceForAttack, this),
                new RunState(_animator,_handPos, _whatIsSolid, _attackRange, _weaponData.Dmg, _zombieRb, _moveSpeed,
                    gameObject.transform,_distanceForAttack, this),
                new IdleState(_animator,_handPos, _whatIsSolid, _attackRange, _weaponData.Dmg, _zombieRb, _moveSpeed,
                    gameObject.transform,_distanceForAttack, this),
            };

            _currentState = _allStates[1];
        }

        private void FixedUpdate()
        {
            Run();
        }
        
 
        private void Update()
        {
            if (_listPlayers.Count > 0)
            {
                _minDist = Vector3.Distance(_listPlayers[0].transform.position, gameObject.transform.position);
                _cashPlayer = _listPlayers[0];
                foreach (var GO in _listPlayers)
                {
                    _dist = Vector3.Distance(GO.transform.position, gameObject.transform.position);
                    if (_dist <= _minDist)
                    {
                        _cashPlayer = GO;
                        _minDist = _dist;
                    }
                }
            }
            else _cashPlayer = null;
            

            Attack();
            Relouding();
            Idle();
        }


        //Перезарядка атаки
        private void Relouding()
        {
            if (_attacking == Attacking.KILLING)
            {
                _couldown = _attackStartCouldown;
                _attacking = Attacking.COULDOWN;
            }

            if (_attacking == Attacking.COULDOWN)
            {
                _couldown -= Time.deltaTime;
            }

            if (_couldown<=0)
            {
                _attacking = Attacking.RDY;
            }
            
        }

        public void Run()
        {
         _currentState.Run(_cashPlayer);
        }

        public void Attack()
        {
           _currentState.Attack( _cashPlayer, ref _attacking);
        }

        public void Idle()
        {
            _currentState.Idle(ref _attacking);
        }

        //Удобня настройка дальности атаки через инспектор
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