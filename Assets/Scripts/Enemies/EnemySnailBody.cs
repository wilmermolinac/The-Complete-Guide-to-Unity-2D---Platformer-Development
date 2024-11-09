using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que representa el cuerpo que deja el EnemySnail al morir.
public class EnemySnailBody : MonoBehaviour
{
    private Rigidbody2D _rb; // Referencia al componente Rigidbody2D del cuerpo.
    private SpriteRenderer _sr; // Referencia al componente SpriteRenderer del cuerpo.
    private float _zRotation; // Almacena la rotación en el eje Z.

    // Configura las propiedades iniciales del cuerpo cuando se crea. 
    public void SetupBody(float yVelocity, float zRotation, int facingDirection)
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _sr.flipX = facingDirection == 1; // Ajusta el sprite según la dirección en que miraba EnemySnail.
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, yVelocity); // Asigna la velocidad en el eje Y.
        _zRotation = zRotation; // Asigna la rotación en el eje Z.
        
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        transform.Rotate(0, 0, _zRotation * Time.deltaTime); // Rota el cuerpo continuamente en el eje Z.
    }
}