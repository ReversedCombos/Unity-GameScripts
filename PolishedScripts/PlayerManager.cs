using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    public void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject Player;
    public GameObject Hud;
    public GameObject ActiveAmmo;
    public GameObject ReserveAmmo;
    public GameObject Health;
}
