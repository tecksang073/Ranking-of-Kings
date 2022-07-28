using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLocalEulerAngles : MonoBehaviour
{
    public Vector3 eulerAngles = Vector3.zero;
    private void Update()
    {
        transform.localEulerAngles = eulerAngles;
    }
}
