using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Clase que controla la interfaz de usuario durante el juego.
public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance; // Instancia global para acceder desde otros scripts.
    public Ui_FadeEffect fadeEffect { get; private set; } // Efecto de desvanecimiento.

    [SerializeField] private TextMeshProUGUI _timerText; // Texto del temporizador.
    [SerializeField] private TextMeshProUGUI _fruitText; // Texto de frutas recolectadas.

    [SerializeField] private GameObject _pauseMenuUI; // Menú de pausa.

    private bool _isPaused = false; // Indica si el juego está pausado.

    // Configura la instancia global y los componentes iniciales.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Evita duplicados de la interfaz.
        }

        fadeEffect = GetComponentInChildren<Ui_FadeEffect>();
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1); // Desvanece la pantalla al iniciar el juego.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            PauseBtn(); // Pausa o reanuda el juego.
        }
    }

    public void GoToMainMenuBtn()
    {
        SceneManager.LoadScene(0); // Carga el menú principal.
    }

    public void PauseBtn()
    {
        if (_isPaused)
        {
            _isPaused = false;
            Time.timeScale = 1; // Reanuda el tiempo.
            _pauseMenuUI.SetActive(false);
        }
        else
        {
            _isPaused = true;
            Time.timeScale = 0; // Pausa el tiempo.
            _pauseMenuUI.SetActive(true);
        }
    }

    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        _fruitText.text = $"{collectedFruits}/{totalFruits}"; // Actualiza el contador de frutas.
    }

    public void UpdateTimerText(float timer)
    {
        _timerText.text = timer.ToString("00") + " s"; // Actualiza el temporizador.
    }
}
