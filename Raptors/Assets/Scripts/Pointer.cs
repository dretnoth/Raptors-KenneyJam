using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pointer : MonoBehaviour
{
    public PlayerControll player;
    public bool targetMotherShipB;
    Vector3 targetDirection;
    public float speed=15;
    Vector3 newDirection;
    Transform theTarget;
    float deathTimer=1;
    

    

    private void Start() {
        if(targetMotherShipB)
        this.GetComponentInChildren<SpriteRenderer>().color = Controll.GameController.mydata.theCollors[Controll.GameController.collorOfPlayer];
    }

    public void InsertTheData(Transform newTarget){
        theTarget = newTarget;
        this.GetComponentInChildren<SpriteRenderer>().color = Controll.GameController.mydata.theCollors[
            theTarget.GetComponentInChildren<DamageHandler>().warSide];
    }

    void Update()
    {
        if(targetMotherShipB){
            if(player.motherShip != null)
                theTarget = player.motherShip;
        }

        if(theTarget != null){
            targetDirection = theTarget.position - transform.position;
            
            
		    targetDirection.Normalize ();

		    float zAngle = Mathf.Atan2 (targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90; //this return radians, 0 angle facing right

		    Quaternion desiredRot = Quaternion.Euler (0, 0, zAngle);
		    transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, 1); //180 * Time.deltaTime
        }else{
            deathTimer -= Time.deltaTime;
            if(deathTimer < 0){Destroy(gameObject);}
        }
    }
}
