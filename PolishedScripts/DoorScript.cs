using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RayCastHandeler))]

public class DoorScript : MonoBehaviour
{
    [SerializeField] private bool DoorOpen;
    public void DoorToggel()
    {
        if(!DoorOpen && !gameObject.GetComponent<Animation>().isPlaying)
        {
            //Play
            gameObject.GetComponent<Animation>().Play("DoorOpen");
            DoorOpen = true;
        }
        else if(!gameObject.GetComponent<Animation>().isPlaying)
        {
            gameObject.GetComponent<Animation>().Play("DoorClose");
            DoorOpen = false;
        }
    }
}
