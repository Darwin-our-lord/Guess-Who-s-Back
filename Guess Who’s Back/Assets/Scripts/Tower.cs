
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 3f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 1;
    [SerializeField] private int cost = 50;
    [SerializeField] private string overrideDesc = "";
    [SerializeField] private GameObject weaponVisual;

    private GameObject bulletParent;
    private Vector3Int gridPosition;
    private float lastFireTime;
    private Enemy currentTarget;


    public GameObject bulletPrefab;
     void Awake()
    {
        bulletParent = GameObject.Find("Bullets");
    }
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

            float distance = UnityEngine.Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= range)
            {
                enemiesInRange.Add(enemy);
            }
        }

        currentTarget = null;
        float maxDistance = 0f;

        foreach (Enemy enemy in enemiesInRange)
        {
            float distance = UnityEngine.Vector3.Distance(transform.position, enemy.transform.position);

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

        UnityEngine.Vector2 targetVel = currentTarget.gameObject.GetComponent<Enemy>().Speed* currentTarget.gameObject.GetComponent<Enemy>().direction;

        float interceptTime = CalculateInterceptTime(transform.position, currentTarget.transform.position, targetVel, bulletSpeed);

        UnityEngine.Vector2 interceptPoint = (UnityEngine.Vector2)currentTarget.transform.position + (targetVel * interceptTime);
        UnityEngine.Vector2 directionToIntercept = interceptPoint - (UnityEngine.Vector2)transform.position;
        float newAngle = Mathf.Atan2(directionToIntercept.y, directionToIntercept.x) * Mathf.Rad2Deg;

        if(weaponVisual != null) weaponVisual.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, newAngle+90);

        GameObject bullet = Instantiate(bulletPrefab, transform.position, UnityEngine.Quaternion.Euler(0, 0, newAngle),bulletParent.transform);

        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().lifeTime =  (transform.position-currentTarget.transform.position).magnitude / bulletSpeed;
        bullet.GetComponent<Bullet>().target = interceptPoint;
        bullet.GetComponent<Bullet>().targetGameObj = currentTarget.gameObject;

        StartCoroutine(DamageTimer((transform.position - currentTarget.transform.position).magnitude / bulletSpeed));
        
    }
    IEnumerator DamageTimer(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        currentTarget.TakeDamage(damage);
    }
    float CalculateInterceptTime(UnityEngine.Vector2 shooterPos, UnityEngine.Vector2 targetPos, UnityEngine.Vector2 targetVelocity, float bulletSpeed)
    {
        UnityEngine.Vector2 relativePosition = targetPos - shooterPos;

        float a = targetVelocity.sqrMagnitude - (bulletSpeed * bulletSpeed);
        float b = 2f * UnityEngine.Vector2.Dot(relativePosition, targetVelocity);
        float c = relativePosition.sqrMagnitude;

        if (Mathf.Abs(a) < 0.0001f)
        {

            return relativePosition.magnitude / bulletSpeed;
        }

        float determinant = b * b - 4f * a * c;

        if (determinant > 0)
        {
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
            float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

            if (t1 > 0 && t2 > 0) return Mathf.Min(t1, t2);
            if (t1 > 0) return t1;
            if (t2 > 0) return t2;
        }

        return relativePosition.magnitude / bulletSpeed; 
    }

    public string GetDescription()
    {
        if (overrideDesc == "") return $"cost: {cost} \ndmg: {damage} \nrange: {range} \nrate: {fireRate}";
        else return overrideDesc;
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