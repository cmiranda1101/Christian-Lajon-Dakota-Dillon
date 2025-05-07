using UnityEngine;
using UnityEngine.UI;

// Assign to hotbar slots in the UI, this script will handle the weapon toggling

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject equippedWeapon;
    [SerializeField] KeyCode hotkey;

    void Update()
    {
        // Checks if the hotkey is pressed and toggles the weapon
        if (Input.GetKeyDown(hotkey))
        {
            ToggleWeapon();
        }
    }

    // Toggles the weapon on and off
    public void ToggleWeapon()
    {
        if (equippedWeapon == null)
        {
            equippedWeapon = Instantiate(weaponPrefab);
            equippedWeapon.transform.SetParent(player.transform);
            equippedWeapon.transform.localPosition = new Vector3(0.573f, 0.576f, 0.455f);
            equippedWeapon.transform.localRotation = Quaternion.identity;

        }
        else
        {
            Destroy(equippedWeapon);
            equippedWeapon = null;
        }
    }
}
