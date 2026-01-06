using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 3f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int cost = 50;

    private Vector3Int gridPosition;
    private float lastFireTime;
    private Enemy currentTarget;

    private void Update()
    {
        if (Time.time >= lastFireTime + (1f / fireRate))
        {
            AcquireTarget();

            if (currentTarget != null)
            {
                Fire();
            }
        }
    }

    private void AcquireTarget()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in allEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= range)
            {
                enemiesInRange.Add(enemy);
            }
        }

        currentTarget = null;
        float maxDistance = 0f;

        foreach (Enemy enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                currentTarget = enemy;
            }
        }
    }

    private void Fire()
    {
        if (currentTarget == null) return;

        currentTarget.TakeDamage(damage);
        lastFireTime = Time.time;

        // TODO: Spawn projectile/bullet
        // TODO: Play fire animation/sound
    }

    public void SetGridPosition(Vector3Int gridPos)
    {
        gridPosition = gridPos;
        // TODO: Convert grid position to world position
    }

    public float Damage => damage;
    public float Range => range;
    public float FireRate => fireRate;
    public int Cost => cost;
    public Vector3Int GridPosition => gridPosition;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}