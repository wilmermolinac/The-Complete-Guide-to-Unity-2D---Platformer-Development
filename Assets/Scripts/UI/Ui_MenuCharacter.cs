using System;
using UnityEngine;

// Clase que controla un personaje en el menú interactivo de la interfaz de usuario.
public class Ui_MenuCharacter : MonoBehaviour
{
    private Animator _anim; // Referencia al componente Animator para gestionar las animaciones.

    [SerializeField] private float _speed; // Velocidad de movimiento del personaje.

    private Vector2 _destination; // Destino actual hacia donde se mueve el personaje.

    private bool _isMoving = false; // Indica si el personaje se está moviendo.

    private int _facingDir = 1; // Dirección actual en la que está mirando el personaje (1 = derecha, -1 = izquierda).
    private bool _isFacingRight = true; // Indica si el personaje está mirando hacia la derecha.

    // Método Awake se ejecuta al cargar el script.
    private void Awake()
    {
        _anim = GetComponent<Animator>(); // Obtiene la referencia al componente Animator del personaje.
    }

    // Método Update se ejecuta en cada frame.
    private void Update()
    {
        // Actualiza el estado de movimiento en la animación.
        _anim.SetBool("isMoving", _isMoving);

        // Si el personaje está en movimiento, avanza hacia el destino.
        if (_isMoving)
        {
            // Mueve al personaje hacia el destino con una velocidad proporcional al tiempo.
            transform.position = Vector2.MoveTowards(transform.position, _destination, _speed * Time.deltaTime);

            // Detiene el movimiento si el personaje ha llegado al destino.
            if (Vector2.Distance(transform.position, _destination) < 0.1f)
            {
                _isMoving = false;
            }
        }
    }

    // Método para mover al personaje a una nueva posición.
    public void MoveTo(Transform newDestination)
    {
        // Define el nuevo destino, manteniendo la posición vertical actual.
        _destination = newDestination.position;
        _destination.y = transform.position.y;

        _isMoving = true; // Activa el estado de movimiento.
        HandleFlip(newDestination.position.x); // Verifica si es necesario voltear al personaje.
    }

    // Maneja el volteo del personaje según la posición del destino.
    private void HandleFlip(float xValue)
    {
        // Si el personaje necesita cambiar de dirección, lo voltea.
        if (xValue < transform.position.x && _isFacingRight || xValue > transform.position.x && !_isFacingRight)
        {
            Flip();
        }
    }

    // Método para voltear el personaje horizontalmente.
    private void Flip()
    {
        _facingDir *= -1; // Cambia la dirección actual.
        transform.Rotate(0f, 180f, 0f); // Rota al personaje en el eje Y.
        _isFacingRight = !_isFacingRight; // Actualiza el estado de dirección.
    }
}
