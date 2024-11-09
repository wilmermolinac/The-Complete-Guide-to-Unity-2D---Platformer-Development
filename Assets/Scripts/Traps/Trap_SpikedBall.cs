using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_SpikedBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _spikeRb;
    [SerializeField] private float _pushForce;

    private void Start()
    {
        Vector2 pushVector = new Vector2(_pushForce, 0);
        
        _spikeRb.AddForce(pushVector, ForceMode2D.Impulse);
    }
}