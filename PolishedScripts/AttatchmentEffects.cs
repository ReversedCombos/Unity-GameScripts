using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttatchmentEffects : MonoBehaviour
{
    //Add effects here! (Note: Set the effects in the Awake function!)
    [Header("Effects")]
    [SerializeField] private float DamageEffect;

    //Initalizes the list for the effects to pool into
    public List<float> Effects;
    private void Awake()
    {
        //Set the effect of the attatchment on awake
        Effects = new List<float>() { DamageEffect };
    }
}
