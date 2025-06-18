using UnityEngine;

public class Bust : MonoBehaviour
{
    [SerializeField] Rigidbody[] rb;
    [SerializeField] int minForce;
    [SerializeField] int maxForce;
    [SerializeField] float destroyTime;

    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Rigidbody rbody in rb) {
            int x = Random.Range(minForce, maxForce);
            int y = Random.Range(minForce, maxForce);
            int z = Random.Range(minForce, maxForce);
            Vector3 randVec = new Vector3(x, y, z);
            rbody.AddForceAtPosition(randVec, Random.onUnitSphere, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= destroyTime) {
            Destroy(gameObject);
        }
    }


}
