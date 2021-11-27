using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationSynchronizer : MonoBehaviour
{
    public Camera playerCamera;

    Vector3 yaw;

    private void FixedUpdate()
    {
        yaw.y = playerCamera.transform.eulerAngles.y;
        transform.eulerAngles = yaw;
    }
}
