using TMPro;
using UnityEngine;
using System.Collections;

public class Reload : MonoBehaviour
{
    //Hud gameObjects
    private GameObject PlayerHud;
    private GameObject ActiveAmmoHud;
    private GameObject ReserveAmmoHud;


    //Public ammo varables
    [SerializeField] public int MaximumAmmo;
    [SerializeField] public int ReserveAmmo;
    [SerializeField] public float ReloadDelay;

    //Private ammo varables
    public int ActiveAmmo;
    private PlayerActions PlayerActions;
    private float ReloadDelayCounter;
    public bool isReloading { get; private set; }

    private void Awake()
    {
        //Initalizes a new PlayerActions()
        PlayerActions = new PlayerActions();
        //Sets the first clip to full
        ActiveAmmo = MaximumAmmo;
        //Sets the ReloadDelayCounter to full so it can be shot on Awake
        ReloadDelayCounter = ReloadDelay;
    }

    private void OnEnable()
    {
        //Resets reload on enable
        isReloading = false;
        //Enables PlayerActions on Enable
        PlayerActions.Enable();
        //Enables hud and updates it
        EnableHud();
        UpdateHud();
    }
    private void OnDisable()
    {
        //Disables PlayerActions() on Disable
        PlayerActions.Disable();
        //Disables hud
        DisableHud();
    }

    private void Update()
    {
        //Updates ReloadDelayCounter
        ReloadDelayCounter += Time.deltaTime;
        //Checks if reload has triggered
        ReloadAmmo();
    }

    //Called when shot
    public void SubtractAmmo()
    {
        //Checks if can reload
        if (ReloadDelayCounter < ReloadDelay)
        {
            return;
        }

        //If clip isn't empty
        if (ActiveAmmo != 0)
        {
            //Subtract one bullet from clip
            ActiveAmmo -= 1;
        }
        //If clip is empty
        else
        {
            StartCoroutine(ReloadCoroutine(ReloadDelay));
        }
        UpdateHud();
    }
    public void ReloadAmmo()
    {
        //Only plays when reload it hit, has ammo in reserve, and has room in clip
        if (PlayerActions.Actions.Reload.triggered && ReserveAmmo != 0 && ActiveAmmo != MaximumAmmo)
        {
            //Starts Corutine
            StartCoroutine(ReloadCoroutine(ReloadDelay));
        }
    }

    public void AddAmmo(int ammoAmount)
    {
        ReserveAmmo += ammoAmount;
        UpdateHud();
    }

    private void UpdateHud()
    {
        //If is AI !Player then don't update hud
        if (gameObject.layer == LayerMask.NameToLayer("AI")) { return; };

        PlayerManager.instance.ActiveAmmo.GetComponent<TextMeshProUGUI>().text = ActiveAmmo.ToString();
        PlayerManager.instance.ReserveAmmo.GetComponent<TextMeshProUGUI>().text = ReserveAmmo.ToString();
    }
    private void EnableHud()
    {
        //If is AI !Player then don't update hud
        if (gameObject.layer == LayerMask.NameToLayer("AI")) { return; };
        PlayerManager.instance.Hud.SetActive(true);
    }
    private void DisableHud()
    {
        //If is AI !Player then don't update hud
        if (gameObject.layer == LayerMask.NameToLayer("AI")) { return; };
        PlayerManager.instance.Hud.SetActive(false);
    }
    IEnumerator ReloadCoroutine(float time)
    {
        if (isReloading == false)
        {
            //gameObject.GetComponent<Attatchments>().SetAttatchment(false, 2);
            ReloadDelayCounter = 0;
            isReloading = true;
            gameObject.GetComponent<Animator>().SetBool("Reloading", true);
            yield return new WaitForSeconds(time);
            gameObject.GetComponent<Animator>().SetBool("Reloading", false);
            if (ReserveAmmo != 0)
            {
                ReserveAmmo += ActiveAmmo;
                ActiveAmmo = 0;

                if (ReserveAmmo >= MaximumAmmo)
                {
                    ActiveAmmo = MaximumAmmo;
                    ReserveAmmo -= MaximumAmmo;
                }
                else
                {
                    ActiveAmmo = ReserveAmmo;
                    ReserveAmmo = 0;
                }

                UpdateHud();
            }
            //gameObject.GetComponent<Attatchments>().SetAttatchment(true, 2);
            isReloading = false;
        }
    }
}
