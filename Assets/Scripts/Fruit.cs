using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// Definición de un enumerador llamado FruitType que lista los diferentes tipos de frutas disponibles.
public enum FruitType
{
    Apple,       // Manzana
    Banana,      // Plátano
    Cherry,      // Cereza
    Kiwi,        // Kiwi
    Melon,       // Melón
    Orange,      // Naranja
    Pineapple,   // Piña
    Strawberry   // Fresa
}

// Clase Fruit que hereda de MonoBehaviour, controlando el comportamiento de las frutas en el juego.
public class Fruit : MonoBehaviour
{
    private GameManager _gameManager; // Referencia al GameManager para interactuar con la lógica global del juego.

    private Animator _anim; // Referencia al componente Animator, encargado de manejar las animaciones de la fruta.

    // Variable privada que utiliza el tipo de enumeración FruitType para definir el tipo específico de fruta.
    [SerializeField] private FruitType _fruitType;

    // Variable para almacenar el efecto visual que se mostrará cuando se recolecte la fruta.
    [SerializeField] private GameObject _pickupVfx;

    private void Awake()
    {
        // Se obtiene la referencia del Animator del objeto hijo donde se encuentra el componente Animator.
        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // Inicializa la referencia al GameManager una vez que este ya ha sido creado.
        // Es importante hacer esto en Start porque el GameManager debe estar inicializado antes de usarlo aquí.
        _gameManager = GameManager.instance;

        // Llama al método que configura el aspecto de la fruta de manera aleatoria si es necesario.
        SetRandomLookIfNeeded();
    }

    // Método que asigna un aspecto aleatorio a la fruta si la opción está habilitada en el GameManager.
    private void SetRandomLookIfNeeded()
    {
        // Verifica si el GameManager permite que las frutas tengan un aspecto aleatorio.
        // Si no es el caso, simplemente actualiza las visuales según el tipo de fruta predefinido.
        if (_gameManager.FruitHaveRandomLook() == false)
        {
            // Actualiza las visuales de la fruta basándose en el tipo de fruta.
            UpdateFruitVisuals();
            return;
        }

        // Si el aspecto aleatorio está activado, selecciona un índice aleatorio entre 0 y 7 (hay 8 frutas).
        int randomIndex = Random.Range(0, 8);

        // Elimina la necesidad de un switch case y asigna directamente el índice aleatorio a la animación.
        _anim.SetFloat("fruitIndex", randomIndex);
    }

    // Método para actualizar las visuales de la fruta basado en su tipo actual (predefinido).
    private void UpdateFruitVisuals()
    {
        // Asigna el índice del tipo de fruta al animador, utilizando su valor como un entero.
        _anim.SetFloat("fruitIndex", (int)_fruitType);
    }

    // Este método se llama automáticamente cuando otro objeto con un Collider entra en contacto con el trigger de la fruta.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprueba si el objeto que colisionó es un jugador, intentando obtener el componente Player.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si el objeto es efectivamente un jugador (no es nulo), ejecuta las siguientes acciones.
        if (player != null)
        {
            // Incrementa el contador de frutas recogidas a través del GameManager.
            _gameManager.AddFruit();

            // Destruye el GameObject de la fruta inmediatamente tras ser recogida.
            Destroy(this.gameObject);

            // Crea una instancia del efecto visual (_pickupVfx) en la posición actual de la fruta.
            GameObject newFx = Instantiate(_pickupVfx, transform.position, Quaternion.identity);

            // Destruye el efecto visual creado después de 5 segundos para limpiar la memoria.
            //  Destroy(newFx, 1);
        }
    }
}
