using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private DifficultyType _gameDifficulty;
    private GameManager _gameManager;
    
    private Rigidbody2D _rb; // Referencia al componente Rigidbody2D para controlar la física

    private Animator _anim; // Referencia al componente Animator para controlar animaciones

    // Variable privada que almacena una referencia al componente CapsuleCollider2D del jugador.
    private CapsuleCollider2D _collider;

    // Variables relacionadas con el movimiento
    [Header("Movement")] [SerializeField] private float _moveSpeed; // Velocidad de movimiento del jugador
    [SerializeField] private float _jumpForce; // Fuerza del salto
    [SerializeField] private float _doubleJumpForce; // Fuerza del doble salto

    // Variables para el buffer y el salto coyote (permitir salto poco después de salir del suelo)
    [Header("Buffer & Coyote Jump")] 
    [SerializeField] private float
        _bufferJumpWindow =
            0.25f; // Tiempo durante el cual se puede realizar un salto después de presionar el botón de salto (buffer)

    [SerializeField] private float
        _coyoteJumpWindow = 0.5f; // Tiempo que permite realizar un salto después de caer del borde (salto coyote)

    // Variables para interacciones con las paredes (como el salto en pared)
    [Header("Wall Interactions")] [SerializeField]
    private float _wallJumpDuration = 0.6f; // Duración durante la cual se permite el salto en pared

    [SerializeField] private Vector2 _wallJumpForce; // Fuerza del salto en pared

    // Variables para la retroalimentación al ser golpeado (knockback)
    [Header("Knockback")] [SerializeField] private float _knockbackDuration = 1; // Duración del knockback
    [SerializeField] private Vector2 _knockbackPower; // Fuerza del knockback

    // Variables para la detección de colisiones
    [Header("Collision")] [SerializeField] private float _groundCheckDistance; // Distancia para detectar el suelo
    [SerializeField] private float _wallCheckDistance; // Distancia para detectar paredes
    [SerializeField] private LayerMask _whatIsGround; // Qué capas son consideradas suelo

    [Space] [SerializeField]
    private Transform _enemyCheck; // Transform que define el punto desde donde se detectan enemigos.

    [SerializeField] private float _enemyCheckRadius; // Radio de detección de enemigos alrededor del punto definido.

    [SerializeField]
    private LayerMask _whatIsEnemy; // Máscara de capa para especificar qué objetos son considerados enemigos

    // Este bloque se refiere a las opciones visuales del jugador.
    // El encabezado "Player Visuals" aparecerá en el inspector para organizar las opciones visuales.
    [Header("Player Visuals")]

    // Conjunto de controladores de animación que permiten cambiar las animaciones del jugador
    // en función de diferentes skins. Estos controladores se pueden asignar desde el inspector.
    [SerializeField] private AnimatorOverrideController[] _animators;

    // Variable que almacena el prefab del efecto visual que se mostrará cuando el jugador muera.
    // El prefab debe asignarse arrastrándolo al inspector desde la carpeta de Prefabs.
    // Este efecto se activa cuando el jugador pierde todas sus vidas o cuando ocurre una muerte en el juego.
    [SerializeField] private GameObject _playerDeath_vfx;

    // ID que representa la skin actual del jugador. 
    // Inicialmente está en cero, pero puede ser modificado para cambiar el aspecto visual del jugador.
    [SerializeField] private int skinId = 0;


    private float _bufferJumpActivated = -1; // Tiempo de activación del salto buffer
    private bool _canDoubleJump; // Si el jugador puede realizar un doble salto
    private float _coyoteJumpActivated = -1; // Tiempo de activación del salto coyote
    private int _facingDir = 1; // Dirección hacia la que mira el jugador (1 = derecha, -1 = izquierda)
    private bool _facingRight = true; // Si el jugador está mirando a la derecha
    private bool _isAirborne; // Si el jugador está en el aire
    private bool _isGrounded; // Si el jugador está en el suelo
    private bool _isKnocked; // Si el jugador ha sido golpeado
    private bool _isPressingHorizontalButtons = false; // Si se están presionando botones de movimiento horizontal
    private bool _isRunning; // Si el jugador está corriendo
    private bool _isWallDetected; // Si se ha detectado una pared
    private bool _isWallJumping; // Si el jugador está realizando un salto en pared
    private float _xInput; // Entrada del jugador en el eje horizontal
    private float _yInput; // Entrada del jugador en el eje vertical

    // Almacena el valor original de la gravedad del Rigidbody2D, para restaurarlo más tarde si es necesario.
    private float _defaultGravityScale;

    // Variable booleana que indica si el jugador puede ser controlado. Inicialmente es false, por lo que no puede ser controlado.
    private bool _canBeControlled = false;


    private void Awake()
    {
        // Este método se llama cuando el objeto se inicializa por primera vez.
        Debug.Log("Awake was called!");

        // Obtiene el componente Rigidbody2D del jugador, que gestiona la física 2D del cuerpo rígido.
        _rb = GetComponent<Rigidbody2D>();

        // Obtiene el componente CapsuleCollider2D, utilizado para la detección de colisiones del jugador.
        _collider = GetComponent<CapsuleCollider2D>();

        // Obtiene el componente Animator del hijo del objeto, que se usa para manejar las animaciones del jugador.
        _anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Este método se llama una vez, justo antes de que comience el primer frame del juego.
        Debug.Log("Start was called");
        // Almacena la escala de gravedad predeterminada del Rigidbody2D, para usarla más adelante.
        _defaultGravityScale = _rb.gravityScale;

        
        _gameManager = GameManager.instance;
        UpdateGameDifficulty();
        // Llama al método que ajusta si la reaparición (respawn) ha finalizado, desactivando el control del jugador inicialmente.
        RespawnFinished(false);
        UpdateSkin();
    }

    public void Damage()
    {
        if (_gameDifficulty == DifficultyType.Normal)
        {
            if (_gameManager.GetFruitCollected() <= 0)
            {
                // Llama al método Die del jugador para que el jugador muera.
                Die();
                // Restart level
                _gameManager.RestartLevel();
            }
            else
            {
                _gameManager.RemoveFruit();
            }
            return;
        }

        if (_gameDifficulty == DifficultyType.Hard)
        {
            // Llama al método Die del jugador para que el jugador muera.
            Die();
            
            // Restart level
            _gameManager.RestartLevel();
        }
        
        
    }

    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;
        if (difficultyManager != null)
        {
            _gameDifficulty = DifficultyManager.instance.difficulty;
        }
    }

    // Método Update se llama una vez por cada frame, actualizando constantemente el estado del jugador.
    void Update()
    {
        // Imprime un mensaje en la consola cada vez que se ejecuta Update (para depuración).
        Debug.Log("Update was called");

        // Actualiza el estado del jugador si está en el aire (cuando no está tocando el suelo).
        UpdateAirborneStatus();

        // Si el jugador no puede ser controlado (por ejemplo, durante un knockback o empuje), maneja las animaciones y colisiones, y sale del método.
        if (!_canBeControlled)
        {
            HandleAnimations(); // Controla las animaciones del jugador basadas en su estado actual.
            HandleCollisions(); // Controla las colisiones del jugador con otros objetos en la escena.
            return; // Detiene la ejecución del resto del código en este frame. 
        }

        // Si el jugador está en estado de knockback, no realiza ninguna acción adicional durante este frame.
        if (_isKnocked)
        {
            return;
        }

        HandleEnemyDetection();

        // Si el jugador puede ser controlado y no está en knockback, maneja las entradas y otras acciones.
        HandleInputs(); // Detecta y procesa las entradas del jugador (como movimiento, saltos, etc.).
        HandleWallSlide(); // Gestiona el deslizamiento en las paredes si el jugador está en contacto con una.
        HandleMovement(); // Gestiona el movimiento del jugador, incluyendo velocidad y dirección.
        HandleFlip(); // Gestiona el volteo del sprite del jugador basado en la dirección en la que se está moviendo.
        HandleCollisions(); // Detecta y maneja las colisiones con otros objetos.
        HandleAnimations(); // Actualiza las animaciones del jugador basadas en su estado actual (correr, saltar, caer, etc.).
    }

    // Método para actualizar el skin en el juego.
    public void UpdateSkin()
    {
        // Obtiene la instancia de SkinManager.
        SkinManager skinManager = SkinManager.instance;

        // Verifica si el SkinManager existe antes de proceder.
        if (skinManager == null)
            return;

        // Obtiene el ID de la skin seleccionada del SkinManager.
        skinId = skinManager.GetSkinId();

        // Asigna el controlador de animación correspondiente al ID de la skin seleccionada.
        _anim.runtimeAnimatorController = _animators[skinId];
    }

    // Método para detectar enemigos y manejar la colisión con ellos.
    private void HandleEnemyDetection()
    {
        // Si el jugador no está descendiendo (velocidad en y >= 0), se detiene el método.
        if (_rb.linearVelocity.y >= 0)
        {
            return;
        }

        // Detecta todos los colliders dentro del radio de _enemyCheck que pertenecen a la capa especificada en _whatIsEnemy.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_enemyCheck.position, _enemyCheckRadius, _whatIsEnemy);

        // Recorre todos los colliders detectados.
        foreach (var enemy in colliders)
        {
            // Intenta obtener el componente Enemy del objeto colisionado.
            Enemy currentEnemy = enemy.gameObject.GetComponent<Enemy>();

            if (currentEnemy != null)
            {
                // Si el objeto colisionado es un enemigo, ejecuta su método Die() y hace que el jugador salte.
                currentEnemy.Die();
                Jump();
            }
        }
    }

