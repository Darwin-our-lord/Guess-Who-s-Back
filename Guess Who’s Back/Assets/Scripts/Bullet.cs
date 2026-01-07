using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime;
    public float speed;
    public Vector3 target;
    public GameObject targetGameObj;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (targetGameObj == null) return;

        if (!targetGameObj.activeSelf || targetGameObj == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += -(transform.position-target).normalized * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 0.01f) Destroy(gameObject);
        
    }

}
