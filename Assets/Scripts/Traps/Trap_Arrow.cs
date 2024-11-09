using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Clase Trap_Arrow que hereda de Trap_Trampoline. Controla el comportamiento de una trampa de flecha.
public class Trap_Arrow : Trap_Trampoline
{
    // Encabezado para organizar los campos adicionales en el Inspector de Unity.
    [Header("Additional info")] 

    // Tiempo de enfriamiento entre lanzamientos de flecha.
    [SerializeField] private float _cooldown;

    // Bandera que determina si la flecha rota hacia la derecha o no.
    [SerializeField] private bool _rotationRight;

    // Velocidad de rotación de la flecha.
    [SerializeField] private float _rotationSpeed = 120;

    // Dirección de rotación (-1 o 1), cambia dependiendo del valor de _rotationRight.
    private int _direction = -1;

    // Espacio visual en el Inspector para separar campos.
    [Space] 

    // Velocidad a la que la flecha aumentará de tamaño al aparecer.
    [SerializeField] private float _scaleUpSpeed = 10;

    // Tamaño objetivo que la flecha alcanzará durante la animación de aparición.
    [SerializeField] private Vector3 _targetScale;

    // Método Start se llama una vez al inicio del juego.
    private void Start()
    {
        // Establece el tamaño inicial de la flecha en un valor reducido para que se escale progresivamente.
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    // Método Update se llama una vez por frame.
    private void Update()
    {
        // Gestiona el escalado progresivo de la flecha.
        HandleScaleUp();

        // Gestiona la rotación continua de la flecha.
        HandleRotation();
    }

    // Método que gestiona el escalado de la flecha para que crezca hasta alcanzar _targetScale.
    private void HandleScaleUp()
    {
        // Si la escala actual de la flecha es menor que la escala objetivo, se continúa escalando.
        if (transform.localScale.x < _targetScale.x)
        {
            // Lerp interpola suavemente entre la escala actual y la escala objetivo, haciendo que la flecha crezca gradualmente.
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * _scaleUpSpeed);
        }
    }

    // Método que gestiona la rotación de la flecha.
    private void HandleRotation()
    {
        // Si _rotationRight es verdadero, la dirección de rotación será -1, de lo contrario, será 1.
        _direction = _rotationRight ? -1 : 1;

        // Rotamos la flecha alrededor del eje Z a una velocidad de _rotationSpeed, en la dirección determinada por _direction.
        transform.Rotate(0, 0, (_rotationSpeed * _direction) * Time.deltaTime);
    }

    // Método que se llama desde un evento de animación para destruir la flecha después de su uso.
    private void DestroyMe()
    {
        // Se obtiene la referencia al prefab de la flecha desde GameManager.
        GameObject arrowPrefab = GameManager.instance.arrowPrefab;

        // Crea una nueva flecha después de que se destruya la actual, usando el cooldown especificado.
        GameManager.instance.CreateObject(arrowPrefab, transform, _cooldown);

        // Destruye la instancia actual de la flecha.
        Destroy(gameObject);
    }
}
