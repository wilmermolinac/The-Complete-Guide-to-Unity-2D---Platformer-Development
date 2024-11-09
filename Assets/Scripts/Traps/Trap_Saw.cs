using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase Trap_Saw que controla el comportamiento de una trampa de sierra en el juego.
public class Trap_Saw : MonoBehaviour
{
    // Referencia al componente Animator, que controlará las animaciones de la sierra.
    private Animator _anim;

    // Referencia al componente SpriteRenderer, que permitirá modificar visualmente el sprite de la sierra.
    private SpriteRenderer _sr;

    // Velocidad a la que la sierra se moverá entre los puntos de ruta (waypoints).
    [SerializeField] private float _moveSpeed = 3;

    // Tiempo de enfriamiento en segundos, durante el cual la sierra se detendrá después de completar un ciclo de movimiento.
    [SerializeField] private float _coolDown = 1;

    // Arreglo de puntos de ruta (waypoints) por los que se moverá la sierra.
    [SerializeField] private Transform[] _waypoints;

    // Arreglo para almacenar las posiciones de los waypoints en forma de Vector3.
    private Vector3[] _wayPointsPosition;

    // Índice del punto de ruta actual al que la sierra se dirige.
    public int waypointIndex = 1;

    // Dirección de movimiento de la sierra, controlada por este valor. -1 significa hacia atrás y 1 hacia adelante.
    public int moveDirection = 1;

    // Bandera para determinar si la sierra puede moverse o no.
    private bool _canMove = true;

    // Este método se llama cuando el script se inicializa. Se usa para obtener referencias a los componentes necesarios.
    private void Awake()
    {
        // Se obtiene el componente Animator para manejar las animaciones de la sierra.
        _anim = GetComponent<Animator>();

        // Se obtiene el componente SpriteRenderer para poder modificar visualmente el sprite de la sierra (como invertirlo).
        _sr = GetComponent<SpriteRenderer>();
    }

    // Este método se llama justo antes de que comience el juego. Se usa para establecer la posición inicial de la sierra.
    private void Start()
    {
        // Llama al método que actualiza las posiciones de los waypoints.
        UpdateWaypointsInfo();

        // Establece la posición inicial de la sierra en el primer punto de ruta del arreglo _wayPointsPosition.
        transform.position = _wayPointsPosition[0];
    }

    // Método para actualizar las posiciones de los waypoints y almacenarlas en un arreglo de Vector3, utilizando las posiciones actuales de los objetos Transform asociados.
    private void UpdateWaypointsInfo()
    {
        // Crea una lista de waypoints a partir de los componentes Trap_SawWaypoint hijos de este objeto.
        List<Trap_SawWaypoint> wayPointsList = new List<Trap_SawWaypoint>(GetComponentsInChildren<Trap_SawWaypoint>());

        // Verifica si la cantidad de waypoints ha cambiado en comparación con el arreglo _waypoints.
        if (wayPointsList.Count != _waypoints.Length)
        {
            // Ajusta el tamaño de _waypoints para que coincida con la cantidad de elementos en wayPointsList.
            _waypoints = new Transform[wayPointsList.Count];

            // Llena el arreglo _waypoints con las posiciones Transform de cada waypoint en la lista.
            for (int i = 0; i < _waypoints.Length; i++)
            {
                _waypoints[i] = wayPointsList[i].transform;
            }
        }

        // Inicializa el arreglo _wayPointsPosition, ajustándolo al tamaño de _waypoints.
        _wayPointsPosition = new Vector3[_waypoints.Length];

        // Recorre el arreglo _waypoints y almacena sus posiciones en _wayPointsPosition.
        for (int index = 0; index < _waypoints.Length; index++)
        {
            _wayPointsPosition[index] = _waypoints[index].position;
        }
    }


    // Este método se llama una vez por cuadro (frame). Se utiliza para manejar el movimiento de la sierra.
    private void Update()
    {
        // Actualiza la animación de la sierra, activando la animación de "isActive" si la sierra está en movimiento.
        _anim.SetBool("isActive", _canMove);

        // Si _canMove es falso, la sierra no se moverá, y saldrá del método Update.
        if (!_canMove)
        {
            return;
        }

        // Mueve la sierra desde su posición actual hacia el punto de ruta actual (waypointIndex) a una velocidad de _moveSpeed.
        transform.position = Vector2.MoveTowards(transform.position, _wayPointsPosition[waypointIndex],
            _moveSpeed * Time.deltaTime);

        // Verifica si la sierra ha llegado al punto de ruta actual comparando la distancia entre ellos.
        if (Vector2.Distance(transform.position, _wayPointsPosition[waypointIndex]) < 0.1f)
        {
            // Si la sierra ha llegado al último o al primer punto de ruta, cambia la dirección de movimiento.
            if (waypointIndex == _wayPointsPosition.Length - 1 || waypointIndex == 0)
            {
                // Cambia la dirección multiplicando por -1.
                moveDirection = moveDirection * -1;

                // Llama a la corrutina que detiene la sierra por el tiempo de enfriamiento.
                StartCoroutine(StopMovement(_coolDown));
            }

            // Actualiza el índice del punto de ruta sumando o restando según la dirección de movimiento.
            waypointIndex = waypointIndex + moveDirection;
        }
    }

    // Corrutina que detiene el movimiento de la sierra por un tiempo determinado antes de reanudarlo.
    private IEnumerator StopMovement(float delay)
    {
        // Detiene el movimiento de la sierra.
        _canMove = false;

        // Espera por un número de segundos determinado por el parámetro 'delay' antes de continuar.
        yield return new WaitForSeconds(delay);

        // Reactiva el movimiento de la sierra.
        _canMove = true;

        // Invierte la dirección visual de la sierra (gira el sprite horizontalmente).
        _sr.flipX = !_sr.flipX;
    }
}