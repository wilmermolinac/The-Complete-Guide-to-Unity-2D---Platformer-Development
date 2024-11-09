using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase Trap_Trampoline que controla el comportamiento de una trampa de trampolín en el juego.
public class Trap_Trampoline : MonoBehaviour
{
    // Referencia al componente Animator para controlar las animaciones del trampolín.
    protected Animator anim;

    // Fuerza con la que el trampolín empujará al jugador hacia arriba.
    [SerializeField] private float _pushPower;

    // Duración en segundos del empuje hacia arriba que el trampolín aplicará al jugador.
    [SerializeField] private float _pushDuration = 0.5f;

    // Método Awake se llama cuando la instancia de este script es inicializada.
    // Se utiliza para obtener referencias a los componentes necesarios antes de que comience el juego.
    private void Awake()
    {
        // Obtiene el componente Animator que manejará las animaciones del trampolín.
        anim = GetComponent<Animator>();
    }

    // Método OnTriggerEnter2D se llama cuando otro collider entra en el trigger del trampolín.
    // Esto se utiliza para detectar la colisión del jugador con el trampolín.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que entra en contacto es el jugador.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si se ha detectado al jugador, se realiza el empuje.
        if (player != null)
        {
            // Empuja al jugador hacia arriba aplicando una fuerza basada en _pushPower y por un tiempo _pushDuration.
            player.Push(transform.up * _pushPower, _pushDuration);

            // Activa la animación del trampolín, utilizando el trigger "activate" para mostrar la animación de activación.
            anim.SetTrigger("activate");
        }
    }
}
