using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Controll myGameController;
    public Vector3 myPosition, targetPosition, point, point2, direction, myNewPosition;
    
    private Vector3 offset;
    public float distanceAlowed, distanceCurent;
    public Transform myTarget;

    private void Start() {
        point.z = 0;
        myNewPosition = transform.position;
    }

    public void MoveCamereaTo(Transform theTarget){
        Vector3 thePosition;
        thePosition.x =theTarget.position.x;
        thePosition.y =theTarget.position.y;
        thePosition.z =transform.position.z;
        transform.position = thePosition;
    }

    public void Reset() {
        Vector3 thePos = transform.position;
        thePos.x = 0;
        thePos.y = 0;
        transform.position = thePos;
    }

    void Update()
    {
        if(myTarget != null){
            myPosition = transform.position;
            targetPosition = myTarget.position;
            myPosition.x = targetPosition.x;
            myPosition.y = targetPosition.y;
            transform.position = myPosition;
            /*
            point.x = myPosition.x;
            point.y = myPosition.y;

            targetPosition = myTarget.position;

            distanceCurent = Vector3.Distance(targetPosition, point);
            if(distanceCurent > distanceAlowed){
                direction = targetPosition - point;
                point2 = targetPosition + direction;
                myNewPosition.x = point2.x;
                myNewPosition.y = point2.y;

                transform.position = myNewPosition;
            }
            */
        }
    }
}
