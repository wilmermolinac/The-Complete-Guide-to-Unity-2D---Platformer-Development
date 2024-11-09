using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que representa el comportamiento de un proyectil disparado por EnemyTrunk.
public class Enemy_Bullet : MonoBehaviour
{
    private Rigidbody2D _rb; // Componente Rigidbody2D del proyectil.
    private SpriteRenderer _sr; // Componente SpriteRenderer del proyectil.

    [SerializeField] private string _playerLayerName = "Player"; // Nombre de la capa del jugador.
    [SerializeField] private string _groundLayerName = "Ground"; // Nombre de la capa del suelo.

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); // Obtiene el Rigidbody2D para controlar la velocidad.
        _sr = GetComponent<SpriteRenderer>(); // Obtiene el SpriteRenderer para controlar la imagen.
    }

    // Método público para establecer la velocidad del proyectil.
    public void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity; // Establece la velocidad del proyectil.
    }

    // Invierte el sprite para que el proyectil se ajuste a la dirección del enemigo.
    public void FlipSprite()
    {
        _sr.flipX = !_sr.flipX; // Invierte el eje X del sprite.
    }

    // Detecta colisiones y maneja los efectos al colisionar con el jugador o el suelo.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el proyectil colisiona con el jugador, aplica retroceso y se destruye.
        if (collision.gameObject.layer == LayerMask.NameToLayer(_playerLayerName))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.Knockback(transform.position.x); // Aplica el retroceso al jugador.
            Destroy(gameObject); // Destruye el proyectil.
        }

        // Si colisiona con el suelo, se destruye después de un breve retardo.
        if (collision.gameObject.layer == LayerMask.NameToLayer(_groundLayerName))
        {
            Destroy(gameObject, 0.05f); // Se destruye rápidamente tras chocar con el suelo.
        }
    }
}