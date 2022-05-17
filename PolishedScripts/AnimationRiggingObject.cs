using UnityEngine;
using UnityEngine.Animations.Rigging;

//This is for Animation Rigging based off of the active state of an object. I use this for weapon animation rigging
//when the weapon is turned on, lerp the animation rig to 1 - When it is turned off, lerp it to 0

public class AnimationRiggingObject : MonoBehaviour
{
    [SerializeField] private GameObject Weapon;
    [SerializeField] private float AnimationSpeed;

    private Rig Rig;

    private void Awake()
    {
        Rig = GetComponent<Rig>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Weapon.activeSelf)
        {
            Rig.weight = Rig.weight + AnimationSpeed * Time.deltaTime;
        }
        else
        {
            Rig.weight = Rig.weight - AnimationSpeed * Time.deltaTime; ;
        }
    }
}
