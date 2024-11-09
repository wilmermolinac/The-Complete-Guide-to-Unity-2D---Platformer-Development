using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// La clase EnemyPlant hereda de Enemy, y añade comportamiento específico de disparo.
public class EnemyPlant : Enemy
{
    [Header("Plant details")] 
    [SerializeField] private Enemy_Bullet _bulletPrefab; // Prefab del proyectil disparado por EnemyPlant.
    [SerializeField] private Transform _gunPoint;       // Punto de origen desde donde se dispara el proyectil.
    [SerializeField] private float _bulletSpeed = 7;    // Velocidad del proyectil.
    [SerializeField] private float _attackCoolDown = 1.5f; // Tiempo de enfriamiento entre ataques.
    private float _lastTimeAttacked;                    // Registro del tiempo del último ataque.

    // Método Update, verifica si el enemigo puede atacar.
    protected override void Update()
    {
        base.Update();
        
        // Determina si ha pasado el tiempo suficiente para atacar de nuevo.
        bool canAttack = Time.time > _lastTimeAttacked + _attackCoolDown;
        
        // Si el jugador es detectado y puede atacar, inicia el ataque.
        if (isPlayerDetected && canAttack)
        {
            Attack();
        }
    }

    // Inicia la animación y registra el tiempo de ataque.
    private void Attack() 
    {
        _lastTimeAttacked = Time.time; // Actualiza el tiempo del último ataque.
        anim.SetTrigger("attack");     // Activa la animación de ataque.
    }

    // Este método se llama desde un evento de animación (enemyPlantAttack) para crear el proyectil.
    private void CreateBullet()
    {
        // Instancia el proyectil en la posición del arma.
        Enemy_Bullet newBullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.identity);
        Vector2 newBulletSpeed = new Vector2(_bulletSpeed * facingDirection, 0); // Asigna la velocidad en la dirección adecuada.
        newBullet.SetVelocity(newBulletSpeed);

        // Destruye el proyectil después de 10 segundos si no ha colisionado.
        Destroy(newBullet.gameObject, 10);
    }

    // Sobrescribe HandleAnimation, dejándolo vacío ya que no es necesario para EnemyPlant.
    protected override void HandleAnimation()
    {
        // No ejecuta la lógica base, ya que no necesita esta animación.
    }
}