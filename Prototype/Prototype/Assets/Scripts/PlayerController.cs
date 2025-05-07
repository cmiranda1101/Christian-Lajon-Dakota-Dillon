using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject flashlight;

    [SerializeField] int speed;

    Vector3 moveDirection;

    void Start()
    {
        
    }

    void Update()
    {
        MovePlayer();
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
}
