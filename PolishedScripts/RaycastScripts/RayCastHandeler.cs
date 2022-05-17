using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastHandeler : MonoBehaviour
{
    //Called with a string and float value to interact with different object on one script
    public void Interact(string type, float amount)
    {
        switch (type)
        {
            //Use case - If shot (Needs - ObjectHealth to work)
            case "Shot":
                gameObject.GetComponent<ObjectHealth>().TakeDamage(amount);
                break;
            case "Door":
                gameObject.GetComponent<DoorScript>().DoorToggel();
                break;
        }
    }
}
