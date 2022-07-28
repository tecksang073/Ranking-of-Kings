using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLocalPosition : MonoBehaviour
{
    public Vector3 position = Vector3.zero;
    private void Update()
    {
        transform.localPosition = position;
    }
}
