using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seperate : Kinematic
{
    public GameObject[] obstacles;

    // Update is called once per frame
    void Update()
    {
        Seperation seperation = new Seperation();
        seperation.targets = obstacles;
        seperation.ai = this;
        SteeringOutput myMovement = seperation.GetSteering();
        LookWhereGoing look = new LookWhereGoing();
        look.ai = this;
        look.target = target;
        SteeringOutput mySteering = look.GetSteering();
        if (mySteering != null)
        {
            //linearVelocity += steering.linear * Time.deltaTime;
            angularVelocity += mySteering.angular * Time.deltaTime;
        }
        linearVelocity += myMovement.linear * Time.deltaTime;
        transform.position += linearVelocity * Time.deltaTime;

        if (float.IsNaN(angularVelocity))
            angularVelocity = 0;
        transform.eulerAngles += new Vector3(0, angularVelocity * Time.deltaTime, 0);
    }
}
