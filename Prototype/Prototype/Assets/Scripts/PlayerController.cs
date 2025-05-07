using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject flashlight;

    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int speed;
    [SerializeField] int grabDistance;

    Vector3 moveDirection;

    void Start()
    {

    }

    void Update()
    {
        MovePlayer();

        if (Input.GetButtonDown("Interact")) {
            GrabObject();
        }
    
        if (Input.GetButtonDown("Toggle Flashlight"))
        {
            ToggleFlashlight();
        }
    }

    void MovePlayer()
    {
        moveDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        characterController.Move(moveDirection * speed * Time.deltaTime);
    }

    void ToggleFlashlight()
    {
        if (flashlight.gameObject.activeSelf == true)
        {
            flashlight.SetActive(false);
        }
        else
        {
            flashlight.SetActive(true);
        }
    }

    void GrabObject()
    {
        //Check if something is grabbed
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance, ~ignoreLayer)) {
            Debug.Log(hit.collider.name);
            Interact grab = hit.collider.GetComponent<Interact>();

            if (grab != null) {
                grab.Pickup();
            }
        }
    }
}
