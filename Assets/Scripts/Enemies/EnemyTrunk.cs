using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrunk : Enemy
{
    [Header("Trunk details")] 
    [SerializeField] private Enemy_Bullet _bulletPrefab; // Prefab del proyectil disparado por EnemyTrunk.
    [SerializeField] private Transform _gunPoint;       // Punto de origen desde donde se dispara el proyectil.
    [SerializeField] private float _bulletSpeed = 7;    // Velocidad del proyectil.
    [SerializeField] private float _attackCoolDown = 1.5f; // Tiempo de enfriamiento entre ataques.
    private float _lastTimeAttacked;                    // Registro del tiempo del último ataque.
    
    // Método Update, verifica si el enemigo puede atacar.
    protected override void Update()
    {
        base.Update(); // Llama al método Update de la clase padre.
        
        if (isDead) 
            return;  // Detiene ejecución si el enemigo está muerto.

        canMove = !isPlayerDetected; // Establece si puede moverse, dependiendo de si detecta al jugador o no.
        
        HandleMovement(); // Controla el movimiento horizontal.
        
        if (isGrounded)
            HandleTurnAround(); // Cambia de dirección si está en el suelo y frente a una pared.
        
        // Determina si ha pasado el tiempo suficiente para atacar de nuevo.
        bool canAttack = Time.time > _lastTimeAttacked + _attackCoolDown;
        
        // Si el jugador es detectado y puede atacar, inicia el ataque.
        if (isPlayerDetected && canAttack)
        {
            Attack();
        }
    }

    // Método para cambiar de dirección si no hay suelo al frente o si detecta una pared.
    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip(); // Cambia la dirección en la que mira el enemigo.
            idleTimer = idleDuration; // Espera antes de moverse de nuevo.
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento.
        }
    }

    // Controla el movimiento del enemigo.
    private void HandleMovement()
    {
        if (isPlayerDetected && !canMove)
        {
            return; // Si el jugador es detectado y no puede moverse, se detiene.
        }
        
        if (idleTimer > 0) // Si el temporizador de espera está activo, no se mueve.
        {
            return;
        }
        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y); // Aplica movimiento horizontal.
    }
    
    // Inicia la animación y registra el tiempo de ataque.
    private void Attack() 
    {
        _lastTimeAttacked = Time.time; // Actualiza el tiempo del último ataque.
        anim.SetTrigger("attack"); // Activa la animación de ataque.
    }

    // Método llamado desde un evento de animación (enemyTrunkAttack) para crear el proyectil.
    private void CreateBullet()
    {
        // Instancia el proyectil en la posición del arma.
        Enemy_Bullet newBullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.identity);
        
        if (facingDirection == 1)
        {
            newBullet.FlipSprite(); // Ajusta la orientación del proyectil según la dirección del enemigo.
        }
        
        Vector2 newBulletSpeed = new Vector2(_bulletSpeed * facingDirection, 0); // Asigna la velocidad en la dirección adecuada.
        newBullet.SetVelocity(newBulletSpeed); // Establece la velocidad en el proyectil.

        // Destruye el proyectil después de 10 segundos si no ha colisionado.
        Destroy(newBullet.gameObject, 10);
    }
}