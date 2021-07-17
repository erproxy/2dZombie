using System;
using System.Collections.Generic;
using Enemy;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class StationBehavior : MonoBehaviour, IStationStateSwitcher
    {
        [Header("Joysticks")]
        [SerializeField] private Joystick _movementJoystick;
        [SerializeField] private Joystick _attackJoystick;

        [Header("Player")] 
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _playerRb;
        [SerializeField] private float _moveSpeed;
        private bool flipRot = true;


        [Header("Attack")]
        [SerializeField] private GameObject _parent;
        [SerializeField] private GameObject _muzzlePos;
        [SerializeField] private GameObject _prefabBulletAk47;
        [SerializeField] private GameObject _prefabBulletPistol;
        [SerializeField] private LayerMask _whatIsSolid;
        [SerializeField] private float _attackRangeKnife;
        [SerializeField] private Weapons _weaponData;
        
        
        private float _attackStartTimePistol=0.3f;
        private float _attackStartTimeAk47=0.1f;
        private float _attackStartKnife=0.1f;
        private float _couldown = 0;
        
        private List<GameObject> _listBullets;
        public static List<GameObject> _listBulletsReadyForShot = new List<GameObject>();
        
        
        
        
        
        private List<Weapon> _weaponsInHand = new List<Weapon>();
        protected int id = 0;
        public enum Weapon{KNIFE,PISTOL,AK47};

        private Weapon _weapon = Weapon.KNIFE;
        
        private BaseState _currentState;
        private List<BaseState> _allStates;


        private void Start()
        {
            _weaponsInHand.Add(Weapon.KNIFE);
            _weaponsInHand.Add(Weapon.PISTOL);
            _weaponsInHand.Add(Weapon.AK47);

            
            _allStates = new List<BaseState>()
            {
                new IdleState(_attackJoystick,_movementJoystick,_animator,this),
                new AttackState(_attackJoystick,_movementJoystick,_animator, this),
                new RunState(_attackJoystick,_movementJoystick,_animator,this)
            };

            _currentState = _allStates[0];
        }

        private void FixedUpdate()
        {
            Idle();
            Run();
        }
        
        //Смена оружия
        public void ChangeWeapon()
        {
            
            id++;
            if (id==_weaponsInHand.Count)
            {
                id = 0;
            }
            _weapon = _weaponsInHand[id];
            Recharge();
        }    
        //перезарядка оружия
        private void Recharge()
        {
            if (_weapon == Weapon.AK47)  _couldown = _attackStartTimeAk47;
            else if (_weapon==Weapon.PISTOL) _couldown = _attackStartTimePistol;
            else if (_weapon==Weapon.KNIFE)_couldown = _attackStartKnife;
        }
        //Выкидывания оружия
        public void DropWeapon()
        {
            if (id != 0)
            {
                _weaponsInHand.RemoveAt(id);
                id--;
            }
            _weapon = _weaponsInHand[id];
        }
        private void Update()
        {
            Attack();
        }

        public void Idle()
        {
            _currentState.Idle(ref _weapon);
        }

        public void Run()
        {
            _currentState.Run(_playerRb, _moveSpeed, ref _weapon);
        }

        public void Attack()
        {
            if (_attackJoystick.Horizontal!=0 && _attackJoystick.Vertical != 0)
            {
                Vector2 movement;
                movement.x = _attackJoystick.Horizontal;
                movement.y = _attackJoystick.Vertical;
                float angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
                angle = flipRot ? -angle : angle;


                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                //Если кулдаун пропал, происходит атака
                if (_couldown <= 0)
                {
                    //Если в руке нож произойдет одна атака, в противном случае выпускается пуля
                    if (_weapon == Weapon.KNIFE)
                    {
                        _animator.Play("AttackKnife");
                        Collider2D[] enemies = Physics2D.OverlapCircleAll (_muzzlePos.transform.position, _attackRangeKnife,  _whatIsSolid);
                        if (enemies != null)
                        {
                            for (int i = 0; i < enemies.Length; i++)
                            {
                                if (enemies[i].CompareTag("Enemy"))
                                {
                                    enemies[i].GetComponent<EnemyHp>().TakeDamage(_weaponData.Dmg);
                                }
                                else if (enemies[i].CompareTag("Wall"))
                                {

                                }
                            }
                        }
                        else
                        {
                            
                        }
                    }else
                    {
                        GameObject cashBullet = null;
                        //Запихивание в кэш префаб пули, который нужно будет заспавнить или выпустить
                        if (_weapon == Weapon.AK47)
                        {
                            int id = _listBulletsReadyForShot.Count - 1;
                            while (id >= 0)
                            {
                                if (_listBulletsReadyForShot[id].tag == "BulletAk47")
                                {
                                    cashBullet = _listBulletsReadyForShot[id];
                                    RemoveFromPull(id);
                                    break;
                                }
                                
                                id--;
                            }
                        }
                        else if (_weapon == Weapon.PISTOL)
                        {
                            int id = _listBulletsReadyForShot.Count - 1;
                            while (id >= 0)
                            {
                                if (_listBulletsReadyForShot[id].tag == "BulletPistol")
                                {
                                    cashBullet = _listBulletsReadyForShot[id];
                                    RemoveFromPull(id);
                                    break;
                                }
                                id--;
                            }
                        }

                        if (cashBullet == null)
                        {
                            {
                                if (_weapon == Weapon.AK47)
                                {
                                    Instantiate(_prefabBulletAk47, _muzzlePos.transform.position,
                                        _playerModel.transform.rotation, _parent.transform);
                                }
                                else if (_weapon == Weapon.PISTOL)
                                {
                                    Instantiate(_prefabBulletPistol, _muzzlePos.transform.position,
                                        _playerModel.transform.rotation, _parent.transform);
                                }
                            }
                        }
                    }
                    Recharge();
                }
                else _couldown -= Time.deltaTime;
            }
        }

        //Активация Пули и убирание его из пула
        private void RemoveFromPull(int id)
        {

            _listBulletsReadyForShot[id].SetActive(true);
            _listBulletsReadyForShot[id].transform.position = _muzzlePos.transform.position;
            _listBulletsReadyForShot[id].transform.rotation = _muzzlePos.transform.rotation;
            _listBulletsReadyForShot.RemoveAt(id);

        }
        
        //Удобня настройка дальности атаки через инспектор
        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_muzzlePos.transform.position, _attackRangeKnife);
        }
        
        public void SwitchState<T>() where T : BaseState
        {
            var state = _allStates.FirstOrDefault(s => s is T);
            _currentState = state;
        }
    }
}