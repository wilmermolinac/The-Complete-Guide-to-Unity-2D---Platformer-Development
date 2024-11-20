using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    // Define una clase GameManager que gestionará elementos clave del juego.
    // 'MonoBehaviour' es la clase base de todos los scripts que se pueden adjuntar a objetos en Unity.

    // Variable estática que contendrá la instancia única del GameManager (patrón singleton).
    public static GameManager instance;
    private UI_InGame _inGame;

    [Header("Level Managment")] [SerializeField]
    private float _levelTimer;

    [SerializeField] private int _currentLevelIndex;

    private int _nextLevelIndex;

    [Header("Player")]
    // Variable para almacenar el prefab (molde) del jugador, que se utilizará para instanciar un nuevo jugador cuando sea necesario.
    [SerializeField]
    private GameObject _playerPrefab;

    // Variable que almacena la posición de reaparecimiento (respawn) del jugador. Es un punto en la escena donde el jugador aparecerá al ser instanciado.
    [SerializeField] private Transform _playerRespawnPoint;
    [SerializeField] private float _playerRespawnDelay;

    // Referencia pública al objeto 'Player', lo que permite que otros scripts accedan fácilmente al jugador en la escena.
    public Player player;

    [Header("Fruit Management")] // Este atributo agrupa las siguientes variables bajo un encabezado en el inspector de Unity.
    public bool
        fruitAreRandom; // Indica si las frutas tendrán un aspecto aleatorio (puede ser configurado en el inspector).

    public int fruitCollected; // Contador de frutas recogidas por el jugador durante el juego.

    // Total de frutas en la scena
    public int totalFruits;

    [Header("Checkpoints")] public bool canReactivate;

    [Header("Traps")] public GameObject arrowPrefab;

    [Header("Managers")] [SerializeField] private AudioManager _audioManager;

    private void Awake()
    {
        // El método Awake se ejecuta cuando la instancia del script se carga en la escena,
        // antes del método Start. Aquí se implementa el patrón singleton.

        // Si no existe ninguna instancia del GameManager, asigna esta instancia a 'instance'.
        if (instance == null)
        {
            instance = this; // 'this' se refiere a la instancia actual de GameManager.
        }
        else
        {
            // Si ya existe una instancia, destruye este GameObject para mantener una única instancia
            // del GameManager en la escena. Esto evita duplicados y garantiza que siempre haya un solo GameManager.
            Destroy(gameObject);
        }
    }

    // Cuando se inica la Scene
    private void Start()
    {
        // Inicializamos UI_InGame
        _inGame = UI_InGame.instance;

        // Asignamos el índice de la escena actual a la variable _currentLevelIndex
        // SceneManager.GetActiveScene() obtiene la escena que está actualmente activa
        // buildIndex es una propiedad de la escena activa que devuelve el índice de la escena en la lista de compilación
        _currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        // Desbloquemos la Scene actual
        PlayerPrefs.SetInt(Constants.KEY_UNLOCK_LEVEL + _currentLevelIndex, 1);

        // Guarda el nivel actual
        PlayerPrefs.SetInt(Constants.KEY_CONTINUE_NUMBER_LEVEL, _currentLevelIndex);

        if (_playerRespawnPoint == null)
        {
            _playerRespawnPoint = FindFirstObjectByType<StartPoint>().transform;
        }

        if (player != null)
        {
            player= FindFirstObjectByType<Player>();
        }

        // obtenemos el index de la siguiente scene
        _nextLevelIndex = _currentLevelIndex + 1;

        CollectFruitInfo();

        CreateManagersIfNeeded();
    }

    private void Update()
    {
        _levelTimer += Time.deltaTime;
        _inGame.UpdateTimerText(_levelTimer);
    }

    private void CreateManagersIfNeeded()
    {
        if (AudioManager.instance == null)
        {
            Instantiate(_audioManager);
        }
    }

    // Método privado para recopilar información sobre todas las frutas en la escena.
    private void CollectFruitInfo()
    {
        // Busca y obtiene una matriz de todas las instancias del componente Fruit en la escena.
        Fruit[] allFruits = FindObjectsOfType<Fruit>();

        // Almacena la cantidad total de frutas encontradas en la variable totalFruits.
        totalFruits = allFruits.Length;

        // Guardamos el total de fruits en el nivel
        PlayerPrefs.SetInt(Constants.KEY_TOTAL_FRUITS_LEVEL + _currentLevelIndex, totalFruits);

        UpdateFruitUI();
    }


    public void RespawnPlayer()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null && difficultyManager.difficulty == DifficultyType.Hard)
            return;


        // Ejecuramos una Coroutina
        StartCoroutine(RespawnPlayerCoroutine());
    }

    // Este método está en el GameManager y se encarga de actualizar la posición de respawn del jugador.
    // Recibe como parámetro un Transform, que indica el nuevo punto de reaparición.
    public void UpdateRespawnPlayerPosition(Transform newRespawnPoint)
    {
        // Actualiza el punto de reaparición del jugador con el nuevo punto.
        _playerRespawnPoint = newRespawnPoint;
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        // Hacemos un delay segun el tiempo que le indiquemos aqui _playerRespawnDelay
        yield return new WaitForSeconds(_playerRespawnDelay);

        // Instancia (crea) un nuevo jugador a partir del prefab en la posición de respawn y sin rotación (Quaternion.identity).
        GameObject newPlayer = Instantiate(_playerPrefab, _playerRespawnPoint.position, Quaternion.identity);

        // Asigna el nuevo objeto instanciado (newPlayer) a la variable 'player', y obtiene el componente 'Player' de este objeto.
        // Esto permite interactuar con el nuevo jugador a través de esta referencia.
        player = newPlayer.GetComponent<Player>();
    }

    public void AddFruit()
    {
        // Método público para aumentar el contador de frutas recogidas por el jugador.
        // Incrementa la variable 'fruitCollected' en 1.
        // Forma abreviada de 'fruitCollected = fruitCollected + 1'.
        fruitCollected++;
        UpdateFruitUI();
    }

    public void RemoveFruit()
    {
        fruitCollected--;
        _inGame.UpdateFruitUI(fruitCollected, totalFruits);
    }

    public int GetFruitCollected()
    {
        return fruitCollected;
    }

    private void UpdateFruitUI()
    {
        _inGame.UpdateFruitUI(fruitCollected, totalFruits);
    }

    public bool FruitHaveRandomLook()
    {
        // Método público que devuelve el valor de 'fruitHaveRandomLook'.
        // Indica si las frutas deben tener un aspecto aleatorio (true o false).
        return fruitAreRandom;
    }

    // Método que crea un objeto instanciando un prefab en la posición del target, con la opción de un retraso antes de la creación.
    public void CreateObject(GameObject prefab, Transform target, float delay = 0)
    {
        // Inicia la corrutina CreateObjectCoroutine, que se encargará de la creación del objeto después de un posible retraso.
        // Se le pasan los parámetros: el prefab a instanciar, el target donde se posicionará y el retraso opcional (por defecto es 0).
        StartCoroutine(CreateObjectCoroutine(prefab, target, delay));
    }


    // Corrutina que crea un nuevo objeto (instancia de prefab) después de un retraso especificado.
    private IEnumerator CreateObjectCoroutine(GameObject prefab, Transform target, float delay)
    {
        // Almacena la posición del objeto de destino (target) en una variable Vector3.
        Vector3 newPosition = target.position;

        // Pausa la ejecución de la corrutina durante el tiempo especificado en el parámetro 'delay'.
        yield return new WaitForSeconds(delay);

        // Instancia (crea) un nuevo objeto usando el prefab proporcionado, en la posición almacenada (newPosition) y con una rotación por defecto (Quaternion.identity).
        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);

        // Aquí podrías añadir más lógica, como modificar propiedades del nuevo objeto creado si fuera necesario.
    }

    // Método que se llama cuando el jugador completa un nivel.
    public void LevelFinished()
    {
        SaveLevelProgress();

        // Llama al método para cargar la siguiente escena.
        LoadNextScene();
    }


    public void SaveLevelProgress()
    {
        // Marca la escena actual como completada en PlayerPrefs.
        PlayerPrefs.SetInt(Constants.KEY_COMPLETED_LEVEL + _currentLevelIndex, 1);

        // Guardamos el tiempo actual 
        SaveBestTime();

        SaveFruitsInfo();

        if (!NoMoreLevels())
        {
            // Desbloquea la siguiente escena en PlayerPrefs para que sea accesible.
            PlayerPrefs.SetInt(Constants.KEY_UNLOCK_LEVEL + _nextLevelIndex, 1);
            // Guarda el index del sigiente nivel
            PlayerPrefs.SetInt(Constants.KEY_CONTINUE_NUMBER_LEVEL, _nextLevelIndex);
            SkinManager skinManager = SkinManager.instance;
            if (skinManager != null)
            {
                PlayerPrefs.SetInt(Constants.KEY_SKIN_INDEX, skinManager.GetSkinId());
            }
        }
    }

    private void SaveBestTime()
    {
        float currentBestTimeLevel = PlayerPrefs.GetFloat(Constants.KEY_BEST_TIME_LEVEL + _currentLevelIndex, 0);
        if (_levelTimer < currentBestTimeLevel)
        {
            PlayerPrefs.SetFloat(Constants.KEY_BEST_TIME_LEVEL + _currentLevelIndex, _levelTimer);
        }
    }

    private void SaveFruitsInfo()
    {
        int fruitCollectedLevelBefore = PlayerPrefs.GetInt(Constants.KEY_FRUIT_COLLECTED_LEVEL + _currentLevelIndex, 0);
        if (fruitCollectedLevelBefore < fruitCollected)
        {
            PlayerPrefs.SetInt(Constants.KEY_FRUIT_COLLECTED_LEVEL + _currentLevelIndex, fruitCollected);

            int totalFruitsInBank = PlayerPrefs.GetInt(Constants.KEY_TOTAL_FRUITS_AMOUNT, 0);
            totalFruitsInBank = totalFruitsInBank + fruitCollected;
            PlayerPrefs.SetInt(Constants.KEY_TOTAL_FRUITS_AMOUNT, totalFruitsInBank);
        }
    }

    public void RestartLevel()
    {
        UI_InGame.instance.fadeEffect.ScreenFade(1, 0.75f, LoadCurrentScene);
    }

    private void LoadCurrentScene()
    {
        SceneManager.LoadScene(Constants.KEY_NAME_LEVEL + _currentLevelIndex);
    }

// Carga la escena "TheEnd" al finalizar el último nivel.
    private void LoadTheEndScene()
    {
        SceneManager.LoadScene("TheEnd");
    }

// Carga la siguiente escena de nivel.
    private void LoadTheNextLevel()
    {
        SceneManager.LoadScene($"Level_{_nextLevelIndex}");
    }

// Carga la próxima escena en función de si hay niveles restantes o si es la última escena.
    private void LoadNextScene()
    {
        // Referencia al efecto de desvanecimiento en pantalla de la UI.
        Ui_FadeEffect fadeEffect = UI_InGame.instance.fadeEffect;

        // Si no hay más niveles, aplica un desvanecimiento y carga la escena "TheEnd".
        if (NoMoreLevels())
        {
            fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        }
        else
        {
            // Si hay más niveles, aplica un desvanecimiento y carga el siguiente nivel.
            fadeEffect.ScreenFade(1, 1.5f, LoadTheNextLevel);
        }
    }

    private bool NoMoreLevels()
    {
        // Calcula el índice del último nivel disponible en el juego.
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2;

        // Verifica si el nivel actual es el último.
        bool noMoreLevels = _currentLevelIndex == lastLevelIndex;
        return noMoreLevels;
    }
}