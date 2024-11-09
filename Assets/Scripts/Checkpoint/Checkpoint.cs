using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que gestiona los puntos de control (checkpoints) en el juego.
public class Checkpoint : MonoBehaviour
{
    // Propiedad privada que obtiene una referencia al componente Animator del GameObject actual.
    private Animator _anim => GetComponent<Animator>();

    // Variable booleana que indica si el checkpoint está activo o no.
    private bool _isActive;

    // Variable que se puede configurar desde el Inspector de Unity. 
    // Define si el checkpoint puede ser reactivado tras ser activado una vez.
    [SerializeField] private bool _canBeReactivated;

    private void Start()
    {
        _canBeReactivated = GameManager.instance.canReactivate;
    }

    // Método que se llama automáticamente cuando otro objeto con un Collider2D entra en contacto con este GameObject.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el checkpoint ya está activo y no puede ser reactivado, no hace nada y retorna.
        if (_isActive && _canBeReactivated == false)
        {
            return;
        }

        // Intenta obtener el componente Player del objeto que colisionó.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si el objeto que colisionó es un jugador (es decir, el componente Player no es nulo).
        if (player != null)
        {
            // Llama al método que activa el checkpoint.
            ActivatedCheckpoint();
        }
    }

    // Método privado que se encarga de activar el checkpoint.
    private void ActivatedCheckpoint()
    {
        // Cambia el estado del checkpoint a activo.
        _isActive = true;

        // Dispara una animación en el Animator, configurada por el trigger "isActivate".
        _anim.SetTrigger("isActivate");

        // Llama al método del GameManager para actualizar la posición de reaparición del jugador.
        GameManager.instance.UpdateRespawnPlayerPosition(transform);
    }
}