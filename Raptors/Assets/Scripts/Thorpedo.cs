using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorpedo : MonoBehaviour
{
    public int warSide;
    public float speedMax, speedRotate, scanerInterval, scanerRange;
    Vector3 pos, velocity, direction;
    float scanerTimer, zAngle, distanceToTarget;
    public Transform theTarget;
    Quaternion desiredRot;
    RaycastHit2D[] hit;


    
    void Update()
    {
        pos = transform.position;
		velocity = new Vector3 (0, speedMax * Time.deltaTime, 0);
		pos += transform.rotation * velocity;
		transform.position = pos;
        desiredRot = transform.rotation;


        scanerTimer += Time.deltaTime;
        pos = transform.position;
        if(scanerTimer > scanerInterval){
            scanerTimer = 0;
            if(theTarget == null)
                Scanner();
        }

        if(theTarget != null){
            direction = theTarget.transform.position - transform.position; //distant vector
			direction.Normalize ();
			zAngle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg - 90; //this return radians, 0 angle facing right
			desiredRot = Quaternion.Euler (0, 0, zAngle);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, speedRotate * Time.deltaTime);

            distanceToTarget = Vector3.Distance(pos, theTarget.position);

            if(theTarget == this.transform){
                Debug.Log("Thorpedo targeted itself");
                theTarget = null;
            }
        }
    }

    void Scanner(){
        //print("Torpedo scanning");
        hit = Physics2D.CircleCastAll (pos, scanerRange, new Vector2 (-1,1));
        float lastDistance=scanerRange+1, newDistance=0;
        if(hit != null){
            for (int i = 0; i < hit.Length; i++){
                if(hit [i].transform.GetComponent<DamageHandler>() != null){
                    if(hit [i].transform.GetComponent<DamageHandler>().warSide != warSide){
                        if(hit [i].transform.GetComponent<DamageHandler>().bulletB == false){
                            newDistance = Vector3.Distance(pos, hit [i].transform.position);
                            if(newDistance < lastDistance){
                                theTarget = hit[i].transform;
                                lastDistance = newDistance;                                
                            }
                        }
                    }
                }
            }
        }
        //if(theTarget != null){print("Torpedo found a target: " +theTarget.name);}
    }
}
