using System;
using UnityEngine;

// Clase que detecta la entrada y salida del jugador en un área específica 
// para controlar el comportamiento de la cámara en ese nivel.
public class LevelCameraTrigger : MonoBehaviour
{
    private LevelCamera _levelCamera; // Referencia a la cámara del nivel.

    // Método Awake se ejecuta al iniciar el script.
    private void Awake()
    {
        // Obtiene la referencia a la cámara del nivel desde el componente padre.
        _levelCamera = GetComponentInParent<LevelCamera>();
    }

    // Método llamado cuando un objeto entra en el área del trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que colisionó es el jugador.
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            // Activa la cámara del nivel.
            _levelCamera.EnableCamera(true);

            // Establece al jugador como el nuevo objetivo de la cámara.
            _levelCamera.SetNewTarget(player.transform);
        }
    }

    // Método llamado cuando un objeto sale del área del trigger.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verifica si el objeto que colisionó es el jugador.
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            // Desactiva la cámara del nivel.
            _levelCamera.EnableCamera(false);
        }
    }
}
