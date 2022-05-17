using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Floats")]
    [SerializeField] private float Distance;
    [SerializeField] private float ShootDelay;
    [SerializeField] private float Damage;
    [SerializeField] private float ShotForce = 5;

    [Header("GameObjects")]
    [SerializeField] private GameObject ShootPosition;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject BulletHole;
    [SerializeField] private GameObject WaterEffect;
    [SerializeField] private GameObject GroundEffect;

    [SerializeField] private GameObject shootSound;

    public bool hasShot { get; set; }
    
    private float AttatchmentDamage;
    private PlayerActions PlayerActions;
    private float ShootDelayCounter;

    private void Awake()
    {
        PlayerActions = new PlayerActions();
        ShootDelayCounter = ShootDelay;
    }
    void OnEnable()
    {
        PlayerActions.Enable();
        ShootDelayCounter = ShootDelay;
    }
    void OnDisable()
    {
        PlayerActions.Disable();
    }

    private void Update()
    {
        //The Shot animation bool is set to false the next Update loop to have the Animator have a frame to enter the animation
        gameObject.GetComponent<Animator>().SetBool("Shot", false);
        if (((gameObject.layer == LayerMask.NameToLayer("AI") && hasShot )|| (gameObject.layer == LayerMask.NameToLayer("Player") && PlayerActions.Actions.Shoot.ReadValue<float>().Equals(1f))) && ShootDelayCounter >= ShootDelay && gameObject.GetComponent<Reload>().isReloading == false)
        {

            //Checks if can be shot - No ammo in both
            if (gameObject.GetComponent<Reload>().ActiveAmmo == 0 && gameObject.GetComponent<Reload>().ReserveAmmo == 0)
            {
                //If not then return;
                return;
            }
            //Checks if ActiveAmmo needs to be reloaded
            else if (gameObject.GetComponent<Reload>().ActiveAmmo == 0)
            {
                //Calls ReloadAmmo in Reload
                gameObject.GetComponent<Reload>().ReloadAmmo();
            }

            //Use case - If shot
            HasShot();

            //Checks if the shot connected - If so then everything will be handeled in the CheckHit function
            CheckHit();
        }
        else
        {
            ShootDelayCounter += Time.deltaTime;
        }
    }

    void CheckHit()
    {
        //Calls the raycast and stores the ray and hit values in RayCastComponets
        RayCastComponets RayCastComponets = gameObject.GetComponentInParent<Raycast>().castRay();

        //Saves data though if() resource saving
        if (RayCastComponets.Hit.distance <= Distance)
        {
            if (RayCastComponets.Hit.collider.gameObject.tag == "Water")
            {
                Instantiate(WaterEffect, RayCastComponets.Hit.point, Quaternion.FromToRotation(Vector3.up, RayCastComponets.Hit.normal)).transform.SetParent(RayCastComponets.Hit.collider.gameObject.transform);
            }
            else if(RayCastComponets.Hit.collider.gameObject.tag == "Ground")
            {
                Instantiate(GroundEffect, RayCastComponets.Hit.point, Quaternion.FromToRotation(Vector3.up, RayCastComponets.Hit.normal)).transform.SetParent(RayCastComponets.Hit.collider.gameObject.transform); ;
                Instantiate(BulletHole, RayCastComponets.Hit.point, Quaternion.FromToRotation(Vector3.up, RayCastComponets.Hit.normal)).transform.SetParent(RayCastComponets.Hit.collider.gameObject.transform);
            }            
            //Use case - If bullet hits object within Distance



            //For errors though not being able to find the RayCast script and not being able to find a Rigidbody on the object shot
            try
            {
                //Adds force on the rigidBody of the game object shot with the Ray direction
                RayCastComponets.Hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(RayCastComponets.Ray.direction * ShotForce, ForceMode.Impulse);
            }
            catch
            {
                Debug.LogWarning("No RayCast script in parent or no Rigidbody on object");
            }
            //Checks if RayCastHandeler is on the object shot
            if (RayCastComponets.Hit.collider.gameObject.GetComponent<RayCastHandeler>() != null)
            {
                //Calls the Interact function on RayCastHandeler for Shot switch
                RayCastComponets.Hit.collider.gameObject.GetComponent<RayCastHandeler>().Interact("Shot", Damage + AttatchmentDamage);
            }
        }
    }

    void HasShot()
    {
        
        //Resets ShootDelayCounter
        ShootDelayCounter = 0f;
        //Plays the Shot animation
        gameObject.GetComponent<Animator>().SetBool("Shot", true);
        //Instantiates the Bullet at the ShootPosition with its rotation
        Instantiate(Bullet, ShootPosition.transform.position, ShootPosition.transform.rotation);

        Instantiate(shootSound, ShootPosition.transform.position, ShootPosition.transform.rotation);
        //Plays the ShootPosition's Particle system
        ShootPosition.GetComponent<ParticleSystem>().Play();
        //Calls the update ammo function
        UpdateAmmo();

        
    }

    public void UpdateAttatchmentEffects(List<List<float>> Effects)
    {
        //Resets every call
        AttatchmentDamage = 0;

        //Recurses though the list of effects
        for (int i = 0; i < Effects.Count; i++)
        {
            //Checks if the current AttatchmentGroup is active
            if (Effects[i] != null)
            {
                //The subList number is the Effect - The subList refrence can be found on AttatchmentEffects

                //Updates AttatchmentDamage
                AttatchmentDamage += Effects[i][0];
            }
        }
    }

    private void UpdateAmmo()
    {
        if (gameObject.GetComponent<Reload>() != null)
        {
            gameObject.GetComponent<Reload>().SubtractAmmo();
        }
    }
}
