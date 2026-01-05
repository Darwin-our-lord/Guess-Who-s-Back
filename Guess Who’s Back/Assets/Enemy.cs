using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int rewardValue = 10;

    private float currentHealth;
    private Vector3Int deathPosition;
    private bool hasDied = false;

    private void Awake()
    {
        currentHealth = maxHealth;
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
        deathPosition = GetCurrentGridPosition();
        hasDied = true;

        // TODO: Give player reward
        // TODO: Trigger death event/animation

        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        currentHealth = maxHealth;

        if (hasDied)
        {
            SetGridPosition(deathPosition);
        }

        gameObject.SetActive(true);
    }

    private Vector3Int GetCurrentGridPosition()
    {
        // TODO: Convert world position to grid position using Unity Grid
        return Vector3Int.zero;
    }

    private void SetGridPosition(Vector3Int gridPos)
    {
        // TODO: Convert grid position to world position and set transform
    }

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Speed => speed;
    public int RewardValue => rewardValue;
    public Vector3Int DeathPosition => deathPosition;
    public bool HasDied => hasDied;
}