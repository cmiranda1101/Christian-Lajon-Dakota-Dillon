using UnityEngine;

public class TutorialDoors : MonoBehaviour
{
    enum objectiveType { pickup, kill, goTo, buttonInput};

    [SerializeField] objectiveType type;
    [SerializeField] GameObject[] objectives;
    [SerializeField] string buttonInputString;

    bool objectiveCompleted;
    bool inInputZone = false;
    private void Update()
    {
        if (type == objectiveType.pickup || type == objectiveType.kill)
        {
            objectiveCompleted = CheckObjectivesList();
        }
        else if (type == objectiveType.buttonInput && inInputZone)
        {
            objectiveCompleted = CheckInput();
        }

        if (objectiveCompleted)
        {
            OpenDoor();
        }
    }

    bool CheckObjectivesList()
    {
        bool allObjectivesCompleted = true;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].gameObject != null)
            {
                allObjectivesCompleted = false;
            }
        }
        return allObjectivesCompleted;
    }
    
    bool CheckInput()
    {
        bool wasButtonPressed = false;
        if (Input.GetButtonDown(buttonInputString))
        {
            wasButtonPressed = true;
        }
        return wasButtonPressed;
    }

    void OpenDoor()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inInputZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inInputZone = false;
        }
    }
}
