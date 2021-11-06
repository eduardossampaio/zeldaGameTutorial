using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int itensCollected;
    private GameManager _gameManager;
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    public int GetItensCollected()
    {
        return itensCollected;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Collectable"))
        {
            Destroy(other.gameObject);
            itensCollected++;
            _gameManager.AddGems(1);
        }
    }
}
