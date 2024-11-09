using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que representa el Snail
public class EnemySnail : Enemy
{
    [Header("Snail Details")] 
    [SerializeField] private EnemySnailBody _snailPrefab; // Prefab del cuerpo que genera EnemySnail al morir.

    [SerializeField] private float _maxSpeed = 10; // Velocidad máxima que el caracol puede alcanzar.

    private bool _hasBody = true; // Indica si el caracol aún tiene cuerpo.

    protected override void Update()
    {
        base.Update(); // Llama al método Update de la clase base Enemy.

        if (isDead)
            return; // Si está muerto, detiene la ejecución de este método.

        HandleMovement(); // Controla el movimiento horizontal.

        if (isGrounded)
            HandleTurnAround(); // Cambia de dirección si está en el suelo y frente a una pared.
    }

    public override void Die()
    {
        if (_hasBody)
        {
            canMove = false; // Impide el movimiento si aún tiene cuerpo.
            _hasBody = false; // Indica que ya no tiene cuerpo.
            anim.SetTrigger("hit"); // Activa la animación de impacto.
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento.
            idleDuration = 0; // Establece la duración de inactividad en cero.
        }
        else if (!canMove && !_hasBody)
        {
            anim.SetTrigger("hit"); // Activa la animación de impacto.
            canMove = true; // Permite el movimiento nuevamente.
            moveSpeed = _maxSpeed; // Aumenta la velocidad de movimiento al máximo.
        }
        else
        {
            base.Die(); // Llama al método Die de la clase base si se cumplen las condiciones.
        }
    }

    // Método para cambiar de dirección.
    private void HandleTurnAround()
    {
        bool canFlipFromLedge = !isGroundInFrontDetected && _hasBody; // Comprueba si puede girar si detecta un borde.
        
        if (canFlipFromLedge || isWallDetected)
        {
            Flip(); // Cambia de dirección.
            idleTimer = idleDuration; // Espera antes de moverse de nuevo.
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

        if (!canMove)
        {
            return; // Detiene el movimiento si canMove es falso.
        }

        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y); // Aplica movimiento horizontal.
    }

    // Llamado en un evento de animación (enemySnailHit) para crear el cuerpo del caracol.
    private void CreateBody()
    {
        EnemySnailBody newBody = Instantiate(_snailPrefab, transform.position, Quaternion.identity); // Instancia el cuerpo en la posición actual.

        if (Random.Range(0, 100) < 50)
        {
            deathRotationDirection = deathRotationDirection * -1; // Cambia aleatoriamente la dirección de rotación.
        }

        newBody.SetupBody(deathImpactSpeed, deathImpactSpeed + deathRotationDirection, facingDirection); // Configura las propiedades del nuevo cuerpo.

        Destroy(newBody, 10); // Destruye el cuerpo después de 10 segundos.
    }

    protected override void Flip()
    {
        base.Flip(); // Llama a la función Flip de la clase base.

        if (!_hasBody)
        {
            anim.SetTrigger("wallHit"); // Activa la animación de impacto en la pared.
        }
    }
}