using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que gestiona eventos relacionados con la animación del jugador.
public class PlayerAnimationEvents : MonoBehaviour
{
    // Referencia privada al objeto Player.
    private Player _player;

    private void Awake()
    {
        // Obtiene la referencia al objeto Player que está en el padre del GameObject actual.
        _player = GetComponentInParent<Player>();
    }

    // Método llamado desde un evento de animación, cuando la animación de reaparición (respawn) termina.
    // Esta función es llamada desde el evento "FinishRespawn" en la animación.
    public void FinishRespawn()
    {
        // Llama al método RespawnFinished del jugador, indicando que la reaparición ha terminado.
        _player.RespawnFinished(true);
    }
}