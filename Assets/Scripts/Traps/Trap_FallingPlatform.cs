using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

// Clase que define el comportamiento de una plataforma que cae en respuesta a la interacción del jugador.
public class Trap_FallingPlatform : MonoBehaviour
{
    // Referencias a los componentes Rigidbody2D, Animator y un arreglo de colisionadores de la plataforma.
    private Rigidbody2D _rb;
    private Animator _anim;
    private BoxCollider2D[] _colliders;

    // Velocidad de movimiento de la plataforma entre puntos de ruta.
    [SerializeField] float speed = 0.75f;

    // Distancia de viaje de la plataforma entre los puntos de ruta.
    [SerializeField] private float _travelDistance;

    // Arreglo que almacena las posiciones de los puntos de ruta (waypoints) de la plataforma.
    private Vector3[] _wayPoints;

    // Índice para rastrear el punto de ruta actual.
    private int _wayPointIndex;

    // Bandera para verificar si la plataforma puede moverse o no.
    private bool _canMove = false;

    // Configuración de la caída de la plataforma al impacto.
    [Header("Platform fall settings")]
    [SerializeField] private float _impactSpeed = 3;

    // Duración de la animación de impacto antes de la caída.
    [SerializeField] private float _impactDuration = 0.1f;

    // Temporizador para controlar la duración del impacto.
    private float _impactTimer;

    // Bandera para verificar si el impacto ya ocurrió.
    private bool _impactHappened;

    // Tiempo de retraso antes de la caída de la plataforma.
    [SerializeField] private float _fallDelay = 0.5f;

    // Método de inicialización, se ejecuta al iniciar el juego.
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); // Asigna el Rigidbody2D de la plataforma.
        _anim = GetComponent<Animator>(); // Asigna el Animator de la plataforma.
        _colliders = GetComponents<BoxCollider2D>(); // Asigna todos los BoxCollider2D de la plataforma.
    }

    // Corrutina que se inicia al comienzo para configurar los waypoints y esperar un tiempo aleatorio antes de activar el movimiento.
    private IEnumerator Start()
    {
        SetupWaypoints(); // Configura los puntos de ruta iniciales de la plataforma.
        float randomDelay = UnityEngine.Random.Range(0f, 0.6f); // Calcula un retraso aleatorio.
        yield return new WaitForSeconds(randomDelay); // Espera el tiempo calculado.
        _canMove = true; // Activa el movimiento de la plataforma.
    }

    // Activa el movimiento de la plataforma.
    private void ActivatePlatform()
    {
        _canMove = true;
    }

    // Configura los puntos de ruta (waypoints) de la plataforma en función de la distancia de viaje especificada.
    private void SetupWaypoints()
    {
        _wayPoints = new Vector3[2]; // Inicializa un arreglo con dos puntos.

        float yOffset = _travelDistance / 2; // Calcula el desplazamiento vertical de la plataforma.

        _wayPoints[0] = transform.position + new Vector3(0, yOffset, 0); // Posición superior.
        _wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0); // Posición inferior.
    }

    // Método que se llama cada frame para gestionar el movimiento y el impacto de la plataforma.
    private void Update()
    {
        HandleImpact(); // Gestiona el efecto de impacto en la plataforma.
        HandleMovement(); // Gestiona el movimiento de la plataforma.
    }

    // Controla el movimiento de la plataforma entre los puntos de ruta.
    private void HandleMovement()
    {
        if (!_canMove)
        {
            return; // Si la plataforma no puede moverse, sale del método.
        }

        // Mueve la plataforma hacia el punto de ruta actual a una velocidad definida.
        transform.position = Vector2.MoveTowards(transform.position, _wayPoints[_wayPointIndex], speed * Time.deltaTime);

        // Comprueba si la plataforma ha alcanzado el punto de ruta actual.
        if (Vector2.Distance(transform.position, _wayPoints[_wayPointIndex]) < 0.1f)
        {
            _wayPointIndex++; // Pasa al siguiente punto de ruta.

            // Si llega al final del arreglo, vuelve al primer punto.
            if (_wayPointIndex >= _wayPoints.Length)
            {
                _wayPointIndex = 0;
            }
        }
    }

    // Controla el efecto de impacto, reduciendo la velocidad de la plataforma después de un impacto.
    private void HandleImpact()
    {
        if (_impactTimer < 0)
        {
            return; // Si el temporizador ha expirado, no hace nada.
        }

        _impactTimer -= Time.deltaTime; // Reduce el tiempo restante del impacto.
        
        // Mueve la plataforma hacia abajo con velocidad en función del temporizador de impacto.
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), _impactTimer * Time.deltaTime);
    } 

    // Método llamado al entrar en colisión con otro objeto, verificando si es el jugador.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_impactHappened)
        {
            return; // Si ya ocurrió un impacto, sale del método.
        }
        
        Player player = collision.gameObject.GetComponent<Player>(); // Obtiene la referencia al jugador.

        if (player != null)
        {
            Invoke(nameof(SwitchOffPlatform), _fallDelay); // Llama a SwitchOffPlatform después de un retraso.
            _impactTimer = _impactDuration; // Establece la duración del impacto.
            _impactHappened = true; // Marca que el impacto ya ocurrió.
        }
    }

    // Método que desactiva la plataforma, provocando su caída y deshabilitando los colisionadores.
    private void SwitchOffPlatform()
    {
        _anim.SetTrigger("deactivate"); // Activa la animación de desactivación.

        _canMove = false; // Detiene el movimiento de la plataforma.

        _rb.isKinematic = false; // Cambia a modo no cinemático para que la gravedad afecte a la plataforma.
        _rb.gravityScale = 3.5f; // Aumenta la gravedad para hacer que la plataforma caiga rápidamente.
        _rb.linearDamping = 0.5f; // Agrega un ligero efecto de fricción al caer.

        // Desactiva todos los colisionadores de la plataforma.
        foreach (BoxCollider2D collider in _colliders)
        {
            collider.enabled = false;
        }
    }
}
