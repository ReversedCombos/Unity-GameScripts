using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
[RequireComponent(typeof(RayCastHandeler))]
public class ObjectHealth : MonoBehaviour
{
    [SerializeField] public float health;

    [SerializeField] private GameObject hurtSound;
    public void TakeDamage(float amount)
    {
        //Subtracts the current health with the amount of damage taken
        health -= amount;

        if(gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerManager.instance.Health.GetComponent<TextMeshProUGUI>().text = health.ToString();
        }

        //If the health is below or equal to zero
        if (health <= 0)
        {
            //If the script is on the player
            if (gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Get the currentScene name
                string currentScene = SceneManager.GetActiveScene().name;
                //Load the currentScene name
                SceneManager.LoadScene(currentScene);
            }
            //If the script is on an AI
            else
            {
                //Destroy the AI
                Destroy(gameObject);
            }
           
        }
    }
}
