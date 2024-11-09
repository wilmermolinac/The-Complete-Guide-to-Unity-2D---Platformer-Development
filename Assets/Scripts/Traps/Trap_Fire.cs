using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que define el comportamiento de una trampa de fuego, que puede encenderse y apagarse, afectando al jugador u otros objetos.
public class Trap_Fire : MonoBehaviour
{
    // Referencias al Animator y al colisionador de cápsula que representa el área de impacto del fuego.
    private Animator _amim;
    private CapsuleCollider2D _fireCollider;

    // Bandera que indica si el fuego está activo o no.
    private bool _isActive;

    // Tiempo durante el cual el fuego permanece apagado.
    [SerializeField] private float _offDuration;

    // Referencia a un objeto Trap_FireButton, que puede controlar el encendido/apagado del fuego.
    [SerializeField] private Trap_FireButton _trapFireButton;

    // Método Awake, se llama antes del Start para inicializar componentes.
    private void Awake()
    {
        _amim = GetComponent<Animator>(); // Asigna el componente Animator del objeto.
        _fireCollider = GetComponent<CapsuleCollider2D>(); // Asigna el colisionador de cápsula del objeto.
    }

    // Método Start, se ejecuta al inicio para establecer configuraciones iniciales.
    private void Start()
    {
        // Comprueba si el botón de la trampa de fuego no está configurado y muestra una advertencia.
        if (_trapFireButton == null)
        {
            Debug.LogWarning($"Trap Fire Button not set in {gameObject.name}!");
        }

        // Activa el fuego por defecto al inicio del juego.
        SetFire(true);
    }

    // Método público para apagar el fuego. Inicia una corrutina que lo apaga temporalmente.
    public void SwitchOffFire()
    {
        // Si el fuego está apagado, no hace nada.
        if (!_isActive)
        {
            return;
        }

        // Inicia la corrutina FireCoroutine que apaga el fuego temporalmente.
        StartCoroutine(FireCoroutine());
    }

    // Corrutina que apaga el fuego por un tiempo determinado y luego lo vuelve a encender.
    private IEnumerator FireCoroutine()
    {
        SetFire(false); // Apaga el fuego.
        yield return new WaitForSeconds(_offDuration); // Espera la duración definida antes de volver a encenderlo.
        SetFire(true); // Enciende el fuego nuevamente.
    }

    // Método que establece el estado del fuego (activo o inactivo), controlando la animación y el colisionador.
    private void SetFire(bool active)
    {
        _amim.SetBool("isActive", active); // Establece el parámetro "isActive" en el Animator para cambiar la animación.
        _fireCollider.enabled = active; // Activa o desactiva el colisionador del fuego.
        _isActive = active; // Actualiza el estado interno de la trampa.
    }
}
