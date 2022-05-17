using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    private PlayerActions PlayerActions;

    private bool isOn;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerActions =  new PlayerActions();
        PlayerActions.Enable();
    }

    void Start()
    {
        gameObject.GetComponent<Attatchments>().SetAttatchment(false, 4); 
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerActions.General.Flashlight.triggered && !isOn)
        {
            gameObject.GetComponent<Attatchments>().SetAttatchment(true, 4);
            isOn = true;
        }
        else if(PlayerActions.General.Flashlight.triggered && isOn)
        {
            gameObject.GetComponent<Attatchments>().SetAttatchment(false, 4);
            isOn = false;
        }
    }
}
