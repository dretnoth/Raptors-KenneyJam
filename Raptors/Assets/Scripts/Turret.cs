using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public int warSide = 0; public bool sateliteB;
    public float speedRotate=80, scanerInterval = 0.1f, fireInterval = 1, scanerRange = 5;
    float fireTimer=0, scanerTimer=0, zAngle, distanceToTarget, sqrDistance;
    public int ammoCurent, amooMax=5;
    public float ammoInterval=3; float ammoTimer=0;
    public Transform[] theAmmoSprites;
    Vector3 direction, pos;
    Quaternion desiredRot;
    public Transform theTarget;
    public AudioSource fireSfx;
    RaycastHit2D[] hit;
    public GameObject firePrefab;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        scanerTimer += Time.deltaTime;
        pos = transform.position;
        //if(theTarget == missing)
        if(scanerTimer > scanerInterval){
            scanerTimer = 0;
            if(theTarget == null)
                Scanner();
        }

        if(sateliteB){
            if(ammoCurent < amooMax){
                ammoTimer += Time.deltaTime;
                if(ammoTimer > ammoInterval){
                    ammoTimer = 0;
                    ammoCurent ++;
                    theAmmoSprites[ammoCurent-1].gameObject.SetActive(true);
                }
            }
        }

        if(theTarget != null){
            direction = theTarget.transform.position - transform.position; //distant vector
			direction.Normalize ();
			zAngle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg - 90; //this return radians, 0 angle facing right
			desiredRot = Quaternion.Euler (0, 0, zAngle);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, speedRotate * Time.deltaTime);

            distanceToTarget = Vector3.Distance(pos, theTarget.position);

            if(desiredRot == transform.rotation){
                if(fireTimer > fireInterval){
                    if(sateliteB == false){
                        fireTimer = 0;
                        FireNormalBullet();
                    }                            

                    if(sateliteB == true){
                        if(ammoCurent > 0){
                            fireTimer = 0;
                            theAmmoSprites[ammoCurent -1].gameObject.SetActive(false);
                            ammoCurent --;
                            FireNormalBullet();
                        }
                    }
                            
                }
            }
        }
        
        if(theTarget != null){
            //sqrDistance = (pos - theTarget.position).sqrMagnitude;
            sqrDistance = Vector3.Distance(pos, theTarget.position);
            if(sqrDistance > scanerRange + 1){
                theTarget = null;
            }    
        }


    }

    void FireNormalBullet(){
        GameObject myBullet = (GameObject)Instantiate(firePrefab, transform.position, transform.rotation);        
        myBullet.GetComponent<DamageHandler>().SetSide( warSide );
        myBullet.transform.SetParent(Controll.GameController.folderForBullets.transform);
        
        fireSfx.Play();
    }

    void Scanner(){
        hit = null;
        //hit = Physics2D.RaycastAll (pos, Vector2.up, scanerRange /*new Vector2 (-1,1) */);
        hit = Physics2D.CircleCastAll(this.transform.position, scanerRange, new Vector2 (-1,1) );
        
        float lastDistance=scanerRange+1, newDistance=0;

                


        if(hit != null){
            for (int i = 0; i < hit.Length; i++){
                if(hit [i].transform.GetComponent<DamageHandler>() != null){
                    if(hit [i].transform.GetComponent<DamageHandler>().warSide != warSide){
                        if(hit [i].transform.GetComponent<DamageHandler>().bulletB == false){
                            newDistance = Vector3.Distance(pos, hit [i].transform.position);
                            if(newDistance < lastDistance){
                                if(sateliteB == false){
                                    theTarget = hit[i].transform;
                                    lastDistance = newDistance;    
                                }

                                if(sateliteB == true){
                                   if(hit [i].transform.GetComponent<DamageHandler>().asteroidB == false){
                                        theTarget = hit[i].transform;
                                        lastDistance = newDistance; 
                                   }else{
                                       if(newDistance < scanerRange / 2){
                                            theTarget = hit[i].transform;
                                            lastDistance = newDistance;
                                       }
                                   } 
                                }
                                
                            }
                        }
                    }
                }
            }
        }
    }
}
