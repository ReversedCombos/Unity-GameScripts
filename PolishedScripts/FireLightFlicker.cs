using UnityEngine;
public class FireLightFlicker : MonoBehaviour
{
    public float FlickerSpeed;
    public float FlickerAmount;
    public float maxValue;
    public float minValue;

    private float counter;
    private Light fireLight;
    private bool subAdd;
    private float subAddAmount;

    private void Awake()
    {
        fireLight = gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (counter >= FlickerSpeed)
        {
            subAdd = (Random.value > 0.5f);
            subAddAmount = Random.Range(0, FlickerAmount);
;
            if(subAdd == true)
            {
                fireLight.intensity += subAddAmount;
                if (subAddAmount > maxValue) { fireLight.intensity = maxValue; }
            }
            else
            {
                fireLight.intensity -= subAddAmount;
                if (subAddAmount < minValue) { fireLight.intensity = minValue; }
            }
            
            counter = 0;
        }
        counter += Time.deltaTime;
    }
}
