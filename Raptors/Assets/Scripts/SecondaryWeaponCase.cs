using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryWeaponCase : MonoBehaviour
{
    public bool shotGunB, cannoB, ThorpedoB, sentrySatelite;
    public int shotGunFragments = 8, warSide=0;
    public float shotgunAngle= 0.15f;
    float angle=0;
    Quaternion theAngle;

    public GameObject shotGunPrefab, canonShellPrefab, thorpedoPrefab, sentrySatelitePrefab;
    GameObject go;
    Vector3 pos;

    public void DoIt(int side, int variation, bool havingAimTargetB, Transform target)
    {
        warSide = side;
        pos = transform.position;
        

        if(shotGunB == true || variation == 1){
            for(int i=0; i< shotGunFragments; i++){
                theAngle = transform.rotation;
                angle += Random.Range(-shotgunAngle, shotgunAngle);
                go = (GameObject)Instantiate(shotGunPrefab, pos , transform.rotation);
                theAngle.z +=angle;
                go.transform.rotation = theAngle; 
              /*
                rotationZ -= moveHorizontal * speedRotate * Time.deltaTime; //change the z angle
		    nextRotation = Quaternion.Euler (0, 0, rotationZ); //recreate the quaternion
		    transform.rotation = nextRotation; //feed quaternion into our rotation
            */
                
                go.GetComponent<DamageHandler>().SetSide(warSide);
                go.transform.SetParent(Controll.GameController.folderForBullets);
            }
        }

        if(cannoB == true || variation == 2){            
                go = (GameObject)Instantiate(canonShellPrefab, pos , transform.rotation);
                go.GetComponent<DamageHandler>().SetSide(warSide);
                go.transform.SetParent(Controll.GameController.folderForBullets);            
        }

        if(ThorpedoB == true || variation == 3){            
                go = (GameObject)Instantiate(thorpedoPrefab, pos , transform.rotation);
                go.GetComponent<DamageHandler>().SetSide(warSide);
                go.GetComponent<Thorpedo>().warSide = warSide;
                if(havingAimTargetB == true){ go.GetComponent<Thorpedo>().theTarget = target;}
                go.transform.SetParent(Controll.GameController.folderForBullets);            
        }

        if(sentrySatelite == true || variation == 4){            
                go = (GameObject)Instantiate(sentrySatelitePrefab, pos , Quaternion.identity);
                //go.GetComponent<Turret>().warSide = warSide;
                go.GetComponent<DamageHandler>().SetSide(warSide);
                go.transform.SetParent(Controll.GameController.folderForObjects);            
        }


        Destroy(gameObject);
    }

    
}
