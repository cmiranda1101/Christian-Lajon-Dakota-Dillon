using UnityEngine;

public class Target : MonoBehaviour, IDamage
{
    public void takeDamage(int amount)
    {
        Destroy(gameObject);
    }
}
