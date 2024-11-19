using Core.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

// Clase que maneja la creación y visualización de los botones de selección de niveles en la interfaz de usuario.
public class Ui_LevelSelection : MonoBehaviour
{
    // Prefab del botón de nivel que se instanciará para cada nivel disponible.
    [SerializeField] private Ui_LevelButton _buttonPrefab;

    // El contenedor que agrupa los botones generados en la jerarquía de la escena.
    [SerializeField] private Transform _buttonsParent;

    // Array que almacena información sobre cada nivel.
    private LevelModel[] _levels;

    // Método Start: se ejecuta al inicio de la escena y llama a la función que crea los botones de nivel.
    private void Start()
    {
        CreateLevelButtons();
    }

    // Crea los botones de selección de niveles y los configura en función del progreso del jugador.
    private void CreateLevelButtons()
    {
        // Opcional: Borrar las preferencias del jugador (PlayerPrefs) para pruebas.
        //DeleteAllPlayerPrefs();

        // Inicializamos el array _levels con la cantidad de niveles,
        // excluyendo el menú principal y la escena final "TheEnd".
        _levels = new LevelModel[SceneManager.sceneCountInBuildSettings - 2];

        // Calculamos la cantidad total de niveles, excluyendo la última escena que corresponde a "TheEnd".
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        int levelId = 0;

        // Iniciamos el bucle en 1 para omitir el menú principal en la posición 0.
        for (int i = 1; i < levelsAmount; i++)
        {
            // Verificamos si el nivel actual está completado usando PlayerPrefs.
            bool isCompleted = PlayerPrefs.GetInt(Constants.KEY_COMPLETED_LEVEL + i, 0) == 1;

            // Verificamos si el nivel está desbloqueado.
            bool isUnlocked = PlayerPrefs.GetInt(Constants.KEY_UNLOCK_LEVEL + i, 0) == 1;

            int totalFruits = PlayerPrefs.GetInt(Constants.KEY_TOTAL_FRUITS_LEVEL + i, 0);
            float bestTime = PlayerPrefs.GetFloat(Constants.KEY_BEST_TIME_LEVEL + i, 0f);

            int fruitCollected = PlayerPrefs.GetInt(Constants.KEY_FRUIT_COLLECTED_LEVEL + i, 0);

            // Creamos un modelo de nivel con su información y lo agregamos al array.
            LevelModel level = new LevelModel(levelId, i, Constants.KEY_NAME_LEVEL + i, isCompleted, isUnlocked, totalFruits, bestTime, fruitCollected);
            
            _levels[levelId] = level;

            // Si el nivel está desbloqueado, generamos un botón de nivel.
            if (level.IsUnlocked)
            {
                Ui_LevelButton newButton = Instantiate(_buttonPrefab, _buttonsParent);
                newButton.SetupButton(level); // Configura el botón con la información del nivel.
                levelId++; // Incrementa el ID de nivel para el siguiente botón.
            }
        }
    }

    // Método para borrar todas las preferencias guardadas del jugador (PlayerPrefs).
    private void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll(); // Elimina todas las PlayerPrefs.
        PlayerPrefs.Save(); // Guarda los cambios.
        Debug.Log("Todas las PlayerPrefs han sido borradas.");
    }
}