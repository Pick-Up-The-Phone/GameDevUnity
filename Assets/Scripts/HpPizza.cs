using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPizza : MonoBehaviour
{
    [SerializeField] private int _hpPoints;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player_Mover player = other.GetComponent<Player_Mover>();
        if (player != null)
        {
            player.AddHp(_hpPoints);
            Destroy(gameObject);
        }
    }
}
