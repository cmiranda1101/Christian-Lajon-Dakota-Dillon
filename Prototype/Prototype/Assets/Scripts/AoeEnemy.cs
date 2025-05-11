using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AoeEnemy : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] float explosionDelay;
    [SerializeField] float explosionDamage;
    [SerializeField] float alertFlashSpeed;
    [SerializeField] int facePlayerSpeed;

    [SerializeField] Color alertColor1;
    [SerializeField] Color alertColor2;


    Color originalColor;
    bool isAlerted;
    bool isExploding;

    Transform player;
    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = model.material.color;
        player = GameManager.instance.player.transform;
        GameManager.instance.UpdateGameGoal(1);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || isExploding) return;
        agent.SetDestination(player.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FacePlayer();
        }
        
    }
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    public void takeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            Explode();
        }
        else
        {
            StartCoroutine(FlashRed());
            agent.SetDestination(player.position);
        }
    }
   public void OnPlayerTriggerEnter(TriggerType type, Collider other)
    {
        switch (type)
        {
            case TriggerType.Detection:
                agent.SetDestination(player.position);
                break;

            case TriggerType.Alert:
                if(!isAlerted)
                {
                    isAlerted = true;
                    StartCoroutine(FlashRed());
                    Invoke(nameof(Explode), explosionDelay);
                }
                break;
        }
    }
    public void OnPlayerInExplosionRange(Collider other)
    {
        if (!isExploding) return;

        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.takeDamage((int)explosionDamage);
        }
    }
    
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = originalColor;
    }
    IEnumerator AlertFlash()
    {
        model.material.color = alertColor1;
        yield return new WaitForSeconds(alertFlashSpeed);
        model.material.color = alertColor2;
        yield return new WaitForSeconds(alertFlashSpeed);
    }
    void Explode()
    {
        if (isExploding) return;

        isExploding = true;
        Debug.Log("Enemy EXPLODED!!!");
        Collider[] hits = Physics.OverlapSphere(transform.position, GetComponentInChildren<SphereCollider>().radius);

        foreach (Collider hit in hits)
        {
            if(hit.CompareTag("Player"))
            {
                IDamage damageable = hit.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.takeDamage((int)explosionDamage);
                    Debug.Log("Player took explosion damage");
                }
            }
        }

        GameManager.instance.UpdateGameGoal(-1);
        Destroy(gameObject);
    }

}
