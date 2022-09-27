using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float _lifetime;
    [SerializeField] private int _damage;
    [SerializeField] private Rigidbody2D _rigidbody;
    
    void Start()
    {
        Invoke(nameof(gameObject), _lifetime);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player_Mover player = other.GetComponent<Player_Mover>();
        if (player != null)
        {
            player.TakeDamage(_damage);
        }
        Destroy();
    }
}
