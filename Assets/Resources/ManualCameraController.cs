using UnityEngine;
using UnityEngine.EventSystems;

public class ManualCameraController : MonoBehaviour
{
    private float X;
    private float Y;

    public float xRotation;
    public float yRotation;

    public float cameraRotationSpeed = 5.0f;
    public float cameraPanSensitivity = 0.7f;
    public float cameraZoomSensitivity = 40.0f;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -180F;
    public float maximumY = 180F;

    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private float originalFov;

    private bool reset = false;

    void Start()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.position;
        originalFov = Camera.main.fieldOfView;
    }

    void Update()
    {
        if (reset == true)
        {
            xRotation = 0;
            yRotation = 0;
            ResetCam();   
            reset = false;
        }
     
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (Camera.main.fieldOfView > 1)
                {
                    Camera.main.fieldOfView -= cameraZoomSensitivity;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (Camera.main.fieldOfView < 100)
                {
                    Camera.main.fieldOfView += cameraZoomSensitivity;
                }
            }

            if (Input.GetMouseButton(1))
            {
                var mouseX = Input.GetAxis("Mouse X");
                var mouseY = Input.GetAxis("Mouse Y");

                var cameraPos = transform.position;
                cameraPos += cameraPanSensitivity * -mouseX * transform.right;
                cameraPos += cameraPanSensitivity * -mouseY * transform.up;
                transform.position = cameraPos;
            }

            if (Input.GetMouseButton(0))
            {                      
                X = Input.GetAxis("Mouse X") * cameraRotationSpeed;
                Y = Input.GetAxis("Mouse Y") * cameraRotationSpeed;

                xRotation += X;
                yRotation += Y;           

                xRotation = ClampAngle(xRotation, minimumX, maximumX);
                yRotation = ClampAngle(yRotation, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(xRotation, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(yRotation, Vector3.left);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void ResetCam()
    {   
        transform.position = originalPosition;
        transform.localRotation = originalRotation;    
        Camera.main.fieldOfView = originalFov;
    }

    public void Reset()
    {
        reset = true;
    }
}

