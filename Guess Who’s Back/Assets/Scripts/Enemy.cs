using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int rewardValue = 10;
    [SerializeField] private int waveValue = 1;

    private float currentHealth;
    private Vector3 deathPosition;
    private bool hasDied = false;
    private RoadMaker roadMaker;

    private int roadTargetNr = 0;

    private void Awake()
    {
        roadMaker = GameObject.Find("RoadMaker").GetComponent<RoadMaker>();
        currentHealth = maxHealth;
    }
    public void FixedUpdate()
    {
        if (!hasDied && roadTargetNr < roadMaker.roads.Count)
        {
            Vector3 targetPos = roadMaker.roads[roadTargetNr].transform.position;
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                roadTargetNr++;

                if (roadTargetNr >= roadMaker.roads.Count)
                {
                    MenuManager menuManager = GameObject.Find("UI").GetComponent<MenuManager>();
                    Time.timeScale = 0f;
                    menuManager.loseUI.SetActive(true);
                }
            }
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        deathPosition = transform.position;
        hasDied = true;

        // TODO: Give player reward
        // TODO: Trigger death event/animation

        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        hasDied = false;

        gameObject.SetActive(true);

    }

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Speed => speed;
    public int RewardValue => rewardValue;
    public int WaveValue => waveValue;
    public Vector3 DeathPosition => deathPosition;
    public bool HasDied => hasDied;

}