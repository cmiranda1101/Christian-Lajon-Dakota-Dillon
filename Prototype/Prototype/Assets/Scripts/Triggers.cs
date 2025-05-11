using UnityEngine;

public enum TriggerType { Detection, Alert, Explosion}

public class Triggers : MonoBehaviour
{
    public TriggerType triggerType;
    public AoeEnemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.OnPlayerTriggerEnter(triggerType, other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (triggerType == TriggerType.Explosion && other.CompareTag("Player"))
        {
            enemy.OnPlayerInExplosionRange(other);
        }
    }

}
