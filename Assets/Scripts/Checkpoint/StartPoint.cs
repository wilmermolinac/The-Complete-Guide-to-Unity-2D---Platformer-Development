using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase StartPoint que controla el comportamiento del punto de inicio del jugador en el juego.
public class StartPoint : MonoBehaviour
{
    // Propiedad privada que obtiene una referencia al componente Animator del GameObject actual.
    // Se utiliza para gestionar las animaciones del punto de inicio.
    private Animator _anim => GetComponent<Animator>();

    // Método que se ejecuta automáticamente cuando otro objeto con un Collider2D sale del área de colisión de este GameObject.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Intenta obtener el componente Player del objeto que colisiona.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si el objeto que sale de la colisión es el jugador (es decir, el componente Player no es nulo).
        if (player != null)
        {
            // Dispara una animación configurada por el trigger "isActivate".
            _anim.SetTrigger("isActivate");
        }
    }
}
