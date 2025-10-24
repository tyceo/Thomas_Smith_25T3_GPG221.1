using UnityEngine;
using System.Collections;

public class SteeringManager : MonoBehaviour
{
    private Avoid leftAvoid;
    private Avoid rightAvoid;
    private TurnTowards turnTowards;

    public GameObject leftSensor;
    public GameObject rightSensor;

    void Start()
    {
        leftSensor = transform.Find("Left").gameObject;
        rightSensor = transform.Find("Right").gameObject;

        leftAvoid = leftSensor.GetComponent<Avoid>();
        rightAvoid = rightSensor.GetComponent<Avoid>();
        turnTowards = GetComponent<TurnTowards>();
    }

    void Update()
    {
        if (leftAvoid != null && rightAvoid != null && turnTowards != null)
        {
            // Check if either raycast is hitting
            bool anyRayHitting = leftAvoid.isHitting || rightAvoid.isHitting;
            
            // Enable/disable TurnTowards based on ray hits
            turnTowards.enabled = !anyRayHitting;

            // Previous sensor logic
            bool bothSensorsHitting = leftAvoid.isHitting && rightAvoid.isHitting;
            string leftHit = leftAvoid.whatLeftIsHitting;
            string rightHit = rightAvoid.whatRightIsHitting;

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