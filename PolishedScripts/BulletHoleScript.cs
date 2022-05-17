using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleScript : MonoBehaviour
{
    public float bulletHoleDestroyTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, bulletHoleDestroyTime);
    }
}
