using UnityEngine;

public class ResetCamera : MonoBehaviour
{
    private ManualCameraController controller;

    void Awake()
    {
        controller = FindObjectOfType<ManualCameraController>();
    }
    public void ResetCam()
    {     
        controller.Reset();
    }
}

