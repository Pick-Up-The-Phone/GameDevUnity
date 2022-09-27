using UnityEngine;
using UnityEngine.Windows.WebCam;

public class Enemy_Dynamic : MonoBehaviour
{
    [SerializeField] private float _patrolRange;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
    [SerializeField] private bool _faceWay;
    [SerializeField] private int _damage;
    [SerializeField] private float _pushPower;
    private Vector2 _startPostion;
    private float _lastAttackTime;

    private Vector2 _drawPostion
    {
        get
        {
            if (_startPostion == Vector2.zero)
                return transform.position;
            else
                return _startPostion;
        }
    }
    private void Start()
    {
        _startPostion = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_drawPostion, new Vector3(_patrolRange*2,1, 0));
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.right * _speed;
    }

    private void Update()
    {
        float xPos = transform.position.x;
        if (xPos > _startPostion.x + _patrolRange && _faceWay)
        {
            Flip();
        }
        else if (xPos < _startPostion.x - _patrolRange && !_faceWay)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _faceWay = !_faceWay;
        transform.Rotate(0,180,0);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Player_Mover player = other.collider.GetComponent<Player_Mover>();
        if (player != null  && Time.time - _lastAttackTime > 0.2f)
        {
            _lastAttackTime = Time.time;
            player.TakeDamage(_damage, _pushPower, transform.position.x);
        }
    }
}