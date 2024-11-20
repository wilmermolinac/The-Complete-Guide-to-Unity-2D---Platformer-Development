using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
// Clase que controla el menú principal de la interfaz de usuario.
public class Ui_MainMenu : MonoBehaviour
{
    private Ui_FadeEffect _fadeEffect; // Referencia al efecto de desvanecimiento.

    [SerializeField] private string _firstLevelName; // Nombre de la escena del primer nivel.
    [SerializeField] private GameObject[] uiElements; // Elementos de la UI que se mostrarán/ocultarán.
    [SerializeField] private GameObject _continueButton; // Botón para continuar un juego existente.

    [Header("Interactive Camera")]
    [SerializeField] private Ui_MenuCharacter _menuCharacter; // Controlador del personaje del menú.
    [SerializeField] private CinemachineCamera _cinemachine; // Cámara de Cinemachine para el menú.
    [SerializeField] private Transform _mainMenuPoint; // Posición de la cámara en el menú principal.
    [SerializeField] private Transform _skinSelectionPoint; // Posición de la cámara en el menú de selección de skin.

    // Inicialización de componentes al cargar el script.
    private void Awake()
    {
        _fadeEffect = GetComponentInChildren<Ui_FadeEffect>(); // Obtiene el efecto de desvanecimiento.
    }

    // Configuración inicial del menú principal.
    private void Start()
    {
        LoadProgress(); // Verifica el progreso guardado para habilitar el botón "Continuar".
        _fadeEffect.ScreenFade(0, 1.5f); // Efecto de desvanecimiento inicial.
        Time.timeScale = 1; // Asegura que el tiempo no esté pausado.
    }

    // Verifica si hay progreso guardado y habilita el botón "Continuar".
    private void LoadProgress()
    {
        if (HasLevelProgression())
        {
            _continueButton.SetActive(true); // Activa el botón si hay progreso.
        }
    }

    // Cambia entre diferentes pantallas del menú activando/desactivando elementos de UI.
    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject uiElement in uiElements)
        {
            uiElement.SetActive(false); // Desactiva todos los elementos.
        }

        uiToEnable.SetActive(true); // Activa el elemento especificado.
        AudioManager.instance.PlaySfx(4);
    }

    // Inicia un nuevo juego cargando la primera escena del nivel.
    public void NewGame()
    {
        _fadeEffect.ScreenFade(1, 1.5f, LoadLevelScene); // Desvanece antes de cargar la escena.
        AudioManager.instance.PlaySfx(4);
    }

    // Carga la escena del primer nivel.
    private void LoadLevelScene()
    {
        SceneManager.LoadScene(_firstLevelName); // Carga la escena por nombre.
    }

    // Verifica si hay niveles completados o guardados.
    private bool HasLevelProgression()
    {
        return PlayerPrefs.GetInt(Constants.KEY_CONTINUE_NUMBER_LEVEL, 0) > 0; // Comprueba el progreso.
    }

    // Continúa desde el último nivel guardado.
    public void ContinueGame()
    {
        int difficultyIndex = PlayerPrefs.GetInt(Constants.KEY_DIFFICULTY_INDEX, 1); // Obtiene la dificultad.
        int continueSceneIndex = PlayerPrefs.GetInt(Constants.KEY_CONTINUE_NUMBER_LEVEL, 0); // Última escena guardada.

        int lastSkinIndex = PlayerPrefs.GetInt(Constants.KEY_SKIN_INDEX, 0);
        SkinManager.instance.SetSkinId(lastSkinIndex); // Configura la skin guardada.
        DifficultyManager.instance.LoadDifficulty(difficultyIndex); // Configura la dificultad.

        SceneManager.LoadScene(Constants.KEY_NAME_LEVEL + continueSceneIndex); // Carga la escena guardada.
        
        AudioManager.instance.PlaySfx(4);
    }

    // Mueve la cámara al menú principal.
    public void MoveCameraToMainMenu()
    {
        _menuCharacter.MoveTo(_mainMenuPoint); // Mueve el personaje al menú principal.
        _cinemachine.Follow = _mainMenuPoint; // Cambia el objetivo de la cámara.
    }

    // Mueve la cámara al menú de selección de skin.
    public void MoveCameraToSkinMenu()
    {
        _menuCharacter.MoveTo(_skinSelectionPoint); // Mueve el personaje al menú de selección de skin.
        _cinemachine.Follow = _skinSelectionPoint; // Cambia el objetivo de la cámara.
    }
}
