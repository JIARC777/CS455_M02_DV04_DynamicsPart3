using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface dynamicSteering
{
    SteeringOutput GetSteering();
}
public class Seek: dynamicSteering
{
    public Kinematic ai;
    // Pseudocode has target as Kinematic, but it only needs to be a gameobject with transform.
    public GameObject target;
    float maxAcceleration = 100f;
    public bool seek = true;

    public virtual SteeringOutput GetSteering()
    {
        SteeringOutput result = new SteeringOutput();

        //get direction to target
        if (seek)
            result.linear = GetTargetPosition() - ai.transform.position;
        else
            result.linear = ai.transform.position - target.transform.position;
        result.linear.y = 0;
        result.linear.Normalize();
        // give full acceleration
        result.linear *= maxAcceleration;
        result.angular = 0;
        return result;
    }
    public virtual Vector3 GetTargetPosition()
    {
        return target.transform.position;
    }
}


public class Arrive: dynamicSteering
{
    public Kinematic ai;
    public GameObject target;
    float maxAcceleration = 100f;
    float targetRadius = 5f;
    float slowRadius = 20f;
    float timeToTarget = 0.1f;
    float maxSpeed = 20f;

    public virtual SteeringOutput GetSteering()
    {
        SteeringOutput result = new SteeringOutput();
        result.linear = target.transform.position - ai.transform.position;
        float distance = result.linear.magnitude;
        float targetSpeed;
        Vector3 targetVelocity;
        // if (distance < targetRadius)
        //     return null;
        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        }
        else
        {
            targetSpeed = maxSpeed * (distance - targetRadius) / targetRadius;
        }
        targetVelocity = result.linear;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        result.linear = targetVelocity - ai.linearVelocity;
        result.linear /= timeToTarget;

        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
        }
        result.angular = 0;
        return result;

    }
}

public class PathFollow : Arrive
{
    public GameObject[] path;
    float targetRadius = 10f;
    int currentIndex;

    public override SteeringOutput GetSteering()
    {
        if (target == null)
        {
            currentIndex = 0;
            target = path[currentIndex];
        }
        
        float distToTarget = (target.transform.position - ai.transform.position).magnitude;
        if (distToTarget < targetRadius)
        {
            currentIndex++;
            if (currentIndex > path.Length - 1)
                currentIndex = 0;
        }
        target = path[currentIndex];
        return base.GetSteering();
    }
}

public class Pursue: Seek
{
    float maxPredicitionTime = 10f;
    public override Vector3 GetTargetPosition()
    {
        float predictionTime;
        Vector3 directionToTarget = target.transform.position - ai.transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        float mySpeed = ai.linearVelocity.magnitude;
        if (mySpeed <= distanceToTarget / maxPredicitionTime)
        {
            predictionTime = maxPredicitionTime;
        } else
        {
            predictionTime = distanceToTarget / mySpeed;
        }
       // get from target which has no useful data to a kinematic (make sure to use pursue on other kinematic
       Kinematic myMovingTarget = target.GetComponent(typeof(Kinematic)) as Kinematic;
        if (myMovingTarget == null)
        {
            return base.GetTargetPosition();
            
        }
       return target.transform.position + myMovingTarget.linearVelocity * predictionTime;
    }

}

public class Seperation: dynamicSteering
{
    public Kinematic ai;
    // Pseudocode has target as Kinematic, but it only needs to be a gameobject with transform.
    public GameObject[] targets;
    public float maxAcceleration = 1000f;
    public float threshold = 15f;
    public float decayCoefficient = 10000f;
    public SteeringOutput GetSteering()
    {
        SteeringOutput result = new SteeringOutput();
        float strength = 0;
        foreach (GameObject target in targets) {
            Vector3 direction = ai.transform.position - target.transform.position;
            direction.y = 0;
            float distance = direction.magnitude;
            if (distance < threshold)
            {
                strength = Mathf.Min(decayCoefficient / (distance * distance), maxAcceleration);
                Debug.Log(strength);
            }
                
            direction.Normalize();
            result.linear += strength * direction;
            
        }
        return result;
    }
}
