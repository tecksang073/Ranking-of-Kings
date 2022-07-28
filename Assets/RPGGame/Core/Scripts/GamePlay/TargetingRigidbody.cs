using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetingRigidbody : MonoBehaviour
{
    public float speed = 2.0f;
    public Vector3 target;
    public bool IsMoving { get; private set; }
    private Vector3 desiredVelocity;
    private Quaternion desiredRotation;
    private float lastSqrMag;

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
    private Rigidbody tempRigidbody;
    public Rigidbody TempRigidbody
    {
        get
        {
            if (tempRigidbody == null)
                tempRigidbody = GetComponent<Rigidbody>();
            return tempRigidbody;
        }
    }

    public void StartMove(Vector3 target, float speed)
    {
        this.speed = speed;
        this.target = target;
        // calculate directional vector to target
        var heading = target - TempTransform.position;
        var directionalVector = heading.normalized * speed;

        // reset lastSqrMag
        lastSqrMag = Mathf.Infinity;
        
        if (directionalVector.magnitude > 0)
        {
            // apply to rigidbody velocity
            desiredVelocity = directionalVector;
            desiredRotation = Quaternion.LookRotation(directionalVector);

            IsMoving = true;
        }
    }

    public void StopMove()
    {
        // rigidbody has reached target and is now moving past it
        // stop the rigidbody by setting the velocity to zero
        desiredVelocity = Vector3.zero;
        TempRigidbody.velocity = desiredVelocity;
        IsMoving = false;
    }

    private void Update()
    {
        if (!IsMoving)
            return;

        // check the current sqare magnitude
        var heading = target - TempTransform.position;
        var sqrMag = heading.sqrMagnitude;

        // check this against the lastSqrMag
        // if this is greater than the last,
        // rigidbody has reached target and is now moving past it
        if (sqrMag > lastSqrMag)
        {
            StopMove();
            return;
        }

        // make sure you update the lastSqrMag
        lastSqrMag = sqrMag;
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            TempRigidbody.velocity = desiredVelocity;
            TempTransform.rotation = desiredRotation;
        }
    }
}
