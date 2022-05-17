using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    public float speed = 1;
    public float destroyTime;
    

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        //Updates the bullets direction
        //gameObject.transform.Translate(0, speed, 0);
        //another option VVVV
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(0, speed, 0, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
       
       
        Destroy(gameObject);
    }
}
