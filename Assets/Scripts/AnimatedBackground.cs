using System;
using UnityEngine;

// Enumera los diferentes tipos de fondo disponibles.
public enum BackgroundType
{
    Blue,   // Fondo de color azul.
    Brown,  // Fondo de color marrón.
    Gray,   // Fondo de color gris.
    Green,  // Fondo de color verde.
    Pink,   // Fondo de color rosa.
    Purple, // Fondo de color púrpura.
    Yellow  // Fondo de color amarillo.
}

// Clase que controla el comportamiento de un objeto con cambio de textura de fondo y movimiento.
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Vector2 _movementDirection; // Dirección del movimiento de la textura de fondo.
    private MeshRenderer _mr; // Referencia al componente MeshRenderer del objeto.

    [Header("Color")]
    [SerializeField] private BackgroundType _backgroundType; // Tipo de fondo seleccionado en el inspector.

    [SerializeField] private Texture2D[] _textures; // Array de texturas para cada tipo de fondo.

    // Método Awake, se llama al iniciar el script.
    private void Awake()
    {
        _mr = GetComponent<MeshRenderer>(); // Obtiene el componente MeshRenderer del objeto.
    }

    // Método Update, se llama una vez por frame.
    private void Update()
    {
        // Desplaza la textura del fondo en la dirección especificada.
        _mr.material.mainTextureOffset += _movementDirection * Time.deltaTime;

        UpdateBackgroundTexture(); // Actualiza la textura del fondo según el tipo seleccionado.
    }

    // Método que se puede llamar desde el menú contextual en el editor de Unity.
    [ContextMenu("Update Background Texture")]
    private void UpdateBackgroundTexture()
    {
        if (_mr == null)
        {
            _mr = GetComponent<MeshRenderer>(); // Asegura que el MeshRenderer esté asignado.
        }
        
        // Asigna la textura correspondiente al tipo de fondo actual.
        _mr.sharedMaterial.mainTexture = _textures[(int)_backgroundType];
    }
}
