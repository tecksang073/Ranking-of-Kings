using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnvironmentObject : MonoBehaviour
{
    private Transform tempTransform;
    public Transform TempTransform
    {
        get
        {
            if (tempTransform == null)
                tempTransform = GetComponent<Transform>();
            return tempTransform;
        }
    }

    private BoxCollider tempBoxCollider;
    public BoxCollider TempBoxCollider
    {
        get
        {
            if (tempBoxCollider == null)
                tempBoxCollider = GetComponent<BoxCollider>();
            return tempBoxCollider;
        }
    }

    public Bounds GetBounds()
    {
        return TempBoxCollider.bounds;
    }
}
