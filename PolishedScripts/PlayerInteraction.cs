using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour 
{
    //Create PlayerActions script
    private PlayerActions PlayerActions;
    
	void Start () 
	{
	    PlayerActions = new PlayerActions();
	    PlayerActions.Enable();
	}
	
	void Update () 
	{
	    //Querey if the E button has been pressed though trigger
	    if(PlayerActions.General.Interaction.triggered)
	    {
	        //Call raycast of the object infront of the player
	        RayCastComponets Raycast = gameObject.GetComponent<Raycast>().castRay();
	        
	        //Check the raycast for the object
	        if(Raycast.Hit.collider.tag == "Door")
	        {
	            Raycast.Hit.collider.gameObject.GetComponent<RayCastHandeler>().Interact("Door", 0);
	        }
	    }
	}
}
