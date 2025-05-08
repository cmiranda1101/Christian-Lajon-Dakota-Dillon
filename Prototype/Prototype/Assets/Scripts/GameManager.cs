using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public GameObject weapon;
    public PlayerController playerScript;
    public GunBase weaponScript;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        weapon = GameObject.FindWithTag("Weapon");
        playerScript = player.GetComponent<PlayerController>();
        weaponScript = weapon.GetComponent<GunBase>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
