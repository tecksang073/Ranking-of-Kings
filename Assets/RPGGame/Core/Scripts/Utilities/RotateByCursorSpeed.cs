using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByCursorSpeed : MonoBehaviour
{
    public bool isRotating;
    public float rotationSpeedX = 5f;
    public float rotationSpeedY = 5f;
    Vector3 rotationEuler;
    Quaternion defaultRotation;

    private void Awake()
    {
        defaultRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isRotating)
            return;

        rotationEuler = transform.rotation.eulerAngles;
        float rotY = -InputManager.GetAxis("Mouse X", false) * rotationSpeedY;
        float rotX = InputManager.GetAxis("Mouse Y", false) * rotationSpeedX;
        rotationEuler.x += rotX;
        rotationEuler.y += rotY;
        transform.rotation = Quaternion.Euler(rotationEuler);
    }

    public void SetIsRotating(bool val)
    {
        isRotating = val;
    }

    public void ResetRotation()
    {
        transform.rotation = defaultRotation;
    }
}
