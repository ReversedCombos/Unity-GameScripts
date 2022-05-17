using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Buoyancy : MonoBehaviour
{
    [SerializeField] private float underWaterDrag = 3;
    [SerializeField] private float underWaterAngularDrag = 1;

    private float airDrag = 0;
    private float airAngularDrag = .05f;

    [SerializeField] private float floatStrength = 15;

    [SerializeField] private Transform waterLevel;

    private Rigidbody RB;

    bool isSubmerged;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        airDrag = RB.drag;
        airAngularDrag = RB.angularDrag;
    }


    void FixedUpdate()
    {
        float dist = transform.position.y - 0;

        if (dist < 0)
        {
            RB.AddForceAtPosition(Vector3.up * floatStrength * Mathf.Abs(dist), transform.position, ForceMode.Force);
            if (!isSubmerged) isSubmerged = true; ChangeState(true);
        }
        else if (isSubmerged) isSubmerged = false; ChangeState(false);

    }

    void ChangeState(bool submerged)
    {
        if (submerged)
        {
            RB.drag = underWaterDrag;
            RB.angularDrag = underWaterAngularDrag;
        }
        else
        {
            RB.drag = airDrag;
            RB.angularDrag = airAngularDrag;
        }
    }
}