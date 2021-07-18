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
        
        [Tooltip("Слои, которые будут атакованы")]
        [SerializeField] private LayerMask _whatIsSolid;
        [SerializeField] private float _attackRangeKnife;
        [SerializeField] private Weapons _weaponData;
        
        //боезапас
        [Tooltip("Боезопас Пистолета")]
        [SerializeField] private int _ammunitionStartPistol;
        [Tooltip("Боезопас Ак47")]
        [SerializeField] private int _ammunitionStartAk47;
        private int _ammunitionPistol;
        private int _ammunitionAk47;
        
        //Время перезарядки
        [Tooltip("Стартовое время перезарядки Пистолета")]
        [SerializeField] private float _reloadingStartTimePistol;
        [Tooltip("Стартовое время перезарядки Ак47")]
        [SerializeField] private float _reloadingStartTimeAk47;
        private float _reloadingTimePistol;
        private float _reloadingTimeAk47;
        
        private float _attackStartTimePistol=0.3f;
        private float _attackStartTimeAk47=0.1f;
        private float _attackStartKnife=0.1f;
        private float _couldown = 0;
        
        private List<GameObject> _listBullets;
        public static List<GameObject> _listBulletsReadyForShot = new List<GameObject>();
        
        
        
        
        
        
        private List<Weapon> _weaponsInHand = new List<Weapon>();
        protected int id = 0;
        
        //Все Enum
        
        public enum RdyToShotAk47{AMMUNITION, RDY}
        private RdyToShotAk47 _rdyToShotAk47=RdyToShotAk47.RDY;
        public enum RdyToShotPistol{AMMUNITION, RDY}
        private RdyToShotPistol _rdyToShotPistol=RdyToShotPistol.RDY;
        public enum Weapon{KNIFE,PISTOL,AK47};
        private Weapon _weapon = Weapon.KNIFE;
        
        private BaseState _currentState;
        private List<BaseState> _allStates;


        private void Start()
        {
            _ammunitionPistol = _ammunitionStartPistol;
            _ammunitionAk47 = _ammunitionStartAk47;
            
            _reloadingTimePistol = _reloadingStartTimePistol;
            _reloadingTimeAk47 = _reloadingStartTimeAk47;
            
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
            RechargeAttackCouldown();
        }    
        //Сброс кулдауна на атаку
        private void RechargeAttackCouldown()
        {
            if (_weapon == Weapon.AK47)  _couldown = _attackStartTimeAk47;
            else if (_weapon==Weapon.PISTOL) _couldown = _attackStartTimePistol;
            else if (_weapon==Weapon.KNIFE)_couldown = _attackStartKnife;
        }    
        
        //Перезарядка оружия
        private void Reloading()
        {
            if (_rdyToShotAk47 == RdyToShotAk47.AMMUNITION)
            {
                if (_reloadingTimeAk47 <= 0)
                {
                    _ammunitionAk47 = _ammunitionStartAk47;
                    _reloadingTimeAk47 = _reloadingStartTimeAk47;
                    _rdyToShotAk47 = RdyToShotAk47.RDY;
                }
                else _reloadingTimeAk47 -= Time.deltaTime;
            }

            if (_rdyToShotPistol == RdyToShotPistol.AMMUNITION)
            {
                if (_reloadingTimePistol <= 0)
                {
                    _ammunitionPistol = _ammunitionStartPistol;
                    _reloadingTimePistol = _reloadingStartTimePistol;
                    _rdyToShotPistol = RdyToShotPistol.RDY;
                }
                else _reloadingTimePistol -= Time.deltaTime;
            }
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
            if (_attackJoystick.Horizontal != 0 && _attackJoystick.Vertical != 0)
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
                        Collider2D[] enemies = Physics2D.OverlapCircleAll(_muzzlePos.transform.position,
                            _attackRangeKnife, _whatIsSolid);
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
                    }
                    else
                    {
                        GameObject cashBullet = null;
                        //Запихивание в кэш префаб пули, который нужно будет заспавнить или выпустить
                        if (_weapon == Weapon.AK47 && _rdyToShotAk47!=RdyToShotAk47.AMMUNITION)
                        {
                            int id = _listBulletsReadyForShot.Count - 1;
                            while (id >= 0)
                            {
                                if (_listBulletsReadyForShot[id].tag == "BulletAk47")
                                {
                                    cashBullet = _listBulletsReadyForShot[id];
                                    RemoveFromPull(id);
                                    _ammunitionAk47--;
                                    break;
                                }

                                id--;
                            }
                        }
                        else if (_weapon == Weapon.PISTOL && _rdyToShotPistol!= RdyToShotPistol.AMMUNITION)
                        {
                            int id = _listBulletsReadyForShot.Count - 1;
                            while (id >= 0)
                            {
                                if (_listBulletsReadyForShot[id].tag == "BulletPistol")
                                {
                                    cashBullet = _listBulletsReadyForShot[id];
                                    RemoveFromPull(id);
                                    _rdyToShotPistol--;
                                    break;
                                }

                                id--;
                            }
                        }

                        if (cashBullet == null)
                        {
                            {
                                if (_weapon == Weapon.AK47&& _rdyToShotAk47!=RdyToShotAk47.AMMUNITION)
                                {
                                    Instantiate(_prefabBulletAk47, _muzzlePos.transform.position,
                                        _playerModel.transform.rotation, _parent.transform);
                                    _ammunitionAk47--;
                                }
                                else if (_weapon == Weapon.PISTOL&& _rdyToShotPistol!= RdyToShotPistol.AMMUNITION)
                                {
                                    Instantiate(_prefabBulletPistol, _muzzlePos.transform.position,
                                        _playerModel.transform.rotation, _parent.transform);
                                    _rdyToShotPistol--;
                                }
                            }
                        }
                    }

                    RechargeAttackCouldown();
                }
                else _couldown -= Time.deltaTime;
            }

            if (_ammunitionAk47 == 0) _rdyToShotAk47 = RdyToShotAk47.AMMUNITION;
            if (_ammunitionPistol == 0) _rdyToShotPistol = RdyToShotPistol.AMMUNITION;
            Reloading();
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