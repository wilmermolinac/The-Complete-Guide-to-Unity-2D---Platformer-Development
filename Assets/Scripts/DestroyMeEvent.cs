using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase DestroyMeEvent que controla el comportamiento de los efectos visuales de UN GameObject
public class DestroyMeEvent : MonoBehaviour
{
   // Método público que destruye el GameObject actual (el objeto que tiene este script).
   public void DestroyMe()
   {
      // Elimina el objeto de la escena, liberando memoria y recursos asociados.
      Destroy(gameObject);
   }
}
