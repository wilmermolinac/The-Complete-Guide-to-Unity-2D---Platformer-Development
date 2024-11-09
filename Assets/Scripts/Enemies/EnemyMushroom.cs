using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Subclase de Enemy que define el comportamiento del enemigo 'EnemyMushroom'.
public class EnemyMushroom : Enemy
{
    protected override void Update()
    {
        base.Update(); // Llama al método Update de la clase padre.

        if (isDead)
            return;  // Detiene ejecución si el enemigo está muerto.

        HandleMovement();                  // Controla el movimiento horizontal.

        if (isGrounded)
            HandleTurnAround(); // Cambia de dirección si está en el suelo y frente a una pared.
    }

    // Método para cambiar de dirección.
    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();                 // Cambia dirección.
            idleTimer = idleDuration;  // Espera antes de moverse de nuevo.
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento.
        }
    }

    // Controla el movimiento del enemigo.
    private void HandleMovement()
    {
        if (idleTimer > 0) // Si el temporizador de espera está activo, no se mueve.
        {
            return;
        }
        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y);  // Aplica movimiento horizontal.
    }
    
}