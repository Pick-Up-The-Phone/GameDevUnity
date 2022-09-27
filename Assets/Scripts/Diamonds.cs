using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Diamonds : MonoBehaviour
{
    [SerializeField] private int _diamonds;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player_Mover player = other.GetComponent<Player_Mover>();
        if (player != null)
        {
            player.SumDiamonds = player.counterDiamonds(_diamonds);
            
            Destroy(gameObject);
        }
    }
    
}
