using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadoutItemSwitch : MonoBehaviour
{
    //Contains the gameObject list
    [SerializeField] private List<GameObject> Items = new List<GameObject>();

    //Contains the PlayerActions Instance
    private PlayerActions PlayerActions;

    //Contains list of the InputActions refrence
    private InputAction[] ItemsList;

    private void Awake()
    {
        //Instantiates PlayerActions as PlayerActions()
        PlayerActions = new PlayerActions();
        //Set up the ItemsList = Items (Note: Trigger = Item by number in list)
        ItemsList = new InputAction[] { PlayerActions.Inventory.ItemOne, PlayerActions.Inventory.ItemTwo, PlayerActions.Inventory.ItemThree };
    }
    private void Update()
    {
        //Based on itemList which is the trigger
        for(int i = 0; i < ItemsList.Length; i++)
        {
            //If triggered = true
            if(ItemsList[i].triggered)
            {
                //If item is active
                if (Items[i].activeSelf)
                {
                    //Set item to false
                    Items[i].SetActive(false);
                }
                //If item isn't active
                else
                {
                    //Loop though every object
                    for (int x = 0; x < Items.Count; x++)
                    {
                        //Set all objects to false
                        Items[x].SetActive(false);
                    }
                    //Set current item to true
                    Items[i].SetActive(true);
                }
            }
        }
    }
    void OnEnable()
    {
        PlayerActions.Enable();
    }
    void OnDisable()
    {
        PlayerActions.Disable();
    }
}


