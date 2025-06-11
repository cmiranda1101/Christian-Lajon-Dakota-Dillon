using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Threading;
using Unity.Collections;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject MainCamera;
    [SerializeField] AudioSource footStepSource;
    [SerializeField] AudioSource playerHurtSource;
    [SerializeField] AudioClip[] footStepClip;
    [SerializeField] AudioClip[] playerHurtClips;
    [SerializeField] float walkRate;
    float walkRateOG;
    float walkTimer;
    float dodgeTimer;
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;

    [SerializeField] float cameraSmoothness;
    [SerializeField] float crouchSpeed;
    [SerializeField] float sprintSpeed;

    [SerializeField] public GameObject Holster;

    [SerializeField] public LayerMask ignoreLayer;

    [SerializeField] public Animator anim;
    [SerializeField] float animTransSpeed;

    [SerializeField] float speed;
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public float maxStamina;
    [SerializeField] public float currentStamina;
    [SerializeField] public float staminaDrain;
    [SerializeField] public float staminaRegenDelay;
    [SerializeField] public int grabDistance;
    [SerializeField] public int money;

    float speedOG;

    [SerializeField] Transform headLocal;
    Vector3 camPosOG;

    Vector3 moveDirection;

    GameObject flashlight;
    public ThrowConsumable throwConsumable;

    float animationCurr;
    float controllerSpeedCurr;

    public bool isHiding;

    void Start()
    {
        flashlight = GameObject.Find("FlashLight");
        dodgeTimer = dodgeCooldown;
        GameManager.instance.moneyScript.UpdateMoneyText();
        camPosOG = MainCamera.transform.localPosition;

        speedOG = speed;
        walkRateOG = walkRate;
    }
    void Update()
    {
        MovePlayer();
        FollowHead();
        if (Input.GetButtonDown("Toggle Flashlight")) {
            ToggleFlashlight();
        }
        if (Input.GetButtonDown("Interact")) {
            GrabObject();
        }
        if (Input.GetButtonDown("Dodge")) {
            StartCoroutine(Dodge());
        }
        if (Input.GetButtonDown("Crouch")) {
            Crouch();
        }
        if (Input.GetButtonDown("Sprint"))
        {
            Sprint();
        }

        SetAnimParameter();
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

    void SetAnimParameter()
    {
        controllerSpeedCurr = characterController.velocity.normalized.magnitude;
        animationCurr = anim.GetFloat("Speed");

        if (!anim.GetBool("isCrouching")) {
            anim.SetFloat("Speed", Mathf.Lerp(animationCurr, controllerSpeedCurr, Time.deltaTime * animTransSpeed));
        }
        else
            anim.SetFloat("Speed", Mathf.Lerp(Mathf.Clamp(animationCurr,0,crouchSpeed), controllerSpeedCurr, Time.deltaTime * animTransSpeed));
    }

    IEnumerator Dodge()
    {
        if (dodgeTimer >= dodgeCooldown) {
            dodgeTimer = 0;
            float originalSpeed = speed;
            speed = dodgeSpeed;
            StartCoroutine(FillCooldownImage());
            yield return new WaitForSeconds(dodgeDuration);
            speed = originalSpeed;
        }
    }

    void Sprint()
    {
        if (characterController.velocity.magnitude > .1f){
            bool sprinting = anim.GetBool("isSprinting");
            anim.SetBool("isSprinting", !sprinting);

            if (anim.GetBool("isSprinting")) {
                speed = speed * sprintSpeed;
                StartCoroutine(Stamina());
            }
            else {
                speed = speedOG;
            }
        }
    }

    IEnumerator Stamina()
    {
        while (anim.GetBool("isSprinting"))
        {
            if (currentStamina <= 0)
            {
                anim.SetBool("isSprinting", false);
                speed = speedOG;
            }
            currentStamina = Mathf.Clamp(currentStamina -= staminaDrain, 0, maxStamina);
            GameManager.instance.staminaBar.fillAmount = currentStamina / maxStamina;
            yield return null;
        }
        yield return new WaitForSeconds(staminaRegenDelay);

        while (anim.GetBool("isSprinting") == false && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Clamp(currentStamina += staminaDrain, 0, maxStamina);
            GameManager.instance.staminaBar.fillAmount = currentStamina / maxStamina;
            yield return null;
        }
    }

    public void Crouch()
    {
        if (!isHiding) {
            bool crouching = anim.GetBool("isCrouching");
            anim.SetBool("isCrouching", !crouching);

            if (anim.GetBool("isCrouching")) {
                speed = speed * crouchSpeed;
                walkRate = walkRate * 2;
                characterController.height = .2f;
                characterController.center = new Vector3(0, .4f, 0);
            }
            else {
                speed = speedOG;
                walkRate = walkRateOG;
                characterController.height = 2f;
                characterController.center = new Vector3(0, 1, 0);
            }
        }
    }

    void FollowHead()
    {
            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, headLocal.position, cameraSmoothness);
    }

    IEnumerator FillCooldownImage()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dodgeCooldown) {
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
        //heldWeapon.transform.Find("MuzzleFlash").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        //heldWeapon.transform.Find("MuzzleFlash").gameObject.SetActive(false);
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
