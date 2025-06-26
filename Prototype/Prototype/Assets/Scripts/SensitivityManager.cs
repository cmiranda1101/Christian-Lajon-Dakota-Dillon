using UnityEngine;
using UnityEngine.UI;

public class SensitivityManager : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] float defaultSensitivity;

    public static float Sensitivity { get; private set; }

    void Start()
    {
        // Load saved sensitivity
        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);

        if (sensitivitySlider != null) {
            sensitivitySlider.value = Sensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
        //else {
        //    Debug.LogWarning("Sensitivity slider not assigned in SettingsManager.");
        //}
    }

    void OnSensitivityChanged(float newSensitivity)
    {
        Sensitivity = newSensitivity;
        PlayerPrefs.SetFloat("Sensitivity", newSensitivity);
        PlayerPrefs.Save();
    }
}

