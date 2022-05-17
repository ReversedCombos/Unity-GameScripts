using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is an attatchment script that can modify values like damage and reload speed based on what attatchment is active.
//The gameObjects state are based off the Booleans and those gameObjects have the effects which is "AttatchmentEffects"

public class Attatchments : MonoBehaviour
{
    
    //Public varables
    [Header("Scopes")]
    [SerializeField] private List<GameObject> ScopesGameobject = new List<GameObject>();
    [SerializeField] private List<bool> ScopesBool = new List<bool>();

    [Header("Stocks")]
    [SerializeField] private List<GameObject> StocksGameobject = new List<GameObject>();
    [SerializeField] private List<bool> StocksBool = new List<bool>();
    [Header("Magazines")]
    [SerializeField] private List<GameObject> MagazinesGameobject = new List<GameObject>();
    [SerializeField] private List<bool> MagazinesBool = new List<bool>();
    [Header("Grips")]
    [SerializeField] private List<GameObject> GripsGameobject = new List<GameObject>();
    [SerializeField] private List<bool> GripsBool = new List<bool>();
    [Header("SideAttatchments")]
    [SerializeField] private List<GameObject> SideAttatchmentsGameobject = new List<GameObject>();
    [SerializeField] private List<bool> SideAttatchmentsBool = new List<bool>();

    //Create three list that contain the varables for each Attatchment type
    private List<List<GameObject>> NestedListGameObject;
    private List<List<bool>> NestedListBoolean;
    //private List<List<float>> LastNestedListFloat;
    private List<int> LastBoolIndex;

    //Public nested lists
    public List<List<float>> NestedFloatList = new List<List<float>>() { null, null, null, null, null };

    // Update is called once per frame
    private void Awake()
    {
        NestedListGameObject = new List<List<GameObject>> { ScopesGameobject, StocksGameobject, MagazinesGameobject, GripsGameobject, SideAttatchmentsGameobject };
        NestedListBoolean = new List<List<bool>> { ScopesBool, StocksBool, MagazinesBool, GripsBool, SideAttatchmentsBool };
    }

    private void Update()
    {
        //Recurses though each one of the nested lists
        for (int i = 0; i < NestedListBoolean.Count; i++)
        {
            //Sets the current lists boolean while updating the attatchments with the lists
            UpdateAttatchments(NestedListGameObject[i], NestedListBoolean[i], i);
        }
        //Update the Attatchment Effects with NestedFloatList
        gameObject.GetComponent<Shoot>().UpdateAttatchmentEffects(NestedFloatList);
    }

    public void SetAttatchment(bool AttatchmentBool, int AttatchmentGroup)
    {
        //Checks the AttatchmentBool to separate SetActive(true/false);
        if (AttatchmentBool)
        {
            try
            {
            //Gets the first object of the list and gets the game object and sets it to true
            NestedListGameObject[AttatchmentGroup][0].transform.parent.gameObject.SetActive(true);
            }
            catch 
            {
                Debug.LogWarning("The Attatchment Index of " + AttatchmentGroup + " hasn't been added");
            }
        }
        else
        {
            try
            {
                //Gets the first object of the list and gets the game object and sets it to true
                NestedListGameObject[AttatchmentGroup][0].transform.parent.gameObject.SetActive(false);
            }
            catch
            {
                Debug.LogWarning("The Attatchment Index of " + AttatchmentGroup + " hasn't been added");
            }
        }
    }

    private void UpdateAttatchments(List<GameObject> CurrentGameObjectList, List<bool> CurrentBooleanList, int AttatchmentGroup)
    {
        //Gets the current boolean list true
        int BoolIndex = CurrentBooleanList.IndexOf(true);

        //Recures though the CurrentGameObjectList
        for (int i = 0; i < CurrentGameObjectList.Count; i++)
        {
            //Set the CurrentGameObjectList of the current recurse false
            CurrentGameObjectList[i].SetActive(false);
        }
        //Checks if the Bool list contains a true item
        if (BoolIndex != -1)
        {
            //Set the gameObject of the indexed object to true
            CurrentGameObjectList[BoolIndex].SetActive(true);
            //Set the NestedFloatList of the AttatchmentGroup to the current Attatchment active
            NestedFloatList[AttatchmentGroup] = CurrentGameObjectList[BoolIndex].GetComponent<AttatchmentEffects>().Effects;

        }
        else
        {
            //If the CurrentBooleanList doesn't contain a true item then set the effect in the AttatchmentGroup null
            //NestedFloatList[AttatchmentGroup] = null;

        }
    }
}

