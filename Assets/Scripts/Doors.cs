using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Doors : MonoBehaviour
{
    [SerializeField] private Sprite _openDoor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private Sprite _closedDoors;
    
    private bool _open;
        
    [SerializeField] private int _diamondsToEndLevel;
    [SerializeField] private int _levelToLoad;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _closedDoors = _spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player_Mover player = other.GetComponent<Player_Mover>();
        if (player != null && !_open && player.SumDiamonds >= _diamondsToEndLevel)
        {
            _spriteRenderer.sprite = _openDoor;
            _open = true;
            Debug.Log("Open");
            Invoke(nameof(LoadNextScene), 2f);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(_levelToLoad);
    }
}
