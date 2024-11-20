using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Estructura que representa las propiedades de un skin.
[Serializable]
public struct Skin
{
    public string skinName;    // Nombre del skin.
    public int skinPrice;      // Precio del skin en la moneda del juego.
    public bool unlocked;      // Indica si el skin está desbloqueado o no.
}

// Clase que gestiona la selección de skins en la interfaz de usuario.
public class Ui_SkinSelection : MonoBehaviour
{
    private Ui_LevelSelection _ui_LevelSelection; // Referencia a la clase de selección de niveles.
    private Ui_MainMenu _ui_MainMenu;             // Referencia al menú principal.

    [SerializeField] private Skin[] skinList;     // Lista de skins disponibles.

    [Header("UI Details")]
    [SerializeField] private Animator _skinDisplay; // Animador encargado de mostrar las skins.

    private int _maxIndex = 0; // Índice máximo de skins disponibles.
    private int _skinIndex = 0; // Índice del skin actualmente seleccionado.

    [Header("UI Text Settings")]
    [SerializeField] private TextMeshProUGUI _bankText;    // Texto que muestra la cantidad de monedas/frutas.
    [SerializeField] private TextMeshProUGUI _skinNameText; // Texto que muestra el nombre del skin seleccionado.
    [SerializeField] private TextMeshProUGUI _priceText;    // Texto que muestra el precio del skin seleccionado.
    [SerializeField] private TextMeshProUGUI _buySelectText; // Texto del botón que indica si se puede comprar o seleccionar el skin.

    // Inicializa valores básicos y actualiza el banco de monedas al cargar el script.
    private void Awake()
    {
        _maxIndex = _skinDisplay.layerCount - 1; // Calcula el número total de skins basándose en las capas del animador.
        _bankText.text = $"Bank: {GetFruitInBank()}"; // Muestra la cantidad de monedas/frutas disponibles.
    }

    // Configura referencias y actualiza la interfaz de usuario al iniciar.
    private void Start()
    {
        _ui_MainMenu = GetComponentInParent<Ui_MainMenu>();
        _ui_LevelSelection = _ui_MainMenu.GetComponentInChildren<Ui_LevelSelection>(true); // Obtiene el componente incluso si está deshabilitado.
        
        LoadSkinsUnlock();    // Carga el estado de desbloqueo de los skins.
        UpdateSkinDisplay();  // Actualiza la visualización del skin seleccionado.
    }

    // Carga el estado de desbloqueo de los skins desde PlayerPrefs.
    private void LoadSkinsUnlock()
    {
        for (int i = 0; i < skinList.Length; i++)
        {
            bool isUnlocked = PlayerPrefs.GetInt(Constants.KEY_UNLOCKED_SKIN_INDEX + i, 0) == 1;
            skinList[i].unlocked = (i == 0) || isUnlocked; // El primer skin está desbloqueado por defecto.
        }
    }

    // Selecciona el skin actual si está desbloqueado, o lo compra si no lo está.
    public void SelectedSkin()
    {
        if (skinList[_skinIndex].unlocked)
        {
            SkinManager.instance.SetSkinId(_skinIndex); // Guarda el skin seleccionado.
            _ui_MainMenu.SwitchUI(_ui_LevelSelection.gameObject); // Cambia a la interfaz de selección de nivel.
        }
        else
        {
            BuySkin(_skinIndex); // Intenta comprar el skin.
        }
        
        UpdateSkinDisplay(); // Actualiza la interfaz después de realizar una acción.
        AudioManager.instance.PlaySfx(4);
    }

    // Avanza al siguiente skin en la lista.
    public void NextSkin()
    {
        _skinIndex = (_skinIndex + 1) % (_maxIndex + 1); // Incrementa y reinicia al llegar al máximo.
        UpdateSkinDisplay(); // Actualiza la interfaz para reflejar el cambio.
        AudioManager.instance.PlaySfx(4);
    }

    // Retrocede al skin anterior en la lista.
    public void PreviousSkin()
    {
        _skinIndex = (_skinIndex - 1 + (_maxIndex + 1)) % (_maxIndex + 1); // Decrementa y reinicia al llegar al mínimo.
        UpdateSkinDisplay(); // Actualiza la interfaz para reflejar el cambio.
        AudioManager.instance.PlaySfx(4);
    }

    // Actualiza la interfaz para mostrar la información del skin seleccionado.
    private void UpdateSkinDisplay()
    {
        _bankText.text = $"Bank: {GetFruitInBank()}"; // Muestra la cantidad de monedas/frutas disponibles.

        // Desactiva todas las capas del animador.
        for (int i = 0; i < _skinDisplay.layerCount; i++)
        {
            _skinDisplay.SetLayerWeight(i, 0);
        }

        // Activa solo la capa correspondiente al skin seleccionado.
        _skinDisplay.SetLayerWeight(_skinIndex, 1);
        _skinNameText.text = skinList[_skinIndex].skinName;

        if (skinList[_skinIndex].unlocked)
        {
            _priceText.transform.parent.gameObject.SetActive(false); // Oculta el precio si está desbloqueado.
            _buySelectText.text = "Select"; // Cambia el texto del botón a "Seleccionar".
        }
        else
        {
            _priceText.transform.parent.gameObject.SetActive(true); // Muestra el precio si no está desbloqueado.
            _priceText.text = $"Price: {skinList[_skinIndex].skinPrice}"; // Actualiza el texto del precio.
            _buySelectText.text = "Buy"; // Cambia el texto del botón a "Comprar".
        }
    }

    // Método para comprar un skin si el jugador tiene suficientes monedas.
    private void BuySkin(int skinIndex)
    {
        int skinPrice = skinList[skinIndex].skinPrice;
        if (BuySkinTransaction(skinPrice))
        {
            skinList[skinIndex].unlocked = true; // Marca el skin como desbloqueado.
            PlayerPrefs.SetInt(Constants.KEY_UNLOCKED_SKIN_INDEX + skinIndex, 1); // Guarda el estado en PlayerPrefs.
        }
        AudioManager.instance.PlaySfx(4);
    }

    // Obtiene la cantidad de monedas/frutas disponibles del jugador.
    private int GetFruitInBank()
    {
        return PlayerPrefs.GetInt(Constants.KEY_TOTAL_FRUITS_AMOUNT, 0);
    }

    // Realiza una transacción para comprar un skin.
    private bool BuySkinTransaction(int skinPrice)
    {
        if (GetFruitInBank() >= skinPrice)
        {
            AudioManager.instance.PlaySfx(10, false);
            PlayerPrefs.SetInt(Constants.KEY_TOTAL_FRUITS_AMOUNT, GetFruitInBank() - skinPrice); // Descuenta el precio.
            return true; // Transacción exitosa.
        }
        else
        {
            AudioManager.instance.PlaySfx(6, false);
            return false;// Transacción fallida por falta de monedas.
        }
       
    }
}