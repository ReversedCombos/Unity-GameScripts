using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moisty : MonoBehaviour
{
    public bool InWater {get; set;}

    private RaycastHit Hit;
    private bool HitBool;
    
    [SerializeField] private float LerpStrength;
    [SerializeField] private float Pos_MaxVelocity;
    [SerializeField] private float Neg_MaxVelocity;
    [SerializeField] private float SubmergenceStrength;
    [SerializeField] private float Viscosity;
    public float Submergence {get; set;}

    private Vector3 Center;
    private Vector3 TopPoint;
    private Vector3 BottomPoint;
    private float Height;

    void Update()
    {
        UpdatePoints();
        CheckDistance();

        if (InWater)
        {
            CheckVolume();
        }
    }

    private void UpdatePoints()
    {
        TopPoint = new Vector3(gameObject.GetComponent<Collider>().bounds.center.x, gameObject.GetComponent<Collider>().bounds.max.y, gameObject.GetComponent<Collider>().bounds.center.z);
        BottomPoint = new Vector3(gameObject.GetComponent<Collider>().bounds.center.x, gameObject.GetComponent<Collider>().bounds.min.y, gameObject.GetComponent<Collider>().bounds.center.z);
        Height = Vector3.Distance(TopPoint, BottomPoint);

        Debug.DrawRay(TopPoint,
        Vector3.down,
        Color.red
        );
        Debug.DrawRay(BottomPoint,
        Vector3.forward,
        Color.red
        );
    }

    private async void CheckVolume()
    {
        if(InWater)
        {
            if(!HitBool)
            {
                Submergence = (1 / Height) * Height;
            }
            else
            {
                Submergence = (1 / Height) * (Height - Hit.distance);
            }

            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0f, Submergence * SubmergenceStrength * Time.deltaTime, 0f), ForceMode.Force);
            gameObject.GetComponent<Rigidbody>().velocity *= 1f - Viscosity * Time.deltaTime;
            gameObject.GetComponent<Rigidbody>().angularVelocity *= 1f - Viscosity * Time.deltaTime;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x, Mathf.Lerp(gameObject.GetComponent<Rigidbody>().velocity.y, Mathf.Clamp(gameObject.GetComponent<Rigidbody>().velocity.y, Neg_MaxVelocity, Pos_MaxVelocity), LerpStrength), gameObject.GetComponent<Rigidbody>().velocity.z);
        }
    }
    private void CheckDistance()
    {
        if(Physics.Raycast(TopPoint, 
        Vector3.down, 
        out Hit,
        100f,
        1 << 4))
        {
            HitBool = true;
        }
        else
        {
            HitBool = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water")
        {
            InWater = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Water")
        {
            InWater = false;
        }
    }
}
