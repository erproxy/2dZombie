using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Player
{
    
    public class StationBehavior : MonoBehaviour, IStationStateSwitcher
    {
        private PhotonView _photonView;
        
        
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
        private GameObject _parent;
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
        
 
        
        private float _attackStartTimePistol=0.3f;
        private float _attackStartTimeAk47=0.1f;
        private float _attackStartKnife=0.3f;
        private float _couldown = 0;
        
        private List<GameObject> _listBullets;
        public static List<GameObject> _listBulletsReadyForShot = new List<GameObject>();

        private List<Weapon> _weaponsInHand = new List<Weapon>();
        protected int id = 0;

        [Header("Звуковое сопровождение")]
        [SerializeField] private AudioClip[] _audioclipMoving;
        [SerializeField] private AudioSource _audioSourceMoving;
        
        //Звуки Ак47
        [SerializeField] private AudioClip[] _audioclipFireAk47;
        [SerializeField] private AudioSource _audioSourceFireAk47;
        [SerializeField] private AudioClip[] _audioclipReloadingAk47;
        [SerializeField] private AudioSource _audioSourceReloadingAk47;
        //Звуки пистолета
        [SerializeField] private AudioClip[] _audioclipFirePistol;
        [SerializeField] private AudioSource _audioSourceFirePistol;
        [SerializeField] private AudioClip[] _audioclipReloadingPistol;
        [SerializeField] private AudioSource _audioSourceReloadingPistol;
        //Звуки ножа
        [SerializeField] private AudioClip[] _audioClipAttackEnemy;
        [SerializeField] private AudioClip[] _audioClipAttackWall;
        [SerializeField] private AudioClip[] _audioClipAttackNothing;
        [SerializeField] private AudioSource _audioSourceAttackKnife;
        

        //Все Enum

        public enum HitEnemyWithKnife{ENEMY,WALL,NOTHING }

        private HitEnemyWithKnife _hitEnemyWithKnife = HitEnemyWithKnife.NOTHING;
        public enum ReloadingAk47{RELOADING, END}
        private ReloadingAk47 _reloadingAk47 = ReloadingAk47.END;
        public enum ReloadingPistol{RELOADING, END}
        private ReloadingPistol _reloadingPistol = ReloadingPistol.END;

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
            _parent = GameObject.FindWithTag("BulletsParrent");
            
            _photonView = GetComponent<PhotonView>();
            Enemy.StationBehavior._listPlayers.Add(gameObject);
            _ammunitionPistol = _ammunitionStartPistol;
            _ammunitionAk47 = _ammunitionStartAk47;
            
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
          //  if (!_photonView.IsMine) return;

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
                if (_reloadingAk47 == ReloadingAk47.END)
                {
                    _reloadingAk47 = ReloadingAk47.RELOADING;
                    StartCoroutine(AudioReloading(_audioclipReloadingAk47,_audioSourceReloadingAk47));
                }
            }
            
            if (_rdyToShotPistol == RdyToShotPistol.AMMUNITION)
            {
                if (_reloadingPistol == ReloadingPistol.END) 
                {
                    _reloadingPistol = ReloadingPistol.RELOADING;
                   StartCoroutine(AudioReloading(_audioclipReloadingPistol,_audioSourceReloadingPistol));
                }
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
          //  if (!_photonView.IsMine) return;
            
            Idle();
            Attack();
            AudioRuning();
        }

        public void Idle()
        {
            _currentState.Idle(ref _weapon);
        }

        public void Run()
        {
            _currentState.Run(_playerRb, _moveSpeed, ref _weapon);
        }

        //Звуковое сопровождение бега
        private void AudioRuning()
        {
            if (_movementJoystick.Horizontal!=0 && _movementJoystick.Vertical != 0)
            {
                if (!_audioSourceMoving.isPlaying)
                {
                    int id=0;
                    foreach (var audio in _audioclipMoving)
                    {
                        if (audio==_audioSourceMoving.clip)
                        {
                            break;
                        }
                        id++;
                    }

                    if (id == _audioclipMoving.Length - 1)
                    {
                        id = 0;
                    }
                    else id++;
                    _audioSourceMoving.clip = _audioclipMoving[id];
                    _audioSourceMoving.Play();
                }
            }
        }

        //Звуковое сопровождение атаки стрельды
        private void AudioFire(AudioClip[] audioClips, AudioSource audioSource)
        {
                    int id=0;
                    foreach (var audio in audioClips)
                    {
                        if (audio==audioSource.clip)
                        {
                            break;
                        }
                        id++;
                    }

                    if (id == audioClips.Length - 1)
                    {
                        id = 0;
                    }
                    else id++;
                    audioSource.clip = audioClips[id];
                    audioSource.Play();
        }
        //Звук перезарядки
        IEnumerator AudioReloading(AudioClip[] audioClips, AudioSource audioSource)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();

            while (true)
            {
                if (audioClips.Length==1&&!audioSource.isPlaying)
                {
                    CheckAudioRes(audioSource);
                    yield break;
                }
                else
                if (!audioSource.isPlaying)
                {
                    int id = 0;
                    foreach (var audio in audioClips)
                    {
                        if (audio == audioSource.clip)
                        {
                            break;
                        }
                        id++;
                    }
                    
                    if (id == audioClips.Length - 1)
                    {
                        CheckAudioRes(audioSource);
                        yield break;
                    }else id++;
                
                    audioSource.clip = audioClips[id];
                    audioSource.Play();
                }

                yield return null;
            }
        }
        //Проверка какой аудиоресурс был загружен в корутину
        private void CheckAudioRes(AudioSource audioSource)
        {
            if (audioSource == _audioSourceReloadingAk47)
            {
                _ammunitionAk47 = _ammunitionStartAk47;
                _rdyToShotAk47 = RdyToShotAk47.RDY;
                _reloadingAk47 = ReloadingAk47.END;
            }
            if (audioSource == _audioSourceReloadingPistol)
            {
                        
                _ammunitionPistol = _ammunitionStartPistol;
                _rdyToShotPistol = RdyToShotPistol.RDY;
                _reloadingPistol = ReloadingPistol.END;
                        
            }
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
                                    _hitEnemyWithKnife = HitEnemyWithKnife.ENEMY;
                                    enemies[i].GetComponent<EnemyHp>().TakeDamage(_weaponData.Dmg);
                                    break;
                                }
                                if (enemies[i].CompareTag("Wall"))
                                {
                                    _hitEnemyWithKnife = HitEnemyWithKnife.WALL;
                                    break;
                                }
                            }
                        }

                        if (_hitEnemyWithKnife==HitEnemyWithKnife.ENEMY)
                        {
                            _audioSourceAttackKnife.clip =
                                _audioClipAttackEnemy[new Random().Next(0, _audioClipAttackEnemy.Length - 1)];
                            _audioSourceAttackKnife.Play();
                        }
                        if (_hitEnemyWithKnife==HitEnemyWithKnife.WALL)
                        {
                            _audioSourceAttackKnife.clip =
                                _audioClipAttackWall[new Random().Next(0, _audioClipAttackWall.Length - 1)];
                            _audioSourceAttackKnife.Play();
                        }
                        if (_hitEnemyWithKnife==HitEnemyWithKnife.NOTHING)
                        {
                            _audioSourceAttackKnife.clip =
                                _audioClipAttackNothing[new Random().Next(0, _audioClipAttackNothing.Length - 1)];
                            _audioSourceAttackKnife.Play();
                        }

                        _hitEnemyWithKnife = HitEnemyWithKnife.NOTHING;
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
                                    AudioFire(_audioclipFireAk47,_audioSourceFireAk47);
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
                                    _ammunitionPistol--;
                                    AudioFire(_audioclipFirePistol,_audioSourceFirePistol);
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
                                    AudioFire(_audioclipFireAk47,_audioSourceFireAk47);
                                }
                                else if (_weapon == Weapon.PISTOL&& _rdyToShotPistol!= RdyToShotPistol.AMMUNITION)
                                {
                                    Instantiate(_prefabBulletPistol, _muzzlePos.transform.position,
                                        _playerModel.transform.rotation, _parent.transform);
                                    _ammunitionPistol--;
                                    AudioFire(_audioclipFirePistol,_audioSourceFirePistol);
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