using System.Collections;
using UnityEngine;

public class ThrowConsumable : MonoBehaviour
{
    public GameObject molotovPrefab;
    public GameObject chemlightPrefab;
    public Transform throwPoint;
    public float throwForce;
    public int chemlightCount;
    public int chemlightDuration;

    void Start()
    {
        GameManager.instance.chemlightCounter.text = chemlightCount.ToString();
    }
    public void ThrowChemlight()
    {
        if(chemlightCount > 0)
        {
            GameObject chemlight = Instantiate(chemlightPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = chemlight.GetComponent<Rigidbody>();
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            StartCoroutine(DestroyChemlight(chemlight));
            chemlightCount--;
            GameManager.instance.chemlightCounter.text = chemlightCount.ToString();
        }
    }

    IEnumerator DestroyChemlight(GameObject chemlight)
    {
        yield return new WaitForSeconds(chemlightDuration);
        Destroy(chemlight);
    }

    public void ThrowMolotov()
    {
        if (GameManager.instance.playerScript.molotovCount > 0)
        {
            GameObject molotov = Instantiate(molotovPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = molotov.GetComponent<Rigidbody>();
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            GameManager.instance.playerScript.molotovCount--;
        }
    }
}
