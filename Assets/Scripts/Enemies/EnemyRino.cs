using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// La clase EnemyRino hereda de Enemy, extendiendo sus capacidades básicas para crear un enemigo con habilidades específicas de carga.
public class EnemyRino : Enemy
{
    // Variable privada que almacena la velocidad inicial del Rino.
    private float _defaultSpeed;

    // Configuraciones específicas del enemigo tipo Rino.
    [Header("Rino Details")]
    [SerializeField] private float _maxSpeed;        // Define la velocidad máxima que puede alcanzar el Rino.
    [SerializeField] private float _speedUpRate = 0.6f; // Define la tasa de aceleración durante la carga.
    [SerializeField] private Vector2 _impactPower;   // Define la fuerza del impacto que el Rino sufre al chocar con una pared.
    
    // El método Start se ejecuta una vez cuando el objeto es instanciado.
    protected override void Start()
    {
        // Llama al método Start de la clase base (Enemy) para inicializar cualquier configuración general.
        base.Start();

        // Inicialmente, el Rino no puede moverse hasta que detecte al jugador.
        canMove = false;
        
        // Guarda la velocidad inicial del Rino en _defaultSpeed para futuras referencias.
        _defaultSpeed = moveSpeed;

    }

    // El método Update se llama en cada cuadro (frame) y ejecuta la lógica de actualización.
    protected override void Update()
    {
        // Llama a Update de la clase base (Enemy) para mantener su comportamiento general.
        base.Update();

        // Llama al método HandleCharge para manejar la lógica de carga del Rino.
        HandleCharge();
    }

    // Controla la lógica de carga del Rino, gestionando movimiento y detección de obstáculos.
    private void HandleCharge()
    {
        // Si el Rino no puede moverse, no ejecuta la lógica de carga.
        if (!canMove)
        {
            return;
        }

        // Controla el incremento de velocidad del Rino mientras está en carga.
        HandleSpeedMovement();

        // Si no se detecta suelo frente al Rino, se gira para evitar caer.
        if (!isGroundInFrontDetected)
        {
            TurnAround();
        }
        
        // Si detecta una pared, ejecuta la lógica de choque contra la pared.
        if (isWallDetected)
        {
            WallHit();
        }
    }

    // Incrementa la velocidad del Rino de acuerdo a la tasa de aceleración hasta alcanzar la velocidad máxima.
    private void HandleSpeedMovement()
    {
        // Aumenta la velocidad del Rino en función de _speedUpRate multiplicada por el tiempo transcurrido.
        moveSpeed = moveSpeed + (Time.deltaTime * _speedUpRate);
        
        // Restringe la velocidad del Rino para que no supere _maxSpeed.
        if (moveSpeed >= _maxSpeed)
            _maxSpeed = moveSpeed;
        
        // Establece la velocidad en el Rigidbody2D para mover al Rino en la dirección actual.
        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y);
    }

    // Método para girar al Rino cuando no detecta suelo adelante.
    private void TurnAround()
    {
        SpeedReset();         // Restablece la velocidad a su valor predeterminado.
        canMove = false;      // Desactiva el movimiento temporalmente.
        rb.linearVelocity = Vector2.zero; // Detiene completamente al Rino.
        Flip();               // Invierte la dirección en la que mira el Rino.
    }

    // Controla el comportamiento del Rino al chocar contra una pared.
    private void WallHit() 
    {
        canMove = false;      // Detiene el movimiento del Rino.
        SpeedReset();         // Restablece la velocidad a su valor inicial.

        // Activa la animación de "choque contra pared".
        anim.SetBool("hitWall", true);

        // Aplica una fuerza de retroceso, usando _impactPower en la dirección opuesta al choque.
        rb.linearVelocity = new Vector2(_impactPower.x * -facingDirection, _impactPower.y);
    }

    // Restaura la velocidad del Rino a su velocidad inicial predeterminada.
    private void SpeedReset()
    {
        moveSpeed = _defaultSpeed;
    }

    // Método llamado cuando la animación de carga termina, reseteando al Rino.
    private void ChargeIsOver()
    {
        // Desactiva la animación de "choque" en el Animator.
        anim.SetBool("hitWall", false);

        // Llama al método Flip después de 1 segundo, para que el Rino esté listo para cargar de nuevo.
        Invoke(nameof(Flip), 1);
    }

    // Maneja las colisiones específicas del Rino, además de las colisiones generales de 'Enemy'.
    protected override void HandleCollisions()
    {
        // Llama a la lógica de colisiones de la clase base 'Enemy'.
        base.HandleCollisions();

        // Si el Rino detecta al jugador, activa el movimiento de carga.
        if (isPlayerDetected)
        {
            canMove = true;
        }
    }
    
}
