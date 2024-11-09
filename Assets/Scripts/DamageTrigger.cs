using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Este método se llama automáticamente cuando otro objeto con un Collider2D entra en contacto
        // con el Collider2D del GameObject al que está asociado este script.

        // En el código comentado, primero se verifica si el objeto con el que colisionamos tiene el tag "Player".
        /*if (collision.tag == "Player")
        {
            // Si el objeto tiene el tag "Player", muestra un mensaje en la consola indicando que el jugador
            // ha entrado en la colisión con el trigger.
            Debug.Log("Player entered trigger collision");

            // Accede a la instancia del jugador a través del GameManager y llama al método Knockback()
            // para aplicar un retroceso o empuje al jugador.
            GameManager.instance.player.Knockback();
        }*/

        // Alternativa sin utilizar "tags":

        // Intenta obtener el componente "Player" del objeto con el que hemos colisionado.
        // GetComponent<Player>() devuelve el componente "Player" si el objeto lo tiene,
        // de lo contrario, devuelve null.
        Player player = collision.gameObject.GetComponent<Player>();

        // Si el componente Player no es null, significa que el objeto con el que colisionamos es el jugador.
        if (player != null)
        {
            // Llama al método Knockback() del jugador para aplicar un efecto de retroceso.
            // y pasamos la posicion actual del game object asignado a este script
            player.Knockback(transform.position.x);
        }
        
        // Tambien lo podemos hacer asi mas simplificado
        // si el Player no es nulo aplicamos el Knockback()
        //player?.Knockback();
    }
}