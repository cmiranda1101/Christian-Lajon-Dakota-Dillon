using UnityEngine;

public class MovingWalls : MonoBehaviour
{
    [SerializeField] GameObject[] appearingWalls;
    [SerializeField] GameObject[] disappearingWalls;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < appearingWalls.Length; i++)
            {
                appearingWalls[i].SetActive(true);
            }
            for (int i = 0; i < disappearingWalls.Length; i++)
            {
                disappearingWalls[i].SetActive(false);
            }
        }
    }
}
