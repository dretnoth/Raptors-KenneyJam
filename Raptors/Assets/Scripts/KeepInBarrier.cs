using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInBarrier : MonoBehaviour
{
    public bool turnItB, hittingBarrierB, ignoringTimerOnB;
    public float extraSpaceRadius;
    float spaceRadius,distanceFromCenter, z, ignoreTimer=0;
    Vector3 centerPoint, pos, fromOriginToObject, newLocation;

    void Update()
    {
        pos = transform.position;
        spaceRadius = Controll.GameController.spaceRadius + extraSpaceRadius;
        centerPoint = Controll.GameController.centerPoint;
        distanceFromCenter = Vector3.Distance(pos, centerPoint);
        if(distanceFromCenter > spaceRadius){
            
            
            if(turnItB){
                //just turn it arround
                if(ignoringTimerOnB == false){
                    z  += 180 + Random.Range(-50, 50);
                    transform.eulerAngles = new Vector3(0,0,z);
                    ignoringTimerOnB = true;    
                }
                
            }else{
                //just keep it on the edge of batltlefield
                fromOriginToObject = pos - centerPoint;
                fromOriginToObject *= spaceRadius / distanceFromCenter;
                newLocation = centerPoint + fromOriginToObject;
                hittingBarrierB = true; // tell that he is hitting barrier
                transform.position = newLocation;    
            }

            
        }else{
            if(hittingBarrierB == true){hittingBarrierB = false;}
        }

        if(ignoringTimerOnB){
            ignoreTimer +=Time.deltaTime;
            if(ignoreTimer > 2){
                ignoreTimer = 0;
                ignoringTimerOnB = false;
            }
        }

    }
}
