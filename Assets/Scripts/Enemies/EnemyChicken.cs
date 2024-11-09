using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// La clase EnemyChicken hereda de Enemy y tiene comportamiento específico de seguimiento.
public class EnemyChicken : Enemy
{
    [Header("Chicken Details")] 
    [SerializeField] private float _aggroDuration; // Duración del estado de aggro.

    private float _aggroTimer; // Temporizador de aggro.
    private bool _canFlip = true; // Controla si puede girar o no.

    protected override void Update()
    {
        base.Update(); // Llama a la lógica de actualización base.

        _aggroTimer -= Time.deltaTime; // Disminuye el temporizador de aggro.

        if (isDead) return; // Si está muerto, sale del método.

        // Si detecta al jugador, habilita el movimiento y reinicia el temporizador de aggro.
        if (isPlayerDetected)
        {
            canMove = true;
            _aggroTimer = _aggroDuration;
        }

        // Si el temporizador de aggro se agota, detiene el movimiento.
        if (_aggroTimer < 0)
        {
            canMove = false;
        }

        HandleMovement(); // Controla el movimiento horizontal.

        // Si está en el suelo, maneja el cambio de dirección al encontrar un obstáculo.
        if (isGrounded)
            HandleTurnAround();
    }

    // Método para girar al enemigo si detecta una pared o ausencia de suelo.
    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip(); // Gira al enemigo.
            canMove = false;
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento.
        }
    }

    // Controla el movimiento horizontal del enemigo en dirección al jugador.
    private void HandleMovement()
    {
        if (!canMove)
        {
            return; // Si no puede moverse, termina el método.
        }

        float xValue = player.transform.position.x; // Obtiene la posición x del jugador.

        HandleFlip(xValue); // Controla la dirección en la que el enemigo mira.

        // Aplica el movimiento en la dirección actual.
        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y);
    }

    // Método Flip que redefine la lógica de volteo del enemigo.
    protected override void Flip()
    {
        base.Flip();
        _canFlip = true; // Habilita el volteo.
    }

    // Controla el volteo del enemigo basado en la posición x del jugador.
    protected override void HandleFlip(float xValue)
    {
        // Si el jugador está a la izquierda y el enemigo mira a la derecha, o viceversa, gira.
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            if (_canFlip)
            {
                _canFlip = false;
                Invoke(nameof(Flip), 0.3f); // Gira después de 0.3 segundos.
            }
        }
    }
}
