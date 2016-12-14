using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Extensions
{
    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }

    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }

    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }

    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }

    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }

    public static bool HasRigidbody(this GameObject gobj)
    {
        return (gobj.GetComponent<Rigidbody>() != null);
    }

    public static bool HasAnimation(this GameObject gobj)
    {
        return (gobj.GetComponent<Animation>() != null);
    }

    public static void SetSpeed(this Animation anim, float newSpeed)
    {
        anim[anim.clip.name].speed = newSpeed;
    }
    public static bool isDistanceSmallerThan(this Transform t, Vector3 otherPos, float range)
    {
        return ((t.position - otherPos).sqrMagnitude < Mathf.Pow(range, 2));
    }
    public static float distanceInPow(this Transform t, Vector3 otherPos)
    {
        return ((t.position - otherPos).sqrMagnitude);
    }
    public static void limitVelocitySoft3D(this Rigidbody r, float maxVelocity, float reverseForce)
    {
        //reverseForce 10 is the minimum for it to work properly
        //mostly use this as default
        float velocity = Vector3.Magnitude(r.velocity);
        if (velocity > maxVelocity)
        {
            Vector3 normalizedVelocity = r.velocity.normalized;
            Vector3 reversedForce = normalizedVelocity * (reverseForce*r.mass);
            r.AddForce(-reversedForce);
        }
        //Debug.Log(velocity);    
    }
    public static void limitVelocityHard3D(this Rigidbody r, float maxVelocity)
    {
        //dont use to often, messes with potential knockback
        float velocity = Vector3.Magnitude(r.velocity);
        if (velocity > maxVelocity)
        {
            Vector3 newVelocity = r.velocity.normalized;
            newVelocity *= maxVelocity;
            r.velocity = newVelocity;
        }
    }
    public static void airDragg3D(this Rigidbody r, float dragg)
    {
        //dragg on 1 maxes velocity around 10 with a mass 1 object
        r.velocity -= r.velocity * (dragg / r.mass) * Time.deltaTime;
    }
    public static void knockback(this Rigidbody r, Vector3 direction, float force, bool absolute)
    {
        if (absolute)//this stops the player completely and then knocks back.
        {
            r.velocity = Vector3.zero;
        }
        r.AddForce(Vector3.zero);
        r.AddRelativeForce(direction * force, ForceMode.Impulse);
    }
    public static void explosion(this GameObject G, float force, float explosionRadius, bool absolute, bool remove)
    {
        Ray ray = new Ray(G.transform.position,Vector3.down);
        RaycastHit[] hit = Physics.SphereCastAll(ray, explosionRadius, 0.001f);
        foreach(RaycastHit i in hit)
        {
            if(i.rigidbody != null)
            {
                Vector3 direct = (i.transform.position - G.transform.position);
                i.rigidbody.knockback(direct.normalized, force, absolute);
            }
            //Debug.Log(i);
        }
        if(remove)
        {
            GameObject.Destroy(G);
        }        
    }
    public static RaycastHit[] getWithinSphere(this Transform T, float checkRadius)
    {
        //this return every object within range of a sphere with the checkRadius radius
        Ray ray = new Ray(T.position, Vector3.down);
        RaycastHit[] hit = Physics.SphereCastAll(ray, checkRadius, 0.001f);
        return(hit);
    }

    public static RaycastHit hitByMouse(this GameObject C)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return (hit);
        }
        return (new RaycastHit());
    }
    public static void lookAtMouse(this Transform T, float speed)
    {
        Plane playerPlane = new Plane(Vector3.up, T.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - T.position);

            T.rotation = Quaternion.Slerp(T.rotation, targetRotation, speed * Time.deltaTime);
        }
    }
    public static Vector3 mousePos(this Transform T)
    {
        Plane playerPlane = new Plane(Vector3.up, T.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            return (ray.GetPoint(hitdist));
        }
        return (new Vector3(0,0,0));
    }
    /*Execute<IDamagable>(currentComponent) =>{
            currentComponent.takedamg();
        }
    */
    public static void Execute<T>(GameObject target, Action<T> message)
    {
        T[] components = target.GetComponents<T>();

        foreach (T component in components)
        {
            message(component);
        }
    }
}