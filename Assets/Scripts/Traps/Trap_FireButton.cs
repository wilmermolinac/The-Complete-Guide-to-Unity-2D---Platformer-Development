using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que define el comportamiento de un botón que activa o desactiva una trampa de fuego.
public class Trap_FireButton : MonoBehaviour
{
    // Referencias al Animator del botón y a la trampa de fuego que controla.
    private Animator _anim;
    private Trap_Fire _trapFire;

    // Método Awake, se llama antes del Start para inicializar componentes.
    private void Awake()
    {
        // Asigna el componente Animator del botón.
        _anim = GetComponent<Animator>();
        // Asigna la referencia a la trampa de fuego en el objeto padre.
        _trapFire = GetComponentInParent<Trap_Fire>();
    }

    // Método que se ejecuta al detectar una colisión con el botón (cuando un objeto entra en el área de colisión).
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intenta obtener el componente Player del objeto que colisiona con el botón.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si el jugador (Player) interactúa con el botón, ejecuta las siguientes acciones.
        if (player != null)
        {
            // Activa la animación del botón para mostrar que ha sido presionado.
            _anim.SetTrigger("activate");
            // Llama al método SwitchOffFire() en la trampa de fuego, apagándola temporalmente.
            _trapFire.SwitchOffFire();
        }
    }
}