using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int rewardValue = 10;
    [SerializeField] private int waveValue = 1;
    [SerializeField] private int maxWavesAlive = 10;

    [Header("Resistances")]
    [SerializeField][Range(0, 100)] private float knockbackResistance = 0f;

    [Header("--DONT TOUCH--")]
    public Vector3 direction;
    private int wavesAlive = 0;
    private float currentHealth;
    private Vector3 deathPosition;
    private bool hasDied = false;
    private RoadMaker roadMaker;
    private StoreManager storeManager;
    public int roadTargetNr = 0;

    private List<DotEffect> activeDots = new List<DotEffect>();
    private List<SlowEffect> activeSlows = new List<SlowEffect>();
    private float freezeTimer = 0f;
    private float baseSpeed;

    private bool isBeingKnockedBack = false;
    private Vector3 knockbackVelocity = Vector3.zero;
    private float knockbackDecay = 10f;
    private void Awake()
    {
        roadMaker = GameObject.Find("RoadMaker").GetComponent<RoadMaker>();
        storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>();
        currentHealth = maxHealth;
        baseSpeed = speed;
    }

    public void FixedUpdate()
    {
        UpdateStatusEffects(Time.fixedDeltaTime);

        if (isBeingKnockedBack)
        {
            transform.position += knockbackVelocity * Time.fixedDeltaTime;

            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.fixedDeltaTime);

            if (knockbackVelocity.magnitude < 0.1f)
            {
                knockbackVelocity = Vector3.zero;
                isBeingKnockedBack = false;
            }
        }

        float effectiveSpeed = CalculateEffectiveSpeed();

        if (!hasDied && !isBeingKnockedBack && roadTargetNr < roadMaker.roads.Count)
        {
            Vector3 targetPos = roadMaker.roads[roadTargetNr].transform.position;
            direction = (targetPos - transform.position).normalized;
            transform.position += direction * effectiveSpeed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                roadTargetNr++;
                if (roadTargetNr >= roadMaker.roads.Count)
                {
                    MenuManager menuManager = GameObject.Find("UI").GetComponent<MenuManager>();
                    EnemySpawner enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
                    Time.timeScale = 0f;
                    menuManager.loseUI.SetActive(true);
                    menuManager.loseUI.transform.GetChild(1).GetComponent<TMP_Text>().text = "you made it to wave: " + enemySpawner.wave;
                }
            }
        }
    }

    private void UpdateStatusEffects(float deltaTime)
    {
        if (freezeTimer > 0) freezeTimer -= deltaTime;

        for (int i = activeDots.Count - 1; i >= 0; i--)
        {
            activeDots[i].timeSinceLastTick += deltaTime;
            activeDots[i].remainingDuration -= deltaTime;

            if (activeDots[i].timeSinceLastTick >= activeDots[i].tickRate)
            {
                TakeDamage(activeDots[i].damage, true);
                activeDots[i].timeSinceLastTick = 0;
            }

            if (activeDots[i].remainingDuration <= 0)
            {
                activeDots.RemoveAt(i);
            }
        }

        for (int i = activeSlows.Count - 1; i >= 0; i--)
        {
            activeSlows[i].remainingDuration -= deltaTime;
            if (activeSlows[i].remainingDuration <= 0)
            {
                activeSlows.RemoveAt(i);
            }
        }
    }

    private float CalculateEffectiveSpeed()
    {
        if (freezeTimer > 0)
            return 0f;

        float totalSlowPercent = 0f;
        foreach (var slow in activeSlows)
        {
            totalSlowPercent += slow.amount;
        }

        totalSlowPercent = Mathf.Min(totalSlowPercent, 75f);

        return speed * (1f - totalSlowPercent / 100f);
    }

    public void TakeDamage(float damage, bool isDot = false)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(float knockbackDistance, Vector3 knockbackDirection)
    {
        if (hasDied) return;

        float actualKnockback = knockbackDistance * (1f - knockbackResistance / 100f);

        if (actualKnockback <= 0) return;


        float knockbackSpeed = actualKnockback * 5f;
        knockbackVelocity = knockbackDirection.normalized * knockbackSpeed;
        isBeingKnockedBack = true;
    }

    public void ApplyFreeze(float duration)
    {
        if (hasDied) return;

        if (duration > freezeTimer)
        {
            freezeTimer = duration;
        }
    }

    public void ApplySlow(float amount, float duration)
    {
        if (hasDied) return;

        activeSlows.Add(new SlowEffect
        {
            amount = amount,
            remainingDuration = duration
        });
    }

    public void ApplyDot(float damage, float duration, float tickRate, string sourceId)
    {
        if (hasDied) return;

        activeDots.Add(new DotEffect
        {
            damage = damage,
            remainingDuration = duration,
            tickRate = tickRate,
            timeSinceLastTick = 0,
            sourceId = sourceId
        });
    }

    private void Die()
    {
        if (wavesAlive >= maxWavesAlive)
        {
            Destroy(gameObject);
            Destroy(this);
        }

        deathPosition = transform.position;
        hasDied = true;
        gameObject.SetActive(false);

        activeDots.Clear();
        activeSlows.Clear();
        freezeTimer = 0;

        isBeingKnockedBack = false;
        knockbackVelocity = Vector3.zero;
    }

    public void Respawn()
    {
        wavesAlive++;
        currentHealth = maxHealth;
        hasDied = false;
        gameObject.SetActive(true);

        transform.position = deathPosition;

        activeDots.Clear();
        activeSlows.Clear();
        freezeTimer = 0;

        isBeingKnockedBack = false;
        knockbackVelocity = Vector3.zero;
    }

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Speed => speed;
    public int RewardValue => rewardValue;
    public int WaveValue => waveValue;
    public Vector3 DeathPosition => deathPosition;
    public bool HasDied => hasDied;
    public float KnockbackResistance => knockbackResistance;
    public int ActiveDotCount => activeDots.Count;
    public bool IsFrozen => freezeTimer > 0;
}

[System.Serializable]
public class DotEffect
{
    public float damage;
    public float remainingDuration;
    public float tickRate;
    public float timeSinceLastTick;
    public string sourceId;
}

[System.Serializable]
public class SlowEffect
{
    public float amount;
    public float remainingDuration;
}