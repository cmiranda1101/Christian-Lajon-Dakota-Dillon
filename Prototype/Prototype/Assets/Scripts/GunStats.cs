using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject model;
    public int damage;
    public float fireRate;
    public int range;
    public int currentAmmo;
    public int magSize;
    public int startingMagCount;
    public int magCount;
    public int levelStartCurrentAmmo;
    public int levelStartmagCount;
    public AudioClip[] shootSounds;
    public AudioClip reloadSound1;
    public AudioClip reloadSound2;
}
