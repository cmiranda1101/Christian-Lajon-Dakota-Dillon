using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float xRot = 0f;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

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
