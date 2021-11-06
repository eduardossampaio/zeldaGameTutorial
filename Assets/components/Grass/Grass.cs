using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private ParticleSystem fxHit;
    private bool isCutted = false;
    void GetHit(int amount)
    {
        if (!isCutted)
        {
            transform.localScale = Vector3.one;
            fxHit.Emit(10);
            isCutted = true;
        }
    }
}
