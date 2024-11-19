using Core.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Clase para configurar los botones de selección de niveles.
public class Ui_LevelButton : MonoBehaviour
{
    // Referencias a los textos que mostrarán información del nivel.
    [SerializeField] private TextMeshProUGUI _levelNameText;
    [SerializeField] private TextMeshProUGUI _bestTimeText;
    [SerializeField] private TextMeshProUGUI _fruitText;

    private int _levelIndex; // Índice del nivel.
    public string sceneName; // Nombre de la escena del nivel.

    // Configura el botón con la información de un nivel.
    public void SetupButton(LevelModel levelModel)
    {
        _levelIndex = levelModel.SceneIndex;
        _levelNameText.text = $"Level {levelModel.SceneIndex}"; // Nombre del nivel.
        _bestTimeText.text = $"Best Time: {levelModel.BestTime.ToString("00.00")}"; // Mejor tiempo.
        _fruitText.text = $"Fruits: {FruitsInfoText()}"; // Frutas recolectadas.
        sceneName = levelModel.Name; // Nombre de la escena.
    }

    // Obtiene la información de las frutas recolectadas y totales.
    private string FruitsInfoText()
    {
        int totalFruitsLevel = PlayerPrefs.GetInt(Constants.KEY_TOTAL_FRUITS_LEVEL + _levelIndex, 0);
        int fruitCollected = PlayerPrefs.GetInt(Constants.KEY_FRUIT_COLLECTED_LEVEL + _levelIndex, 0);

        // Si no hay frutas totales definidas, muestra "?".
        return totalFruitsLevel == 0 ? $"{fruitCollected}/?" : $"{fruitCollected}/{totalFruitsLevel}";
    }

    // Carga la escena del nivel al hacer clic en el botón.
    public void LoadLevel()
    {
        // Guarda el nivel de dificultad actual.
        int difficultyIndex = (int)DifficultyManager.instance.difficulty;
        PlayerPrefs.SetInt(Constants.KEY_DIFFICULTY_INDEX, difficultyIndex);

        // Carga la escena correspondiente.
        SceneManager.LoadScene(sceneName);
    }
}
