using UnityEngine;
using UnityEngine.Animations.Rigging;

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
