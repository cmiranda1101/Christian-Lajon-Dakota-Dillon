using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float sensitivity;
    float xRot = 0f;
    void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 250f);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        RotateCamera();
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        //Debug.Log(mouseX);
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (invertY == true)
        {
            xRot += mouseY;
        }
        else
        {
            xRot -= mouseY;
        }
        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
