using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController characterController;

    [SerializeField] public Transform pistolSpot;
    [SerializeField] public Transform rifleSpot;
    [SerializeField] public Transform Holster;

    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int speed;
    [SerializeField] int grabDistance;
    [SerializeField] int HP;
    [SerializeField] public int money;

    Vector3 moveDirection;

    GameObject flashlight;
    GameObject pistol;
    GameObject rifle;
    GameObject heldWeapon;

    void Start()
    {
        flashlight = GameObject.Find("FlashLight");
        pistol = transform.GetChild(1).gameObject;
        //rifle = transform.GetChild(2).gameObject;
        heldWeapon = pistol;
    }

    void Update()
    {
        MovePlayer();
        SwapWeapons();

        if (Input.GetButtonDown("Toggle Flashlight"))
        {
            ToggleFlashlight();
        }
        if (Input.GetButtonDown("Interact"))
        {
            GrabObject();
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            IInteract grab = hit.collider.GetComponentInParent<IInteract>();

            if (grab != null)
            {
                grab.Interact();
            }
        }
    }

    void SwapWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && heldWeapon != pistol)
        {
            heldWeapon.SetActive(false);
            pistol.SetActive(true);
            heldWeapon = pistol;
        }
        //if (Input.GetKeyDown(KeyCode.Alpha2) && heldWeapon != rifle)
        //{
        //    heldWeapon.SetActive(false);
        //    rifle.SetActive(true);
        //    heldWeapon = rifle;
        //}
    }

    public void Heal(int amount)
    {
        HP += amount;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        //Need to check for death
        if (HP <= 0) {
            //TODO uncomment below when we have a lose screen
            GameManager.instance.YouLose();
        }
    }
}
