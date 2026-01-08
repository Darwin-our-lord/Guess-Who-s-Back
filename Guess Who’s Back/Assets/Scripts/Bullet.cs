using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 2f;
    public Vector2 target;
    public GameObject targetGameObj;
    public Tower sourceTower; // Reference to the tower that fired this bullet

    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifeTime)
        {
            // Apply effects on impact
            if (targetGameObj != null && sourceTower != null)
            {
                Enemy enemy = targetGameObj.GetComponent<Enemy>();
                if (enemy != null && !enemy.HasDied)
                {
                    sourceTower.ApplyDamageAndEffects(enemy);
                }
            }

            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Optional: Make bullet explode on contact
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && !enemy.HasDied && sourceTower != null)
            {
                sourceTower.ApplyDamageAndEffects(enemy);
            }

            Destroy(gameObject);
        }
    }
}