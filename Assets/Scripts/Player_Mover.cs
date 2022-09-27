using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

[RequireComponent(typeof(Rigidbody2D))]

public class Player_Mover : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _groundChecking;
    [SerializeField] private float _groundCheckingRadius;
    [SerializeField] private LayerMask _isitGround;
    [SerializeField] private LayerMask _isitCell;
    [SerializeField] private Collider2D _headCollider;
    [SerializeField] private float _headCheckerRadius;
    [SerializeField] private Transform _headChecker;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _runAnimatorKey;
    [SerializeField] private string _jumpAnimatorKey;
    [SerializeField] private string _crouchAnimatorKey;
    [SerializeField] private string _hitAnimatorKey;
    [SerializeField] private string _attackAnimatorKey;
    [SerializeField] private string _throwAnimatorKey;

    [Header("UI")] 
    [SerializeField] private TMP_Text _countDiamondsText;
    [SerializeField] private int _maxHp;
    [SerializeField] private Slider _hpBar;

    [Header("Attack")] 
    [SerializeField] private int _swordDamage;
    [SerializeField] private Transform _swordAttackPoint;
    [SerializeField] private float _swordAttackRadius;
    [SerializeField] private LayerMask _isitEnemy;
    [SerializeField] private int _throwDamage;
    [SerializeField] private Transform _throwAttackPoint;
    [SerializeField] private float _throwLength;
    [SerializeField] private LineRenderer _throwLine;
    
    private bool _faceWay;
    
    private float _horizontalDirection;
    private float _verticalDirection;
    private bool _jump;
    private bool _crawl;
    private int _sumDiamonds;
    private int _currentHp;
    private bool _needToAttack;
    private bool _needToThrow;
    
    private float _lastPushTime;
    public int SumDiamonds 
    {
        get => _sumDiamonds;
        set
        {
            _sumDiamonds = value;
            _countDiamondsText.text = value.ToString();
        }
    }

    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (value > _maxHp) value = _maxHp;
            _currentHp = value;
            _hpBar.value = value;
        }
    }

    public bool CanClimb { private get; set; }
    private void Start()
    {
        SumDiamonds = 0;
        _hpBar.maxValue = _maxHp;
        CurrentHp = _maxHp;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            _needToAttack = true;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _needToThrow = true;
        }
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _verticalDirection = Input.GetAxisRaw("Vertical");
        
        _animator.SetFloat(_runAnimatorKey, Mathf.Abs(_horizontalDirection));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
        }

        if (_horizontalDirection > 0 && _faceWay)
        {
            Flip();
        }
        else if (_horizontalDirection < 0 && !_faceWay)
        {
            Flip();
        }

        _crawl = Input.GetKey(KeyCode.C);

    }
    
    private void Flip()
    {
        _faceWay = !_faceWay;
        transform.Rotate(0, 180, 0);
    }

    private void FixedUpdate()
    {
        bool canJump = Physics2D.OverlapCircle(_groundChecking.position, _groundCheckingRadius, _isitGround);
        if (_animator.GetBool(_hitAnimatorKey))
        {
            if (Time.time - _lastPushTime > 0.05f && canJump)
            {
                _animator.SetBool(_hitAnimatorKey, false);
            }
            _needToAttack = false;
            _needToThrow = false;
            return;
        }

        if (CanClimb)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _verticalDirection * _speed);
            _rigidbody.gravityScale = 0;
        }
        else _rigidbody.gravityScale = 1;
        bool canStand = !Physics2D.OverlapCircle(_headChecker.position, _headCheckerRadius, _isitCell);
        
        
        _headCollider.enabled = !_crawl && canStand;

        if (_jump && canJump)
        {
            _jump = false;
            _rigidbody.AddForce(Vector2.up * _jumpForce);
        }

        _animator.SetBool(_jumpAnimatorKey, !canJump);
        _animator.SetBool(_crouchAnimatorKey, !(!_crawl && canStand));
        _rigidbody.velocity = new Vector2(_horizontalDirection * _speed, _rigidbody.velocity.y);
        
        if (!_headCollider.enabled)
        {
            _needToAttack = false;
            _needToThrow = false;
            return;
        }
        
        if (_needToAttack)
        {
            StartToAttack();
        }

        if (_needToThrow)
        {
            StartThrow();
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecking.position, _groundCheckingRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_headChecker.position, _headCheckerRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_swordAttackPoint.position, new Vector3(_swordAttackRadius, _swordAttackRadius, 0));
    }

    public void AddHp (int _hpPoints)
    {
        int missingHp = _maxHp - CurrentHp;
        int pointsToAdd = missingHp > _hpPoints ? _hpPoints : missingHp;
        StartCoroutine(RestoreHp(pointsToAdd));
    }
    
    private void StartToAttack()
    {
        if (_animator.GetBool(_attackAnimatorKey))
        {
            return;
        }
        _animator.SetBool(_attackAnimatorKey, true);
    }

    public void Attack()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(_swordAttackPoint.position, new Vector2(_swordAttackRadius, _swordAttackRadius), _isitEnemy);
        foreach (var target in targets)
        {
            Enemy_Ranged rangedEnemy = target.GetComponent<Enemy_Ranged>();
            if (rangedEnemy != null)
            {
                rangedEnemy.TakeDamage(_swordDamage);
            }
        }
        _animator.SetBool(_attackAnimatorKey, false);
        _needToAttack = false;
    }

    private void StartThrow()
    {
        if (_animator.GetBool(_throwAnimatorKey))
        {
            return;
        }
        _animator.SetBool(_throwAnimatorKey, true);
    }

    private void Throw()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(_throwAttackPoint.position, transform.right, _throwLength, _isitEnemy);
        foreach (var hit in hits)
        {
            Enemy_Ranged target = hit.collider.GetComponent<Enemy_Ranged>();
            if (target != null)
            {
                target.TakeDamage(_throwDamage);
            }
        }
        _throwLine.SetPosition(0, _throwAttackPoint.position);
        _throwLine.SetPosition(1, _throwAttackPoint.position + transform.right * _throwLength);
        _throwLine.enabled = true;
        _animator.SetBool(_throwAnimatorKey, false);
        _needToThrow = false;
        Invoke(nameof(DisableThrow), 0.25f);
    }
    

    private void DisableThrow()
    {
        _throwLine.enabled = false;
    }

    private IEnumerator RestoreHp(int pointsToAdd)
    {
        while (pointsToAdd != 0)
        {
            pointsToAdd--;
            CurrentHp++;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public int counterDiamonds(int _diamonds = 1)
    {
        return SumDiamonds += _diamonds;
    }

    public void TakeDamage(int damage, float pushPower = 0, float posX = 0)
    {
        if (_animator.GetBool(_hitAnimatorKey))
        {
            return;
        }
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            gameObject.SetActive(false);
            Invoke(nameof(ReloadScene), 1f);;
        }

        if (pushPower != 0 && Time.time - _lastPushTime > 0.5f)
        {
            _lastPushTime = Time.time;
            int direction = posX > transform.position.x ? -1 : 1;
            _rigidbody.AddForce(new Vector2(direction * pushPower/3, pushPower));
            _animator.SetBool(_hitAnimatorKey, true);
        }
    }
    
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
