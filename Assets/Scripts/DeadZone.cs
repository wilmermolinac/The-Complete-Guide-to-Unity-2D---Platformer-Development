using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Clase que maneja la interacción con una zona de muerte (DeadZone).
public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Este método se llama automáticamente cuando otro objeto con un Collider2D entra en contacto
        // con el Collider2D del objeto al que está adjunto este script (la zona de muerte).

        // Intenta obtener el componente Player del objeto que ha colisionado.
        Player player = collision.gameObject.GetComponent<Player>();

        // Intenta obtener el componente Enemy del objeto que ha colisionado.
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        // Si el objeto que colisionó es un jugador (es decir, el componente Player no es nulo).
        if (player != null)
        {
            player.Damage();
            // Llama al método Die del jugador para que el jugador muera.
            //player.Die();

            // Pide al GameManager que vuelva a instanciar (reaparecer) un nuevo jugador.
            GameManager.instance.RespawnPlayer();
        }

        if (enemy != null)
        {
            // Si colisono con el con un Objeto Enemy
            // El objeto muere
            enemy.Die();
        }
    }
}