// Este método gestiona lo que sucede cuando la reaparición (respawn) ha terminado o no.
    // Recibe un parámetro booleano 'finished', que indica si la reaparición ha finalizado.
    public void RespawnFinished(bool finished)
    {
        // Guarda el valor de la gravedad predeterminada en una variable local.
        float gravityScale = _defaultGravityScale;

        if (finished)
        {
            // Si la reaparición ha terminado, restaura la gravedad del jugador a su valor predeterminado.
            _rb.gravityScale = gravityScale;

            // Habilita el control del jugador.
            _canBeControlled = true;

            // Habilita el collider del jugador, permitiéndole interactuar con otros objetos.
            _collider.enabled = true;
        }
        else
        {
            // Si la reaparición no ha terminado, desactiva la gravedad.
            _rb.gravityScale = 0;

            // Desactiva el control del jugador.
            _canBeControlled = false;

            // Desactiva el collider, para evitar interacciones durante el respawn.
            _collider.enabled = false;
        }
    }

    // Método para gestionar el knockback (retroceso que sufre el personaje al ser golpeado)
    public void Knockback(float sourceDamageXPosition)
    {
        // Variable que determina la dirección del knockback. Inicialmente, se asume que el knockback será hacia la derecha.
        float knockbackDir = 1;

        // Compara la posición del personaje con la fuente del daño (sourceDamageXPosition).
        // Si el personaje está a la izquierda de la fuente de daño, el knockback debe ir hacia la izquierda (por lo tanto, se cambia la dirección).
        if (transform.position.x < sourceDamageXPosition)
        {
            knockbackDir = -1; // El knockback será hacia la izquierda.
        }

        // Verifica si el personaje ya está en un estado de knockback (si está siendo empujado).
        if (_isKnocked)
        {
            return; // Si ya está en knockback, no hace nada y se sale del método.
        }
        
        CameraManager.instance.ScreenShake(knockbackDir);

        // Inicia una corrutina que manejará el estado del knockback durante un tiempo determinado.
        StartCoroutine(KnockbackCoroutine());

        // Aplica una fuerza de knockback en el Rigidbody2D (_rb) del personaje.
        // La fuerza se aplica tanto en el eje X como en el eje Y (según el valor de _knockbackPower).
        // El valor de knockbackDir determina la dirección del retroceso en el eje X (hacia la derecha o izquierda).
        _rb.linearVelocity = new Vector2(_knockbackPower.x * knockbackDir, _knockbackPower.y);
    }


    // Corrutina para gestionar la duración del knockback
    private IEnumerator KnockbackCoroutine()
    {
        _isKnocked = true; // Marca que el jugador está en knockback

        // Activa la animación de knockback
        _anim.SetBool("isknocked", _isKnocked);

        yield return new WaitForSeconds(_knockbackDuration); // Espera la duración del knockback

        _isKnocked = false; // Finaliza el estado de knockback

        // Desactivamos la animación de knockback
        _anim.SetBool("isknocked", _isKnocked);
    }

    // Método público que destruye el GameObject actual (el objeto que tiene este script).
    public void Die()
    {
        // Instancia el efecto visual de muerte en la posición actual del objeto y con la rotación por defecto (sin rotación).
        GameObject newDeathVfx = Instantiate(_playerDeath_vfx, this.transform.position, Quaternion.identity);

        // Elimina el objeto actual (por ejemplo, el Player) de la escena, liberando memoria y recursos asociados.
        Destroy(this.gameObject);
    }


    // Método para empujar al jugador en una dirección específica con una duración opcional.
    public void Push(Vector2 direction, float duration = 0f)
    {
        // Inicia una corrutina para manejar el empuje del jugador.
        StartCoroutine(PushCoroutine(direction, duration));
    }

    // Corrutina que gestiona el empuje del jugador durante un tiempo determinado.
    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        // Desactiva el control del jugador para que no pueda moverse durante el empuje.
        _canBeControlled = false;

        // Establece la velocidad del Rigidbody2D a cero antes de aplicar la fuerza para evitar interferencias.
        _rb.linearVelocity = Vector2.zero;

        // Aplica una fuerza instantánea en la dirección especificada, lo que empuja al jugador.
        _rb.AddForce(direction, ForceMode2D.Impulse);

        // Espera el tiempo especificado antes de permitir que el jugador vuelva a moverse.
        yield return new WaitForSeconds(duration);

        // Reactiva el control del jugador para que pueda ser controlado nuevamente.
        _canBeControlled = true;
    }

    // Actualiza si el jugador está en el aire o no
    private void UpdateAirborneStatus()
    {
        if (_isGrounded && _isAirborne)
        {
            HandleLanding(); // Si aterrizó, llama a la función para manejar el aterrizaje
        }

        if (!_isGrounded && !_isAirborne)
        {
            BecomeAirborne(); // Si el jugador ha saltado, se considera en el aire
        }
    }

    // Maneja el aterrizaje del jugador
    private void HandleLanding()
    {
        _isAirborne = false; // Marca que ya no está en el aire
        _canDoubleJump = true; // Permite hacer un doble salto
        AttemptBufferJump(); // Intenta realizar un salto si hay buffer jump disponible
    }

    // Marca al jugador como en el aire
    private void BecomeAirborne()
    {
        _isAirborne = true;

        if (_rb.linearVelocity.y < 0)
        {
            // Activa el salto coyote cuando el jugador empieza a caer
            Debug.Log("Activate coyote jump");
            ActivateCoyoteJump();
        }
    }

    // Maneja las entradas del jugador
    private void HandleInputs()
    {
        _xInput = Input.GetAxisRaw("Horizontal"); // Captura la entrada horizontal del jugador
        _isPressingHorizontalButtons = _xInput != 0; // Verifica si se están presionando las teclas de movimiento
        _yInput = Input.GetAxisRaw("Vertical"); // Captura la entrada vertical del jugador

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Cuando se presiona el espacio, intenta saltar
            JumpButton();
            RequestBufferJump(); // Solicita un salto buffer si está en el aire
        }
    }

    private void RequestBufferJump()
    {
        // Si el personaje está en el aire (_isAirborne es verdadero)
        if (_isAirborne)
        {
            // Activa el buffer jump estableciendo la variable _bufferJumpActivated al tiempo actual del juego
            _bufferJumpActivated = Time.time;
        }
    }

    private void AttemptBufferJump()
    {
        // Comprueba si el tiempo actual está dentro del intervalo permitido por la ventana de buffer (bufferJumpWindow)
        if (Time.time < _bufferJumpActivated + _bufferJumpWindow)
        {
            // Reinicia _bufferJumpActivated para que no se repita el salto buffer
            _bufferJumpActivated = Time.time - 1;

            // Llama a la función Jump() para ejecutar el salto
            Jump();
        }
    }

    private void ActivateCoyoteJump()
    {
        // Activa el salto de coyote asignando el tiempo actual a la variable _coyoteJumpActivated
        _coyoteJumpActivated = Time.time;
    }

    private void CancelCoyoteJump()
    {
        // Cancela el salto de coyote estableciendo el valor de _coyoteJumpActivated a un tiempo anterior al actual
        _coyoteJumpActivated = Time.time - 1;
    }

    private void JumpButton()
    {
        // Verifica si el salto de coyote aún está disponible, comparando el tiempo actual con el tiempo en que se activó el coyote jump más su ventana
        bool coyoteJumpAvailable = Time.time < _coyoteJumpActivated + _coyoteJumpWindow;

        // Si el personaje está en el suelo o el salto de coyote está disponible
        if (_isGrounded || coyoteJumpAvailable)
        {
            // Ejecuta el salto normal
            Jump();
        }
        // Si se detecta una pared y se están presionando los botones de dirección horizontal mientras no está en el suelo
        else if (_isWallDetected && _isPressingHorizontalButtons && !_isGrounded)
        {
            // Ejecuta un salto en la pared
            WallJump();
        }
        // Si el personaje aún puede hacer un doble salto (_canDoubleJump es verdadero)
        else if (_canDoubleJump)
        {
            // Ejecuta el doble salto
            DoubleJump();
        }

        // Cancela el salto de coyote tras el intento de salto
        CancelCoyoteJump();
    }

    private void Jump()
    {
        // Cambia la velocidad vertical del personaje al valor de _jumpForce, manteniendo la velocidad horizontal
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
    }

    private void DoubleJump()
    {
        // Desactiva el estado de salto en la pared (por si estuviera activado)
        _isWallJumping = false;

        // Desactiva la capacidad de hacer otro doble salto
        _canDoubleJump = false;

        // Aplica la fuerza del doble salto a la velocidad vertical del personaje
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _doubleJumpForce);
    }

    private void WallJump()
    {
        // Aplica la fuerza de salto en la pared en ambas direcciones: horizontal (según el lado al que mire el personaje) y vertical
        _rb.linearVelocity = new Vector2(_wallJumpForce.x * _facingDir, _wallJumpForce.y);

        // Permite realizar un doble salto después de hacer un salto en la pared
        _canDoubleJump = true;

        // Inicia la corrutina para gestionar el tiempo que el personaje está en estado de salto en la pared
        StartCoroutine(WallJumpCoroutine());
    }

    private IEnumerator WallJumpCoroutine()
    {
        // Establece que el personaje está en estado de salto en la pared
        _isWallJumping = true;

        // Espera la duración del salto en la pared (_wallJumpDuration) antes de continuar
        yield return new WaitForSeconds(_wallJumpDuration);

        // Finaliza el estado de salto en la pared
        _isWallJumping = false;
    }

    private void HandleMovement()
    {
        // Si el personaje está detectando una pared, detiene el movimiento normal (evita que se mueva mientras está pegado a la pared)
        if (_isWallDetected)
        {
            return;
        }

        // Actualiza la velocidad horizontal del personaje según la entrada del jugador (_xInput) multiplicada por la velocidad de movimiento (_moveSpeed),
        // manteniendo la velocidad vertical actual
        _rb.linearVelocity = new Vector2(_xInput * _moveSpeed, _rb.linearVelocity.y);
    }


    private void HandleAnimations()
    {
        // Actualiza el valor "xVelocity" en el animador con la velocidad horizontal actual del personaje
        _anim.SetFloat("xVelocity", _rb.linearVelocity.x);

        // Actualiza el valor "isPressingHorizontalButtons" en el animador según si se están presionando los botones de movimiento horizontal
        _anim.SetBool("isPressingHorizontalButtons", _isPressingHorizontalButtons);

        // Actualiza el valor "yVelocity" en el animador con la velocidad vertical actual del personaje
        _anim.SetFloat("yVelocity", _rb.linearVelocity.y);

        // Actualiza el valor "isGrounded" en el animador para reflejar si el personaje está en el suelo
        _anim.SetBool("isGrounded", _isGrounded);

        // Actualiza el valor "isWallDetected" en el animador para indicar si se detecta una pared cerca del personaje
        _anim.SetBool("isWallDetected", _isWallDetected);
    }

    private void HandleFlip()
    {
        // Verifica si el personaje debe voltear: 
        // Si está moviéndose hacia la izquierda (xInput < 0) y mirando a la derecha, o viceversa
        if (_xInput < 0 && _facingRight || _xInput > 0 && !_facingRight)
        {
            Flip(); // Si las condiciones se cumplen, invoca la función Flip para voltear al personaje
        }
    }

    private void Flip()
    {
        // Invierte el valor de _facingRight para reflejar que el personaje ha girado
        _facingRight = !_facingRight;

        // Cambia el valor de _facingDir para que las detecciones de colisión se realicen correctamente después del giro
        _facingDir *= -1;

        // Gira el personaje 180 grados en el eje Y, cambiando la dirección visual
        transform.Rotate(0, 180, 0);
    }

    private void HandleWallSlide()
    {
        // Si el jugador está presionando el botón hacia abajo, modifica la velocidad del deslizamiento
        float yModifier = _yInput < 0 ? 1 : 0.5f;

        // Si no se están presionando los botones horizontales, no realiza ningún deslizamiento en la pared
        if (!_isPressingHorizontalButtons)
        {
            return;
        }

        // Si se detecta una pared y el personaje está cayendo (velocidad y < 0)
        if (_isWallDetected && _rb.linearVelocity.y < 0)
        {
            Debug.Log("Wall Sliding Now"); // Mensaje de depuración indicando que se está deslizando por la pared

            // Modifica la velocidad vertical para simular un deslizamiento lento en la pared
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * yModifier);
        }
    }

    private void HandleCollisions()
    {
        // Realiza una detección de colisión con el suelo utilizando un Raycast que se proyecta hacia abajo
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance, _whatIsGround);

        // Realiza una detección de colisión con una pared utilizando un Raycast proyectado hacia la dirección en que mira el personaje
        _isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * _facingDir, _wallCheckDistance,
            _whatIsGround);
    }

    // Método para visualizar las colisiones en el editor y facilitar el ajuste de los parámetros de detección.
    private void OnDrawGizmos()
    {
        // Dibuja una esfera de alambre en el editor que representa el área de detección de enemigos.
        Gizmos.DrawWireSphere(_enemyCheck.position, _enemyCheckRadius);

        // Dibuja una línea desde la posición del jugador hacia abajo para representar la detección de suelo.
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x, transform.position.y - _groundCheckDistance));

        // Dibuja una línea hacia la derecha o izquierda desde la posición del jugador para representar la detección de paredes.
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x + (_wallCheckDistance * _facingDir), transform.position.y));
    }
}