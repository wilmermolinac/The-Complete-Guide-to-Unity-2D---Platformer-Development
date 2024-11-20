using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Clase que gestiona los ajustes de audio en el juego, incluyendo la música de fondo (BGM) y efectos de sonido (SFX).
/// Permite cambiar el volumen de ambos mediante sliders, y guarda/carga los valores usando PlayerPrefs.
/// </summary>
public class Ui_Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer; // Mezclador de audio que controla los volúmenes de SFX y BGM.
    [SerializeField] private float _mixerMultiplier = 25; // Multiplicador para ajustar la escala del valor en el mezclador.

    [Header("SFX Settings")] 
    [SerializeField] private Slider _sfxSlider; // Control deslizante para ajustar el volumen de los SFX.
    [SerializeField] private TextMeshProUGUI _sfxSliderText; // Texto que muestra el porcentaje de volumen de los SFX.
    [SerializeField] private string _sfxParameter; // Nombre del parámetro del mezclador para los SFX.

    [Header("BGM Settings")]
    [SerializeField] private Slider _bgmSlider; // Control deslizante para ajustar el volumen de la música de fondo (BGM).
    [SerializeField] private TextMeshProUGUI _bgmSliderText; // Texto que muestra el porcentaje de volumen de la BGM.
    [SerializeField] private string _bgmParameter; // Nombre del parámetro del mezclador para la BGM.

    /// <summary>
    /// Método llamado cuando se mueve el slider de SFX.
    /// Actualiza el volumen en el mezclador y el texto del slider.
    /// </summary>
    public void SfxSliderValue(float value)
    {
        // Actualiza el texto del slider con el porcentaje redondeado.
        _sfxSliderText.text = $"{Mathf.RoundToInt(value * 100)}%";

        // Convierte el valor del slider a escala logarítmica y lo ajusta en el mezclador.
        float newValue = Mathf.Log10(value) * _mixerMultiplier;
        _audioMixer.SetFloat(_sfxParameter, newValue);
    }

    /// <summary>
    /// Método llamado cuando se mueve el slider de BGM.
    /// Actualiza el volumen en el mezclador y el texto del slider.
    /// </summary>
    public void BgmSliderValue(float value)
    {
        // Actualiza el texto del slider con el porcentaje redondeado.
        _bgmSliderText.text = $"{Mathf.RoundToInt(value * 100)}%";

        // Convierte el valor del slider a escala logarítmica y lo ajusta en el mezclador.
        float newValue = Mathf.Log10(value) * _mixerMultiplier;
        _audioMixer.SetFloat(_bgmParameter, newValue);
    }

    /// <summary>
    /// Método llamado cuando el objeto se desactiva.
    /// Guarda los valores actuales de los sliders en PlayerPrefs.
    /// </summary>
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(Constants.KEY_SFXPARAMETER_VALUE, _sfxSlider.value);
        PlayerPrefs.SetFloat(Constants.KEY_BGMPARAMETER_VALUE, _bgmSlider.value);
    }

    /// <summary>
    /// Método llamado cuando el objeto se activa.
    /// Carga los valores guardados de los sliders desde PlayerPrefs.
    /// </summary>
    private void OnEnable()
    {
        // Carga el valor guardado del slider de SFX o asigna un valor por defecto.
        float sfxValue = PlayerPrefs.GetFloat(Constants.KEY_SFXPARAMETER_VALUE, 0.7f);
        _sfxSlider.value = sfxValue;

        // Carga el valor guardado del slider de BGM o asigna un valor por defecto.
        float bgmValue = PlayerPrefs.GetFloat(Constants.KEY_BGMPARAMETER_VALUE, 0.7f);
        _bgmSlider.value = bgmValue;
    }
}