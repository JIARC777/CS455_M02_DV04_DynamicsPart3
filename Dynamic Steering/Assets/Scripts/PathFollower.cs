using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFollower: Kinematic
{
    public GameObject[] waypoints;
    PathFollow path;
    void Start()
    {
        path = new PathFollow();
        path.path = waypoints;
        path.ai = this;
    }
    void Update()
    {
        SteeringOutput myMovement = path.GetSteering();
        LookWhereGoing look = new LookWhereGoing();
        look.target = path.target;
        look.ai = this;
        SteeringOutput mySteering = look.GetSteering();
        linearVelocity += myMovement.linear * Time.deltaTime;
        transform.position += linearVelocity * Time.deltaTime;
        if (linearVelocity.magnitude > maxSpeed)
        {
            linearVelocity.Normalize();
            linearVelocity *= maxSpeed;
        }
        /*
        angularVelocity += mySteering.angular * Time.deltaTime;
        if (float.IsNaN(angularVelocity))
            angularVelocity = 0;
        transform.eulerAngles += new Vector3(0, angularVelocity * Time.deltaTime, 0);
        */
    }
}

