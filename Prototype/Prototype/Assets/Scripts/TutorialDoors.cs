using UnityEditor.SearchService;
using UnityEngine;

public class TutorialDoors : MonoBehaviour
{
    enum objectiveType { pickup, kill, goTo, buttonInput, instantiateObj};

    [SerializeField] objectiveType type;
    [SerializeField] GameObject[] objectives;
    [SerializeField] GameObject tutorialItem;
    [SerializeField] string buttonInputString;
    [SerializeField] float moveSpeed;

    string tutorialItemName;

    bool objectiveCompleted;
    bool inInputZone = false;

    Vector3 startingPos;

    private void Start()
    {
        startingPos = transform.position;

        if (tutorialItem != null) tutorialItemName = tutorialItem.name;
    }
    private void Update()
    {
        if (!objectiveCompleted)
        {
            if (type == objectiveType.pickup || type == objectiveType.kill)
            {
                objectiveCompleted = CheckObjectivesList();
            }
            else if (type == objectiveType.buttonInput && inInputZone)
            {
                objectiveCompleted = CheckInput();
            }
            else if(type == objectiveType.instantiateObj) 
            {
                objectiveCompleted = WasInstantiated();
            }

        }
        else
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

    bool WasInstantiated()
    {
        if (GameObject.Find(tutorialItemName + "(Clone)")) {
            return true;
        }
        else return false;
    }

    void OpenDoor()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingPos.x + 15, startingPos.y, startingPos.z), moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == objectiveType.goTo)
            {
                objectiveCompleted = true;
            }
            else
            {
                inInputZone = true;
            }
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
