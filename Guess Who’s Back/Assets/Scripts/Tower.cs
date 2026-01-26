using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum TargetType
{
    closest,
    first,
    strongest,
    healthiest,
    flying,
    random
}
public enum Rarities
{
    common,
    uncommon,
    rare,
    epic,
    legendary,
    mytical
}

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 3f;
    [SerializeField] private float rangeAngle = 360f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 1;
    [SerializeField] private int cost = 50;
    [SerializeField] private string overrideDesc = "";
    [SerializeField] private GameObject weaponVisual;

    [Header("Special Stats - Knockback")]
    [SerializeField] private bool hasKnockback = false;
    [SerializeField] private float knockbackDistance = 0f;

    [Header("Special Stats - Freeze")]
    [SerializeField] private bool hasFreeze = false;
    [SerializeField][Range(0, 100)] private float freezeChance = 0f;
    [SerializeField] private float freezeDuration = 0f;

    [Header("Special Stats - Slow")]
    [SerializeField] private bool hasSlow = false;
    [SerializeField][Range(0, 100)] private float slowAmount = 0f;
    [SerializeField] private float slowDuration = 0f;

    [Header("Special Stats - DOT")]
    [SerializeField] private bool hasDot = false;
    [SerializeField] private float dotDamage = 0f;
    [SerializeField] private float dotDuration = 0f;
    [SerializeField] private float dotTickRate = 0.5f;

    [Header("Special Stats - AOE")]
    [SerializeField] private bool hasAoe = false;
    [SerializeField] private float aoeRadius = 0f;
    [SerializeField] private GameObject AoeCircle;
    [SerializeField][Range(0, 100)] private float aoeDamageFalloff = 100f;

    private GameObject bulletParent;
    private Vector3Int gridPosition;
    private float lastFireTime;
    private Enemy currentTarget;

    [Header("other")]
    public TargetType targetType = TargetType.first;
    public GameObject bulletPrefab;

    void Awake()
    {
        bulletParent = GameObject.Find("Bullets");
    }

    private void Update()
    {
        if (Time.time >= lastFireTime + (fireRate))
        {
            AcquireTarget();

            if (currentTarget != null)
            {
                Fire();
            }
        }
    }
    public void ResetFireRateTimer()
    {
        lastFireTime = 0;
    }
    private void AcquireTarget()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        List<Enemy> enemiesInRange = new List<Enemy>();

        float coneAngle = rangeAngle;

        foreach (Enemy enemy in allEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            Vector2 directionToEnemy = (enemy.transform.position - transform.position);
            float distance = directionToEnemy.magnitude;

            if (distance <= range)
            {
                float angle = Vector2.Angle(transform.right, directionToEnemy);

                if (angle <= coneAngle / 2f)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        currentTarget = null;
        if (targetType == TargetType.closest)
        {
            float minDistance = float.MaxValue;

            foreach (Enemy enemy in enemiesInRange)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentTarget = enemy;
                }
            }
        }
        else if (targetType == TargetType.first)
        {
            Enemy currentFirstEnemy = null;

            foreach (Enemy enemy in enemiesInRange)
            {
                if (enemy.roadTarget == null)
                    continue;

                if (currentFirstEnemy == null)
                {
                    currentFirstEnemy = enemy;
                    continue;
                }
                
                if (enemy.roadTargetNr > currentFirstEnemy.roadTargetNr)
                {
                    currentFirstEnemy = enemy;
                }
                else if (enemy.walkType == WalkType.flying)
                {
                    float enemyDistance = Vector3.Distance(enemy.transform.position, enemy.roadTarget.position);
                    float currentFirstDistance = Vector3.Distance(currentFirstEnemy.transform.position, currentFirstEnemy.roadTarget.position);

                    if (enemyDistance < currentFirstDistance) currentFirstEnemy = enemy;
                }
            }

            currentTarget = currentFirstEnemy;
        }
        else if (targetType == TargetType.flying)
        {
            Enemy currentFirstEnemy = null;
            bool flyingInRange = false;

            foreach (Enemy enemy in enemiesInRange)
            {
                if (enemy.walkType == WalkType.flying)
                {
                    flyingInRange = true;
                    if (currentFirstEnemy == null)
                    {
                        currentFirstEnemy = enemy;
                        continue;
                    }
                    float enemyDistance = Vector3.Distance(enemy.transform.position, enemy.roadTarget.position);
                    float currentFirstDistance = Vector3.Distance(currentFirstEnemy.transform.position, currentFirstEnemy.roadTarget.position);

                    if (enemyDistance < currentFirstDistance) currentFirstEnemy = enemy;
                }
            }
            if (flyingInRange == false)
            {
                foreach (Enemy enemy in enemiesInRange)
                {
                    if (enemy.roadTarget == null)
                        continue;

                    if (currentFirstEnemy == null)
                    {
                        currentFirstEnemy = enemy;
                        continue;
                    }

                    if (enemy.roadTargetNr > currentFirstEnemy.roadTargetNr)
                    {
                        currentFirstEnemy = enemy;
                    }
                }
            }
            flyingInRange = false;
            currentTarget = currentFirstEnemy;
        }
        else if (targetType == TargetType.strongest)
        {
            Enemy currentStrongestEnemy = null;

            foreach (Enemy enemy in enemiesInRange)
            {
                if (currentStrongestEnemy == null) currentStrongestEnemy = enemy;
                if (currentStrongestEnemy.MaxHealth < enemy.MaxHealth)
                {
                    currentStrongestEnemy = enemy;
                }
            }

            currentTarget = currentStrongestEnemy;
        }
        else if (targetType == TargetType.healthiest)
        {
            Enemy currentHealthiestEnemy = null;

            foreach (Enemy enemy in enemiesInRange)
            {
                if (currentHealthiestEnemy == null) currentHealthiestEnemy = enemy;
                if (currentHealthiestEnemy.CurrentHealth < enemy.CurrentHealth)
                {
                    currentHealthiestEnemy = enemy;
                }
            }

            currentTarget = currentHealthiestEnemy;
        }
        else if (targetType == TargetType.random)
        {
            if (enemiesInRange.Count > 0)
            {
                Enemy RandomEnemy = enemiesInRange[UnityEngine.Random.Range(0, enemiesInRange.Count)];
                currentTarget = RandomEnemy;
            }
        }
    }

    private void Fire()
    {
        if (currentTarget == null) return;

        lastFireTime = Time.time;

        if (Settings.ShowBullets)
        {
            Vector2 targetVel = currentTarget.gameObject.GetComponent<Enemy>().Speed * currentTarget.gameObject.GetComponent<Enemy>().direction;

            float interceptTime = CalculateInterceptTime(transform.position, currentTarget.transform.position, targetVel, bulletSpeed);

            Vector2 interceptPoint = (Vector2)currentTarget.transform.position + (targetVel * interceptTime);
            Vector2 directionToIntercept = interceptPoint - (Vector2)transform.position;
            float newAngle = Mathf.Atan2(directionToIntercept.y, directionToIntercept.x) * Mathf.Rad2Deg;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, newAngle), bulletParent.transform);

            if (weaponVisual != null) weaponVisual.transform.rotation = Quaternion.Euler(0, 0, newAngle + 90);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.speed = bulletSpeed;
            float lifetime = (transform.position - currentTarget.transform.position).magnitude / bulletSpeed;
            bulletScript.lifeTime = lifetime;
            bulletScript.target = interceptPoint;
            bulletScript.targetGameObj = currentTarget.gameObject;

            bulletScript.sourceTower = this;

            StartCoroutine(DamageTimer(lifetime, currentTarget));
        }
        else
        {
            float newAngle = Mathf.Atan2(currentTarget.gameObject.transform.position.y, currentTarget.gameObject.transform.position.x) * Mathf.Rad2Deg;
            if (weaponVisual != null) weaponVisual.transform.rotation = Quaternion.Euler(0, 0, newAngle + 90);
            StartCoroutine(DamageTimer(0.5f, currentTarget));
        }
    }

    IEnumerator DamageTimer(float lifetime, Enemy target)
    {
        yield return new WaitForSeconds(lifetime);

        if (target != null && !target.HasDied)
        {
            ApplyDamageAndEffects(target);
        }
    }

    public void ApplyDamageAndEffects(Enemy target)
    {
        if (target == null || target.HasDied) return;

        target.TakeDamage(damage);

        if (hasKnockback && knockbackDistance > 0)
        {
            Vector3 knockbackDir = (target.transform.position - transform.position).normalized;
            target.ApplyKnockback(knockbackDistance, knockbackDir);
        }

        if (hasFreeze && UnityEngine.Random.Range(0f, 100f) < freezeChance)
        {
            target.ApplyFreeze(freezeDuration);
        }

        if (hasSlow)
        {
            target.ApplySlow(slowAmount, slowDuration);
        }

        if (hasDot)
        {
            string dotSourceId = gameObject.GetInstanceID().ToString();
            target.ApplyDot(dotDamage, dotDuration, dotTickRate, dotSourceId);
        }

        if (hasAoe && aoeRadius > 0)
        {
            StartCoroutine(AOECircleSpawn(target.transform.position));

            ApplyAoeDamage(target.transform.position);
        }
    }

    private IEnumerator AOECircleSpawn(Vector3 impactPosition)
    {
        GameObject cloe = Instantiate(AoeCircle);
        cloe.transform.position = impactPosition;
        cloe.GetComponent<SpriteRenderer>().sortingOrder = 500;
        cloe.transform.localScale = new Vector3(aoeRadius, aoeRadius, 1);
        yield return new WaitForSeconds(0.3f);
        Destroy(cloe);
    }
    private void ApplyAoeDamage(Vector3 impactPosition)
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in allEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy || enemy.HasDied) continue;
            if (enemy == currentTarget) continue;

            float distance = Vector3.Distance(impactPosition, enemy.transform.position);

            if (distance <= aoeRadius)
            {
                float damageMultiplier = CalculateAoeDamageMultiplier(distance);
                float aoeDamage = damage * damageMultiplier;

                enemy.TakeDamage(aoeDamage);

                if (hasKnockback && knockbackDistance > 0)
                {
                    Vector3 knockbackDir = (enemy.transform.position - impactPosition).normalized;
                    enemy.ApplyKnockback(knockbackDistance * damageMultiplier, knockbackDir);
                }

                if (hasFreeze && UnityEngine.Random.Range(0f, 100f) < freezeChance)
                {
                    enemy.ApplyFreeze(freezeDuration);
                }

                if (hasSlow)
                {
                    enemy.ApplySlow(slowAmount * damageMultiplier, slowDuration);
                }

                if (hasDot)
                {
                    string dotSourceId = gameObject.GetInstanceID().ToString();
                    enemy.ApplyDot(dotDamage * damageMultiplier, dotDuration, dotTickRate, dotSourceId);
                }
            }
        }
    }

    private float CalculateAoeDamageMultiplier(float distance)
    {
        if (distance >= aoeRadius) return 0f;

        float falloffMultiplier = aoeDamageFalloff / 100f;
        float distanceRatio = distance / aoeRadius;

        return 1f - (distanceRatio * (1f - falloffMultiplier));
    }

    float CalculateInterceptTime(Vector2 shooterPos, Vector2 targetPos, Vector2 targetVelocity, float bulletSpeed)
    {
        Vector2 relativePosition = targetPos - shooterPos;

        float a = targetVelocity.sqrMagnitude - (bulletSpeed * bulletSpeed);
        float b = 2f * Vector2.Dot(relativePosition, targetVelocity);
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
        if (overrideDesc != "")
            return overrideDesc;

        string desc = $"Cost: {cost}\nDmg: {damage}\nRange: {range}\nRate: {fireRate}";

        if (hasKnockback) desc += $"\nKnockback: {knockbackDistance}";
        if (hasFreeze) desc += $"\nFreeze: {freezeChance}% ({freezeDuration}s)";
        if (hasSlow) desc += $"\nSlow: {slowAmount}% ({slowDuration}s)";
        if (hasDot) desc += $"\nDOT: {dotDamage}/tick ({dotDuration}s)";
        if (hasAoe) desc += $"\nAOE: {aoeRadius} radius";

        return desc;
    }

    public void ChangeTargetType()
    {
        int count = System.Enum.GetValues(typeof(TargetType)).Length;
        int nextIndex = ((int)targetType + 1) % count;
        targetType = (TargetType)nextIndex;
    }

    public float Damage => damage;
    public float Range => range;
    public float FireRate => fireRate;
    public int Cost => cost;
    public Vector3Int GridPosition => gridPosition;
    public bool HasAoe => hasAoe;
    public float AoeRadius => aoeRadius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        if (hasAoe && aoeRadius > 0)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
}