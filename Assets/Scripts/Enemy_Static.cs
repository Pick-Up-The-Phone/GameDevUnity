using UnityEngine;

public class Enemy_Static : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _damageDelay;
    
    private float _lastDamageTime;
    private Player_Mover _player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player_Mover player = other.GetComponent<Player_Mover>();
        if (player != null)
        {
            _player = player;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Player_Mover player = other.GetComponent<Player_Mover>();
        if(player == _player)
        {
            _player = null;
        }
    }
    
    private void Update()
    {
        if (_player != null && Time.time - _lastDamageTime > _damageDelay )
        {
            _lastDamageTime = Time.time;
            _player.TakeDamage(_damage);
        }
    }
}