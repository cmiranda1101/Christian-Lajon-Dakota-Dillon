using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject MainCamera;
    [SerializeField] AudioSource footStepSource; 
    [SerializeField] AudioSource playerHurtSource;
    [SerializeField] AudioClip[] footStepClip;
    [SerializeField] AudioClip[] playerHurtClips;
    [SerializeField] float walkRate;
    float walkTimer;
    float dodgeTimer;
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;

    [SerializeField] public GameObject pistolSpot;
    [SerializeField] public GameObject rifleSpot;

    [SerializeField] public GameObject Holster;

    [SerializeField] public LayerMask ignoreLayer;

    [SerializeField] float speed;
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public int grabDistance;
    [SerializeField] public int money;

    Vector3 moveDirection;

    [SerializeField] GameObject pistolPrefab;
    GameObject flashlight;
    [HideInInspector] public GameObject pistol;
    //Dynamic Creation DO NOT set in Inspector or unhide
    [HideInInspector] public GameObject rifle;
    public GameObject heldWeapon;

    void Start()
    {
        flashlight = GameObject.Find("FlashLight");
        pistol = Instantiate(pistolPrefab, pistolSpot.transform.position, pistolSpot.transform.rotation, pistolSpot.transform);
        heldWeapon = pistol;
        heldWeapon.SetActive(true);
        GameManager.instance.ammoScript.UpdatePistolAmmoAndMagCount();
        dodgeTimer = dodgeCooldown;
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
        if (Input.GetButtonDown("Dodge")) {
            StartCoroutine(Dodge());
        }
    }

    void MovePlayer()
    {
        moveDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        characterController.Move(moveDirection * speed * Time.deltaTime);

        dodgeTimer += Time.deltaTime;
        walkTimer += Time.deltaTime;
        if (walkTimer >= walkRate && characterController.velocity.magnitude > .01f) {
            WalkSound();
            walkTimer = 0f;
        }
    }

    IEnumerator Dodge()
    {
        if(dodgeTimer >= dodgeCooldown) 
        {
            dodgeTimer = 0;
            float originalSpeed = speed;
            speed = dodgeSpeed;
            StartCoroutine(FillCooldownImage());
            yield return new WaitForSeconds(dodgeDuration);
            speed = originalSpeed;
        }
    }

    IEnumerator FillCooldownImage()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dodgeCooldown)
        {
            elapsedTime += Time.deltaTime;
            GameManager.instance.dodgeCooldownRadial.fillAmount = elapsedTime / dodgeCooldown;
            yield return null;
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
            GameManager.instance.ammoScript.UpdatePistolAmmoAndMagCount();
        }
        if (rifle != null && Input.GetKeyDown(KeyCode.Alpha2) && heldWeapon != rifle) {
            heldWeapon.SetActive(false);
            rifle.SetActive(true);
            heldWeapon = rifle;
            GameManager.instance.ammoScript.UpdateRifleAmmoAndMagCount();
        }
    }

    public void Heal(float amount)
    {
        if (currentHP < maxHP) {
            currentHP += amount;

            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            GameManager.instance.healthBar.fillAmount = currentHP / maxHP;

            StartCoroutine(HealScreenFlash());
        }
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
            GameManager.instance.healthBar.fillAmount = currentHP / maxHP;
            
            int i = Random.Range(0, playerHurtClips.Length);
            playerHurtSource.clip = playerHurtClips[i];
            playerHurtSource.Play();
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

    IEnumerator HealScreenFlash()
    {
        GameManager.instance.HealFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.HealFlash.SetActive(false);
    }
}
