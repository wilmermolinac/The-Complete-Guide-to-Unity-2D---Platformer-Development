using System;
using Unity.Cinemachine;
using UnityEngine;

// Clase que controla una cámara específica del nivel, 
// permitiendo activarla y asignar un nuevo objetivo.
public class LevelCamera : MonoBehaviour
{
    private CinemachineCamera _cinemachine; // Referencia a la cámara de Cinemachine.

    // Método Awake se ejecuta al iniciar el script.
    private void Awake()
    {
        // Obtiene la referencia a la cámara de Cinemachine desde los hijos de este objeto.
        _cinemachine = GetComponentInChildren<CinemachineCamera>(true);

        // Desactiva la cámara al inicio para que no esté activa por defecto.
        EnableCamera(false);
    }

    // Método para activar o desactivar la cámara.
    public void EnableCamera(bool enable)
    {
        _cinemachine.gameObject.SetActive(enable); // Activa o desactiva el GameObject de la cámara.
    }

    // Método para establecer un nuevo objetivo para la cámara.
    public void SetNewTarget(Transform newTarget)
    {
        _cinemachine.Follow = newTarget; // Asigna el nuevo objetivo al componente de Cinemachine.
    }
}
