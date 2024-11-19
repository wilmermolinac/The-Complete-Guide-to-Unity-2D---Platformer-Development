using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Clase que controla la finalización de un nivel en el juego.
public class Finshed : MonoBehaviour
{
    // Propiedad privada que obtiene una referencia al componente Animator del GameObject actual.
    // Se utiliza para gestionar las animaciones.
    private Animator _anim => GetComponent<Animator>();

    // Método que se llama automáticamente cuando otro objeto con un Collider2D entra en contacto con este GameObject.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intenta obtener el componente Player del objeto que colisionó.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si el objeto que colisionó es el jugador (es decir, el componente Player no es nulo).
        if (player != null)
        {
            // Dispara una animación configurada por el trigger "isActivate".
            _anim.SetTrigger("isActivate");

            // Imprime un mensaje en la consola de Unity indicando que el jugador completó el nivel.
            Debug.Log("You completed the level!");

            GameManager.instance.LevelFinished();
        }
    }
}