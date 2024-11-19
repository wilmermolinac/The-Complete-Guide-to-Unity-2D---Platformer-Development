using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Clase que controla los créditos en la interfaz de usuario.
public class Ui_Credits : MonoBehaviour
{
    private Ui_FadeEffect _fadeEffect; // Efecto de desvanecimiento.

    [SerializeField] private RectTransform _rectT; // Referencia al RectTransform del objeto de créditos.
    [SerializeField] private float scrollSpeed = 200f; // Velocidad de desplazamiento vertical de los créditos.
    [SerializeField] private float offScreenPosition = 1800f; // Posición fuera de pantalla para finalizar créditos.

    [SerializeField] private string _mainMenuSceneName = "MainMenu"; // Nombre de la escena del menú principal.

    private bool _creditsSkipped; // Indica si los créditos han sido saltados.

    private void Awake()
    {
        _fadeEffect = GetComponentInChildren<Ui_FadeEffect>();
        _fadeEffect.ScreenFade(0, 1); // Efecto de desvanecimiento inicial al aparecer la pantalla.
    }

    // Método Update, se llama una vez por frame.
    private void Update()
    {
        // Desplaza el RectTransform hacia arriba en función de la velocidad de desplazamiento y el tiempo transcurrido.
        _rectT.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Si los créditos alcanzan la posición fuera de pantalla, se llama al método para ir al menú principal.
        if (_rectT.anchoredPosition.y > offScreenPosition)
        {
            GoToMainMenu();
        }
    }

    // Este método es llamado al hacer clic en el botón para saltar los créditos.
    public void SkipCredits()
    {
        // Aumenta la velocidad de desplazamiento si los créditos no han sido saltados previamente.
        if (!_creditsSkipped)
        {
            scrollSpeed *= 10;
            _creditsSkipped = true;
        }
        else
        {
            GoToMainMenu(); // Si ya han sido saltados, ir directamente al menú principal.
        }
    }

    // Activa el efecto de desvanecimiento y cambia a la escena del menú principal.
    private void GoToMainMenu()
    {
       _fadeEffect.ScreenFade(1, 1, SwitchMenuScene);
    }

    // Cambia a la escena del menú principal.
    private void SwitchMenuScene()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }
}