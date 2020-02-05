using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    // position from gameObject transform
    // rotation comes from transform as well
    public Vector3 linearVelocity;
    public float angularVelocity; // in degrees
    public GameObject target;
    public float maxSpeed = 40f;
    public dynamicSteering control;
    public string movementType;
    public string rotationType;
    // Only For Seperation or path following
    public GameObject[] Obstacles;
    // When object is seperating do not provide any other control
    bool seperating;
    // Update is called once per frame
    void Update()
    {
        transform.position += linearVelocity * Time.deltaTime;
        // adding angular velocity to current transform rotation y component
        if (float.IsNaN(angularVelocity))
            angularVelocity = 0;
        transform.eulerAngles += new Vector3(0, angularVelocity * Time.deltaTime, 0);
        //dynamicSteering steering = new Seek();
        // control to switch to proper steering behavior
        if (Obstacles.Length > 0)
        {
            Seperation seperation = new Seperation();
            seperation.targets = Obstacles;
            seperation.ai = this;
            SteeringOutput seperateForce = seperation.GetSteering();
            // check to see if steering is greater than zero and lock out control from other steering
            if (seperateForce.linear.magnitude > 0)
                seperating = true;
            else
                seperating = false;
            linearVelocity += seperateForce.linear * Time.deltaTime;
        }
        switch (movementType)
        {
            case "seek":
                Seek mySeek = new Seek();
                mySeek.ai = this;
                // if seek is false set seek property on class to false to activate flee
                mySeek.seek = true;
                mySeek.target = target;
                SteeringOutput steeringSeek = mySeek.GetSteering();
                if (!seperating)
                    linearVelocity += steeringSeek.linear * Time.deltaTime;
                if (linearVelocity.magnitude > maxSpeed)
                {
                    linearVelocity.Normalize();
                    linearVelocity *= maxSpeed;
                }
                break;

            case "flee":
                Seek myFlee = new Seek();
                myFlee.ai = this;
                // if seek is false set seek property on class to false to activate flee
                myFlee.seek = false;
                myFlee.target = target;
                SteeringOutput steeringFlee = myFlee.GetSteering();
                if (!seperating)
                    linearVelocity += steeringFlee.linear * Time.deltaTime;
                if (linearVelocity.magnitude > maxSpeed)
                {
                    linearVelocity.Normalize();
                    linearVelocity *= maxSpeed;
                }
                break;

            case "arrive":
                Arrive myArrive = new Arrive();
                myArrive.ai = this;
                myArrive.target = target;
                SteeringOutput steeringArrive = myArrive.GetSteering();
                if (!seperating)
                    linearVelocity += steeringArrive.linear * Time.deltaTime;
                break;
            case "pursue":
                Pursue myPursue = new Pursue();
                myPursue.ai = this;
                myPursue.target = target;
                SteeringOutput steeringPursue = myPursue.GetSteering();
                if (!seperating)
                    linearVelocity += steeringPursue.linear * Time.deltaTime;
                if (linearVelocity.magnitude > maxSpeed)
                {
                    linearVelocity.Normalize();
                    linearVelocity *= maxSpeed;
                }
                break;
            case "evade":
                Pursue myEvade = new Pursue();
                myEvade.ai = this;
                myEvade.target = target;
                // This changes the seek flag in the parent Seek class of Pursue, sending it the flee vector instead
                myEvade.seek = false;
                SteeringOutput steeringEvade = myEvade.GetSteering();
                if (!seperating)
                    linearVelocity += steeringEvade.linear * Time.deltaTime;
                if (linearVelocity.magnitude > maxSpeed)
                {
                    linearVelocity.Normalize();
                    linearVelocity *= maxSpeed;
                }
                break;
            default:
                // provide no input
                break;
            // If obstacles have been provided, return steering to seperate from them
            
        }

        switch (rotationType)
        {
            case "face":
                Face myFace = new Face();
                myFace.ai = this;
                myFace.target = target;
                SteeringOutput steeringFace = myFace.GetSteering();
                if (steeringFace != null)
                {
                //    linearVelocity += steering.linear * Time.deltaTime;
                    angularVelocity += steeringFace.angular * Time.deltaTime;
                }

                break;
            case "align":
                Align myAlign = new Align();
                myAlign.ai = this;
                myAlign.target = target;
                SteeringOutput steeringAlign = myAlign.GetSteering();
                if (steeringAlign != null)
                {
                    //linearVelocity += steering.linear * Time.deltaTime;
                    angularVelocity += steeringAlign.angular * Time.deltaTime;
                }
                break;

            case "look":
                LookWhereGoing myLook = new LookWhereGoing();
                myLook.ai = this;
                myLook.target = target;
                SteeringOutput steeringLook = myLook.GetSteering();
                if (steeringLook != null)
                {
                    //linearVelocity += steering.linear * Time.deltaTime;
                    angularVelocity += steeringLook.angular * Time.deltaTime;
                }
                break;
            default:
                //provide no input
                break;
        }
    }
}
