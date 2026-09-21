using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public enum WalkType
{
    flying,
    normal
}
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected string displayName = "Enemy";
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int waveValue = 1;
    [SerializeField] protected int maxWavesAlive = 10;
    public int waveReq = 0;
    public WalkType walkType = WalkType.normal;

    [Header("Wave Scaling")]
    [SerializeField] protected int scalingStartWave = 10;
    [SerializeField] protected float hpGrowthPerWave = 0.03f;
    protected float baseMaxHealth;
    protected EnemySpawner enemySpawner;

    [Header("Resistances")]
    [SerializeField][Range(0, 100)] protected float knockbackResistance = 0f;

    [Header("CorpseStuff")]
    [SerializeField] protected GameObject corpsePrefab;
    [SerializeField] protected GameObject corpseParent;
    protected GameObject corpse;

    [Header("HealthBar")]
    [SerializeField] protected GameObject healthBarPrefab;
    protected EnemyHealthBar healthBarInstance;
    protected bool isMouseOver = false;

    [Header("--DONT TOUCH--")]
    public Vector3 direction;

    protected int wavesAlive = 0;
    protected float currentHealth;
    protected Vector3 deathPosition;
    protected bool hasDied = false;
    protected RoadMaker roadMaker;
    protected StoreManager storeManager;

    public int roadTargetNr = 0;

    protected List<DotEffect> activeDots = new List<DotEffect>();
    protected List<SlowEffect> activeSlows = new List<SlowEffect>();
    protected float freezeTimer = 0f;
    protected bool canFreeze = true;
    protected float baseSpeed;

    protected bool isBeingKnockedBack = false;
    protected Vector3 knockbackVelocity = Vector3.zero;
    protected float knockbackDecay = 10f;

    public Transform roadTarget;

    public bool soonToDie = false;

    protected void Awake()
    {
        roadMaker = GameObject.Find("RoadMaker").GetComponent<RoadMaker>();
        storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>();
        corpseParent = GameObject.Find("corpses");
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        baseSpeed = speed;

        baseMaxHealth = maxHealth;
        ApplyWaveScaling();

        if (healthBarPrefab != null)
        {
            GameObject canvasObj = GameObject.Find("UI");
            if (canvasObj != null)
            {
                Canvas canvas = canvasObj.GetComponent<Canvas>();
                if (canvas != null)
                {
                    GameObject healthBarObj = Instantiate(healthBarPrefab, canvas.transform);
                    healthBarInstance = healthBarObj.GetComponent<EnemyHealthBar>();
                    if (healthBarInstance != null)
                    {
                        healthBarInstance.Initialize(this);
                        healthBarInstance.Hide();
                    }
                }
            }
        }
    }

    protected void ApplyWaveScaling()
    {
        int currentWave = enemySpawner != null ? enemySpawner.wave : 1;
        int wavesPastStart = Mathf.Max(0, currentWave - scalingStartWave);
        float multiplier = Mathf.Pow(1f + hpGrowthPerWave, wavesPastStart);
        maxHealth = baseMaxHealth * multiplier;
        currentHealth = maxHealth;
    }

    protected void OnDestroy()
    {
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }
    }
    public void OnMouseOver()
    {
        healthBarInstance.Show();
    }
    public void OnMouseExit()
    {
        healthBarInstance.Hide();
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

        if (!hasDied && !isBeingKnockedBack)
        {
            if (walkType == WalkType.normal)
            {
                if (roadTarget == null) roadTarget = roadMaker.firstRoad.transform;

                direction = (roadTarget.position - transform.position).normalized;
                transform.position += direction * effectiveSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.position, roadTarget.position) < 0.05f)
                {
                    roadTargetNr++;

                    if (roadTarget.GetComponent<Road>().nextTiles.Count == 0)
                    {
                        TriggerGameOver();
                    }
                    else
                    {
                        roadTarget = roadTarget.GetComponent<Road>().nextTiles[UnityEngine.Random.Range(0, roadTarget.GetComponent<Road>().nextTiles.Count)];
                    }
                }
            }
            else if (walkType == WalkType.flying)
            {
                if (roadTarget == null) roadTarget = roadMaker.branchFronts[UnityEngine.Random.Range(0, roadMaker.branchFronts.Count)].transform;
                for (int i = 0; i < roadMaker.branchFronts.Count; i++)
                {
                    if (roadTarget == null)
                    {
                        roadTarget = roadMaker.branchFronts[i].transform;
                        continue;
                    }
                    if (Vector3.Distance(roadMaker.branchFronts[i].transform.position, transform.position) < Vector3.Distance(roadTarget.position, transform.position))
                    {
                        roadTarget = roadMaker.branchFronts[i].transform;
                    }
                }

                direction = (roadTarget.position - transform.position).normalized;
                transform.position += direction * effectiveSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.position, roadTarget.position) < 0.05f)
                {
                    TriggerGameOver();
                }
            }
        }
    }
    protected void TriggerGameOver()
    {
        MenuManager menuManager = GameObject.Find("UI").GetComponent<MenuManager>();

        if (LeaderboardClient.Instance != null)
        {
            LeaderboardClient.Instance.SubmitScore(enemySpawner.wave, displayName);
        }

        Time.timeScale = 0f;
        menuManager.loseUI.SetActive(true);
        menuManager.loseUI.transform.GetChild(1).GetComponent<TMP_Text>().text = "you made it to wave: " + enemySpawner.wave;
    }

    protected void UpdateStatusEffects(float deltaTime)
    {
        if (freezeTimer > 0) freezeTimer -= deltaTime;
        if (freezeTimer <= 0) StartCoroutine(AllowFreeze());
        
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

    protected float CalculateEffectiveSpeed()
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

    public virtual void TakeDamage(float damage, bool isDot = false)
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
        if (!canFreeze) return;

        if (duration > freezeTimer)
        {
            freezeTimer = duration;
            canFreeze = false;
        }
    }
    protected IEnumerator AllowFreeze()
    {
        yield return new WaitForSeconds(1f);
        canFreeze = true;
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

    protected virtual void Die()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.Hide();
        }

        if (wavesAlive >= maxWavesAlive)
        {
            Destroy(corpse);
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        corpse = Instantiate(corpsePrefab, transform.position, Quaternion.identity, corpseParent.transform);

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
        Destroy(corpse);
        corpse = null;

        soonToDie = false;

        if (walkType == WalkType.flying) roadTarget = roadMaker.branchFronts[UnityEngine.Random.Range(0, roadMaker.branchFronts.Count)].transform;

        wavesAlive++;
        ApplyWaveScaling();
        hasDied = false;
        gameObject.SetActive(true);

        transform.position = deathPosition;

        activeDots.Clear();
        activeSlows.Clear();
        freezeTimer = 0;

        isBeingKnockedBack = false;
        knockbackVelocity = Vector3.zero;

        if (healthBarInstance != null && isMouseOver)
        {
            healthBarInstance.Show();
        }
    }

    public string DisplayName => displayName;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Speed => speed;
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
    public float timeSinceLastTick = 0;
    public string sourceId;
}

[System.Serializable]
public class SlowEffect
{
    public float amount;
    public float remainingDuration;
}