using UnityEngine;
using System.Collections;


public class SteeringManager : MonoBehaviour
{
    private Avoid leftAvoid;
    private Avoid rightAvoid;

    public GameObject leftSensor;
    public GameObject rightSensor;

    void Start()
    {
        leftSensor = transform.Find("Left").gameObject;
        rightSensor = transform.Find("Right").gameObject;

        leftAvoid = leftSensor.GetComponent<Avoid>();
        rightAvoid = rightSensor.GetComponent<Avoid>();
    }

    void Update()
    {
        if (leftAvoid != null && rightAvoid != null)
        {
            // Check if both raycasts are hitting
            bool bothSensorsHitting = leftAvoid.isHitting && rightAvoid.isHitting;
            
            string leftHit = leftAvoid.whatLeftIsHitting;
            string rightHit = rightAvoid.whatRightIsHitting;
            
            

            // Your existing condition
            if (leftHit == "6" && rightHit == "7" && bothSensorsHitting)
            {
                rightSensor.SetActive(false);
                
            }
            else
            {
                rightSensor.SetActive(true);
            }

            if (leftHit == "7" && rightHit == "6" && bothSensorsHitting)
            {
                leftSensor.SetActive(false);
            }
            else
            {
                leftSensor.SetActive(true);
            }
        }
    }
    
    

}