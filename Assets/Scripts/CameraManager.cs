using Unity.Cinemachine;
using UnityEngine;

// Clase para gestionar efectos de cámara, como sacudidas.
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance; // Instancia única para uso global.

    [Header("Screen Shake")]
    [SerializeField] private Vector2 _shakeVelocity; // Velocidad del efecto de sacudida.

    private CinemachineImpulseSource _impulseSource; // Fuente de impulsos para generar efectos.

    private void Awake()
    {
        instance = this; // Asigna esta instancia como única.
        _impulseSource = GetComponent<CinemachineImpulseSource>(); // Obtiene el componente de impulso.
    }

    // Genera un efecto de sacudida de pantalla con dirección ajustable.
    public void ScreenShake(float shakeDirection)
    {
        _impulseSource.DefaultVelocity = new Vector2(_shakeVelocity.x * shakeDirection, _shakeVelocity.y);
        _impulseSource.GenerateImpulse(); // Genera el impulso para la sacudida.
    }
}