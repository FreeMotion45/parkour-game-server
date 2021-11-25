using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MouseLookOld : MonoBehaviour
{
    public float mouseSensitivity;

    private float verticalRotation;
    private float horizontalRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float verticalMouseMovement = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float horizontalMouseMovement = Input.GetAxis("Mouse X") * mouseSensitivity;

        verticalMouseMovement *= -1; // The mouse Y axis is inverted.

        verticalRotation = Mathf.Clamp(verticalRotation + verticalMouseMovement, -90, 90);
        horizontalRotation += horizontalMouseMovement;

        transform.eulerAngles = new Vector3(verticalRotation, horizontalRotation, transform.eulerAngles.z);
    }

    public Vector3 GetHorizontalRotation()
    {
        return Vector3.up * horizontalRotation;
    }
}
