using System.Collections;
using UnityEngine;

public class GhostEnemy : Enemy
{

    [Header("Special Abilities")]
    [SerializeField] [Range(0f, 100f)] private float ghostlyResistance = 50f;
    [SerializeField] GameObject missText;

    public override void TakeDamage(float damage, bool isDot = false)
    {
        int resistanceCheck = Random.Range(0, 100);
        if(resistanceCheck < ghostlyResistance)
        {
            StartCoroutine(ShowMissText());
            return;
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator ShowMissText()
    {
        GameObject instantiatedMissText = 
            Instantiate(missText, transform.position + new Vector3(0, 1 + Random.Range(-0.1f, 0.1f), 0.3f+Random.Range(-0.1f, 0.1f)), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Destroy(instantiatedMissText);
    }
}
