using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject MainCamera;
    [SerializeField] AudioSource footStepSource; 
    [SerializeField] AudioClip[] footStepClip;
    [SerializeField] float walkRate;
    float walkTimer;

    [SerializeField] public GameObject pistolSpot;
    [SerializeField] public GameObject rifleSpot;

    [SerializeField] public GameObject Holster;

    [SerializeField] public LayerMask ignoreLayer;

    [SerializeField] int speed;
    [SerializeField] public int maxHP;
    [SerializeField] public int grabDistance;
    [SerializeField] public int currentHP;
    [SerializeField] public int money;

    Vector3 moveDirection;

    [SerializeField] GameObject pistolPrefab;
    GameObject flashlight;
    [HideInInspector] public GameObject pistol;
    //Dynamic Creation DO NOT set in Inspector or unhide
    [HideInInspector] public GameObject rifle;
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
        if (Input.GetButtonDown("Toggle Flashlight")) {
            ToggleFlashlight();
        }
        if (Input.GetButtonDown("Interact")) {
            GrabObject();
        }
    }

    void MovePlayer()
    {
        moveDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        characterController.Move(moveDirection * speed * Time.deltaTime);

        walkTimer += Time.deltaTime;
        if (walkTimer >= walkRate && characterController.velocity.magnitude > .01f) {
            WalkSound();
            walkTimer = 0f;
        }
    }


    void ToggleFlashlight()
    {
        if (flashlight.gameObject.activeSelf == true) {
            flashlight.SetActive(false);
        }
        else {
            flashlight.SetActive(true);
        }
    }

    void GrabObject()
    {
        //Check if something is grabbed
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance, ~ignoreLayer)) {
            Debug.Log(hit.collider.name);
            IInteract grab = hit.collider.GetComponentInParent<IInteract>();

            if (grab != null) {
                grab.Interact();
            }
        }
    }
    void SwapWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && heldWeapon != pistol) {
            heldWeapon.SetActive(false);
            pistol.SetActive(true);
            heldWeapon = pistol;
        }
        if (rifle != null && Input.GetKeyDown(KeyCode.Alpha2) && heldWeapon != rifle) {
            heldWeapon.SetActive(false);
            rifle.SetActive(true);
            heldWeapon = rifle;
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        GameManager.instance.healthBar.fillAmount = (float)currentHP / maxHP;
    }

    public void takeDamage(int amount)
    {
        //Flash damage screen
        StartCoroutine(DamageScreenFlash());

        //lower HP
        currentHP = Mathf.Clamp(currentHP -= amount, 0, maxHP);

        //Need to check for death
        if (currentHP <= 0) {
            GameManager.instance.healthUI.SetActive(false);
            GameManager.instance.YouLose();
        }
        //Scale HP Bar
        else {
            float scale = currentHP / maxHP;
            GameManager.instance.healthBar.fillAmount = (float)currentHP / maxHP;
        }
    }

    public IEnumerator MuzzleFlash()
    {
        heldWeapon.transform.Find("MuzzleFlash").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        heldWeapon.transform.Find("MuzzleFlash").gameObject.SetActive(false);
    }

    void WalkSound()
    {
        int i = Random.Range(0, footStepClip.Length);
        footStepSource.clip = footStepClip[i];
        footStepSource.Play();
    }

    IEnumerator DamageScreenFlash()
    {
        GameManager.instance.DamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.DamageFlash.SetActive(false);
    }
}
