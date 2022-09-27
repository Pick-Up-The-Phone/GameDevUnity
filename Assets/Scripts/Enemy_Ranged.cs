using UnityEngine;
using UnityEngine.UI;

public class Enemy_Ranged : MonoBehaviour
{
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _isitPlayer;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Rigidbody2D _fireball;
    [SerializeField] private float _fireballSpeed;
    [SerializeField] private bool _faceWay;
    [SerializeField] private int _maxHp;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private GameObject _enemySystem;
    
    private bool _canShoot;
    private int _currentHp;

    [Header("Animation")] [SerializeField] private Animator _animator;
    [SerializeField] private string _fireAnimationKey;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_attackRange, 1, 0));
    }
    
    private void ChangeHp(int hp)
    {
        _currentHp = hp;
        if (_currentHp <= 0)
        {
            Destroy(_enemySystem);
        }
        _hpBar.value = hp;
    }
    
    private void Start()
    {
        _hpBar.maxValue = _maxHp;
        ChangeHp(_maxHp);
    }
    private void FixedUpdate()
    {
        if (_canShoot)
        {
            return;
        }
        CheckCanShoot();
    }

    private void CheckCanShoot()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(_attackRange, 1), 0, _isitPlayer);
        if (player != null)
        {
            _canShoot = true;
            StartShoot(player.transform.position);
        }
        else _canShoot = false;
    }

    private void StartShoot(Vector2 playerPosition)
    {
        if (transform.position.x > playerPosition.x && _faceWay ||
            transform.position.x < playerPosition.x && _faceWay)
        {
            _faceWay = !_faceWay;
            transform.Rotate(0, 180, 0);
        }
        _animator.SetBool(_fireAnimationKey, true);
    }

    public void Shoot()
    {
        Rigidbody2D fireball = Instantiate(_fireball, _muzzle.position, Quaternion.identity);
        fireball.velocity = _fireballSpeed * transform.right;
        _animator.SetBool(_fireAnimationKey, false);
        Invoke(nameof(CheckCanShoot), 1f);
    }

    public void TakeDamage(int damage)
    {
        ChangeHp(_currentHp - damage);
    }
}



