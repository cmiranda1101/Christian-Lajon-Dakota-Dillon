using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject MainCamera;

    [SerializeField] public GameObject pistolSpot;
    [SerializeField] public GameObject rifleSpot;
    [SerializeField] public GameObject Holster;

    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int speed;
    [SerializeField] public int grabDistance;
    [SerializeField] int HP;
    [SerializeField] public int money;

    Vector3 moveDirection;

    [SerializeField] GameObject pistolPrefab;
    GameObject flashlight;
    GameObject pistol;
    public GameObject rifle;
    GameObject heldWeapon;

    void Start()
    {
        flashlight = GameObject.Find("FlashLight");
        pistol = Instantiate(pistolPrefab, pistolSpot.transform.position, pistolSpot.transform.rotation, pistolSpot.transform);
        heldWeapon = pistol;
        heldWeapon.SetActive(true);
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
        if (Input.GetKeyDown(KeyCode.Alpha2) && heldWeapon != rifle)
        {
            if(rifle == null)
            {
                return;
            }
            heldWeapon.SetActive(false);
            rifle.SetActive(true);
            heldWeapon = rifle;
        }
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
