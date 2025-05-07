using UnityEngine;

public class EnemyGun : GunBase
{
    [SerializeField] Transform target;
    [SerializeField] float shootInterval;
    float shootTimer;

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (target != null && shootTimer >= shootInterval)
        {
            // Face the target
            transform.LookAt(target.position);

            // Fire at the target
            Fire();
            shootTimer = 0f;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
