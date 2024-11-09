using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

// La clase 'Enemy' define el comportamiento general de un enemigo en el juego.
// Hereda de MonoBehaviour y se usa como base para clases específicas de enemigos.
public class Enemy : MonoBehaviour
{
    // Accede al componente SpriteRenderer en el mismo GameObject para manipular el sprite.
    private SpriteRenderer _sr => GetComponent<SpriteRenderer>();
    
    // Variables para el manejo de animaciones y física
    protected Animator anim; // Controlador de animaciones del enemigo.
    protected Rigidbody2D rb; // Referencia al componente Rigidbody2D para aplicar física.
    protected Collider2D[] colliders; // Declaramos un array protegido para almacenar los colliders

    protected Transform player; // Objeto jugador
    //[SerializeField] protected GameObject damageTrigger; // Objeto que activa el daño.

    [Header("General Info")] [SerializeField]
    protected float moveSpeed = 2f; // Velocidad de movimiento del enemigo.

    [SerializeField] protected float idleDuration = 1.5f; // Tiempo en espera antes de moverse.
    protected float idleTimer; // Temporizador para gestionar el tiempo de espera.
    protected bool canMove = true;
    
    [Header("Death details")] // Configuración para el comportamiento al morir.
    [SerializeField] protected float deathImpactSpeed = 5f; // Velocidad hacia arriba al morir.
    [SerializeField] protected float deathRotationSpeed = 150f; // Velocidad de rotación al morir.
    protected int deathRotationDirection = 1; // Dirección de rotación al morir.
    protected bool isDead; // Indica si el enemigo ha muerto.

    [Header("Basic Collision")] // Configuración de colisiones.
    [SerializeField]
    protected float groundCheckDistance = 1.1f; // Distancia para detectar el suelo.

    [SerializeField] protected float wallCheckDistance = 0.7f; // Distancia para detectar paredes.
    [SerializeField] protected LayerMask whatIsGround; // Define qué capas son consideradas "suelo".
    [SerializeField] protected Transform groundCheck; // Posición para revisar el suelo.
    [Space] [SerializeField] protected LayerMask whatIsPlayer; // Definine que capas se consideran "Player"
    [SerializeField] protected float playerDetectionDistance = 15f;
    protected bool isPlayerDetected;

    protected bool isGrounded; // Indica si el enemigo está tocando el suelo.
    protected bool isGroundInFrontDetected; // Indica si hay suelo delante del enemigo.
    protected bool isWallDetected; // Indica si hay una pared delante del enemigo.

    // Variables para la dirección y orientación del enemigo
    protected int facingDirection = -1; // Dirección en la que se mueve: -1 o 1.
    protected bool facingRight = false; // Indica si el enemigo mira a la derecha.

    // Método Awake inicializa componentes básicos. 
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>(); // Obtiene el Animator del objeto.
        rb = GetComponent<Rigidbody2D>(); // Obtiene el Rigidbody2D para manejar física.
        // Obtenemos todos los componentes de tipo Collider2D en los hijos del GameObject actual
        colliders = GetComponentsInChildren<Collider2D>();
    }

    // Método Start virtual para inicialización, permitiendo sobrecarga en clases derivadas.
    protected virtual void Start()
    {
        // Llama a UpdatePlayerRef cada segundo para actualizar la referencia al jugador.
        InvokeRepeating(nameof(UpdatePlayerRef), 0, 1);

        // Alinea el sprite en la dirección correcta al iniciar.
        if (_sr.flipX && !facingRight)
        {
            _sr.flipX = false;
            Flip();
        }
    }

    // Actualiza la referencia del jugador, obteniéndola desde el GameManager si es necesario.
    private void UpdatePlayerRef()
    {
        if (player == null)
        {
            player = GameManager.instance.player.transform;
        }
    }

    // Método Update es llamado en cada frame.
    protected virtual void Update()
    {
        HandleAnimation();
        
        HandleCollisions();
        
        idleTimer -= Time.deltaTime; // Resta el tiempo transcurrido al temporizador de espera.

        if (isDead)
        {
            HandleDeathRotation(); // Aplica rotación de muerte si el enemigo ha muerto.
        }
    }

    // Método Die se ejecuta cuando el enemigo muere.
    public virtual void Die()
    {
        // Recorremos cada collider en el array de colliders
        foreach (Collider2D collider in colliders)
        {
            // Desactivamos cada collider para evitar nuevas colisiones
            collider.enabled = false;
        }

        //damageTrigger.SetActive(false); // Desactiva el objeto que aplica daño.
        anim.SetTrigger("hit"); // Activa la animación de muerte.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpactSpeed); // Aplica impulso hacia arriba.
        isDead = true; // Marca al enemigo como muerto.

        // Aleatoriamente selecciona una dirección de rotación al morir.
        if (Random.Range(0, 100) < 50)
        {
            deathRotationDirection *= -1;
        }
        
        Destroy(gameObject, 10);
    }

    // Método para aplicar rotación al enemigo mientras cae muerto.
    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationDirection * deathRotationSpeed) * Time.deltaTime);
    }

    // Controla si el enemigo debe girarse al cambiar de dirección.
    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    // Cambia la orientación del enemigo.
    protected virtual void Flip()
    {
        facingRight = !facingRight; // Invierte la variable de orientación.
        facingDirection *= -1; // Cambia la dirección de movimiento.
        transform.Rotate(0, 180, 0); // Rota el objeto 180 grados en el eje Y.
    }

    // Método de utilidad que invierte la dirección predeterminada del sprite.
    [ContextMenu("Change Facing Direction")]
    public void FlipDefaultFacingDirection()
    {
        _sr.flipX = !_sr.flipX;
    }

    // Método virtual que maneja las animaciones. Puede ser sobrescrito en clases derivadas.
    protected virtual void HandleAnimation()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x); // Actualiza la animación con la velocidad en x.
    }  

    // Verifica colisiones del enemigo.
    protected virtual void HandleCollisions()
    {
        // Detectamos el suelo
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        // Detectamos si hay suelo delante del GameObject
        isGroundInFrontDetected =
            Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        // Detectamos si hay una pared
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance,
            whatIsGround);

        // Detectamos si hay un jugador
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection,
            playerDetectionDistance, whatIsPlayer);
        
    }

    // Visualización de colisiones en el editor.
    protected virtual void OnDrawGizmos()
    {
        // visualizacion del colisionador el suelo
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position,
            new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        // visualizacion del colisionador para paredes
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x + (facingDirection * wallCheckDistance), transform.position.y));
        
        // Dibuja una línea en el editor, que muestra el alcance de detección del jugador.
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x + (playerDetectionDistance * facingDirection), transform.position.y));
    }
}