using UnityEngine;

public class TutorialDoors : MonoBehaviour
{
    enum objectiveType { pickup, kill, goTo};

    [SerializeField] objectiveType type;
    [SerializeField] GameObject[] objectives;

    bool objectiveCompleted;
    private void Update()
    {
        if (type == objectiveType.pickup || type == objectiveType.kill)
        {
            objectiveCompleted = CheckObjectivesList();
            if (objectiveCompleted)
            {
                OpenDoor();
            }
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

    void OpenDoor()
    {
        Destroy(gameObject);
    }
}
