//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using NewBotSystem;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
	public class Projectile : MonoBehaviour
	{
        //public UnityEvent<float, Transform, Vector3, Vector3> OnCollided = new UnityEvent<float, Transform, Vector3, Vector3>();
        //public UnityEvent<Vector3, Vector3, string> OnCollidedEffect = new UnityEvent<Vector3, Vector3, string>();
        //public UnityEvent<float, GameObject, GameObject> OnCollidedWithDamageble= new UnityEvent<float, GameObject, GameObject>();
        [SerializeField]
        private LayerMask _playerMask;
        [SerializeField]
        private GameObject _sender;
        [SerializeField]
        private string _senderName;
        [SerializeField]
        private string _externalTag;
        [SerializeField]
        private bool _isDrill;
        private string _tag;

        private PlayerNetworkResolver _resolver;

        private float _damage;
		private int _impacts;

		private Transform _inoffTarget;
		private float _lifeTime = 60f;

		private float _startSpeed;
		private float _deltaTime = 0;
		private float _time = 0;
		private float _nexttime = 0;
		private Vector3 _direction;
		private Vector3 _origin;

		private Vector3 _position;
		private Vector3 _nextPosition;

		private Vector3 _bulletDirection;
		private float _bulletDeltaPath = 2;

		private bool _bulletExist = false;
		private bool _bulletInited = false;


        public virtual void Init(float damage, float speed, GameObject sender, PlayerNetworkResolver resolver)
		{
			//Debug.Log($"Projectile {damage}");
            _deltaTime = Time.fixedDeltaTime;
            _direction = transform.forward;
			_bulletDirection = _direction;
			_damage = damage;
			_startSpeed = speed;
			_origin = transform.position;
			_bulletExist = true;
			_bulletInited = true;
            _sender = sender;
            _resolver = resolver;
            _senderName = _resolver.gameObject.name;
            //Ray ray = Camera.main.ViewportPointToRay(new Vector2(.5f, .5f));

            //var raycasts = Physics.RaycastAll(Camera.main.ViewportToScreenPoint(new Vector2(.5f, .5f)), _bulletDirection);

            Vector3 pos = Camera.main.transform.position;
			Vector3 dir = Camera.main.transform.forward;

            LifeIsLife();

            CalculateTrajectory();

        }

        public virtual void Start()
		{
        }

        public virtual void FixedUpdate()
		{
            if(_isDrill)
            {
                _bulletExist = false;
                DestroyBullet();
            }
            else
            {
                if (_bulletInited)
                {
                    _time += _deltaTime;
                    _nexttime = _time + _deltaTime;

                    _position = _origin + _startSpeed * _direction * _time + new Vector3(0, -9.8f, 0) * _time * _time / 2f;
                    _nextPosition = _origin + _startSpeed * _direction * _nexttime + new Vector3(0, -9.8f, 0) * _nexttime * _nexttime / 2f;

                    transform.position = _position;
                    _bulletDirection = (_nextPosition - _position).normalized;
                    _bulletDeltaPath = Vector3.Distance(_nextPosition, _position);

                    if (_bulletExist == false)
                    {
                        DestroyBullet();
                    }

                    ////Debug.Log(_time+" "+ _lifeTime);
                    if (_time > _lifeTime)
                    {
                        _bulletExist = false;
                    }

                    LifeIsLife();

                }
                //Debug.Break();
            }
        }

        public virtual void LifeIsLife()
        {

            if (Physics.Raycast(transform.position, _bulletDirection, out RaycastHit hit, _bulletDeltaPath, _playerMask))
            {
                if (hit.collider.name.Equals(_senderName) == true)
                {
                    //Debug.Log("Self Collided");
                    //Debug.DrawRay(transform.position, _bulletDirection * _bulletDeltaPath, Color.white, 100);
                }
                else
                {
                    //Debug.DrawRay(transform.position, _bulletDirection * _bulletDeltaPath, Color.red, 100);
                    if (_bulletExist)
                    {
                        _resolver.OnBulletCollidedOnServer();

                        if (string.IsNullOrEmpty(_externalTag))
                        {
                            _resolver.OnEffectOnServer(hit.point, hit.normal, hit.transform.gameObject.tag);
                            _tag = hit.transform.gameObject.tag;
                        }
                        else
                        {
                            if (_externalTag.Equals("Drill"))
                            {
                                _resolver.OnEffectOnServer(hit.point, hit.normal, _externalTag);
                                _tag = _externalTag;
                            }
                            else
                            {
                                _resolver.OnEffectOnServer(hit.point, hit.normal, _externalTag);
                                _tag = _externalTag;
                                //GameWorld.Instance.IntentionMakeExplosion(hit.point, hit.normal);
                                //_resolver.AddMoreRandomBullet();
                            }
                        }
                        
                        if (hit.transform.gameObject.TryGetComponent(out Damageble damageble))
                        {
                            //Debug.Log("Вот тут нас будут ебать читеры! \nЧитеры привет! Это ОЧКО осталось непрекрытым специально для вас... Уебы... \nКЧАУ!");
                            damageble.ReceiveDamage(_damage, _resolver.PlayerID);
                        }
                        
                        if (hit.transform.gameObject.TryGetComponent(out BotDamageble botDamageble))
                        {
                            botDamageble.ReceiveDamage(_damage, _resolver.PlayerID);
                        }
                        /*
                        if (hit.transform.gameObject.TryGetComponent(out ChankRenderer renderer))
                        {
                            _resolver.AddRandomBullet();
                            renderer.IntentionСhangeBlock(hit.point, hit.normal, _damage);
                        }*/
                    }

                    _bulletExist = false;
                    //HitHandler(hit.collider.transform);
                }
            }
            else
            {
                //Debug.DrawRay(_position, _bulletDirection * _bulletDeltaPath, Color.white, 100);
            }

        }

        public virtual void DestroyBullet()
		{
			//Debug.Log("bullet was destoyed");
			Destroy(this.gameObject);
		}

        public virtual void OnDestroy()
		{
			//Debug.Log($"Bullet is Destroyed {_tag} {_damage} impacts {_impacts} ");
        }

        public virtual void CalculateTrajectory()
        {
            float time = 0;
            float nexttime = time + _deltaTime;

            Vector3 position = _origin + _startSpeed * _direction * time + new Vector3(0, -9.8f, 0) * time * time / 2f;
            Vector3 nextPosition = _origin + _startSpeed * _direction * nexttime + new Vector3(0, -9.8f, 0) * nexttime * nexttime / 2f;

            Vector3 bulletDirection = (nextPosition - position).normalized;

            int iterationCount = (int)(_lifeTime / _deltaTime);

            Debug.DrawLine(position, nextPosition, Color.red);

            for (int i = 0; i < iterationCount; i++)
            {
                time += _deltaTime;
                nexttime = time + _deltaTime;

                position = _origin + _startSpeed * _direction * time + new Vector3(0, -9.8f, 0) * time * time / 2f;
                nextPosition = _origin + _startSpeed * _direction * nexttime + new Vector3(0, -9.8f, 0) * nexttime * nexttime / 2f;
            }

            _resolver.OnPjectileSpawnedOnServer(nextPosition, _deltaTime);
        }
    }
}