using UnityEngine;

// Clase para manejar la dificultad del juego.
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance; // Instancia única (singleton).

    public DifficultyType difficulty; // Tipo de dificultad actual.

    private void Awake()
    {
        // Asegura que este objeto no se destruya al cambiar de escena.
        DontDestroyOnLoad(this);

        // Configura el singleton.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Destruye instancias duplicadas.
        }
    }

    // Establece la nueva dificultad y la guarda en PlayerPrefs.
    public void SetDifficulty(DifficultyType newDifficulty)
    {
        difficulty = newDifficulty;
        int difficultyIndex = (int)newDifficulty;
        PlayerPrefs.SetInt(Constants.KEY_DIFFICULTY_INDEX, difficultyIndex);
    }

    // Carga la dificultad desde PlayerPrefs.
    public void LoadDifficulty(int difficultyIndex)
    {
        difficulty = (DifficultyType)difficultyIndex;
    }
}

// Enum que define los tipos de dificultad del juego.
public enum DifficultyType
{
    Easy = 1,   // Fácil.
    Normal,     // Normal.
    Hard        // Difícil.
}
