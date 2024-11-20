using UnityEngine;

// Clase que administra la selección de skins en el juego, permitiendo persistir la selección del usuario entre escenas.
public class SkinManager : MonoBehaviour
{
    public static SkinManager instance; // Instancia global de SkinManager para acceder desde otras clases.
    private int _chooseSkinId = 0; // ID de la skin seleccionada actualmente.

    // Método Awake se ejecuta antes del inicio del juego.
    private void Awake()
    {
        // No destruye el objeto SkinManager al cargar nuevas escenas, permitiendo persistencia de datos.
        DontDestroyOnLoad(this.gameObject);

        // Implementa el patrón Singleton para asegurar una sola instancia de SkinManager en el juego.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Destruye cualquier instancia adicional para mantener una única instancia.
        }
    }

    // Método para establecer el ID de la skin seleccionada.
    public void SetSkinId(int id)
    {
        _chooseSkinId = id;
    }

    // Método para obtener el ID de la skin seleccionada.
    public int GetSkinId()
    {
        return _chooseSkinId;
    }
}