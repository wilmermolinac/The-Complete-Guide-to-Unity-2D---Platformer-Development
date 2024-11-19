using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Clase que controla el efecto de desvanecimiento en la interfaz de usuario.
public class Ui_FadeEffect : MonoBehaviour
{
    private Image _fadeImage; // Imagen usada para el efecto de desvanecimiento.

    private void Awake()
    {
        _fadeImage = GetComponent<Image>();
    }

    // Método para iniciar el efecto de desvanecimiento.
    public void ScreenFade(float targetAlpha, float duration, System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(targetAlpha, duration, onComplete));
    }

    // Corrutina que realiza el cambio de opacidad gradualmente.
    private IEnumerator FadeCoroutine(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0f;
        Color currentColor = _fadeImage.color; // Color actual de la imagen.

        float startAlpha = _fadeImage.color.a; // Opacidad inicial.

        // Interpolación de la opacidad desde el valor inicial hasta el valor objetivo.
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            _fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        _fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        onComplete?.Invoke(); // Llama a la función de finalización si está definida.
    }
}
