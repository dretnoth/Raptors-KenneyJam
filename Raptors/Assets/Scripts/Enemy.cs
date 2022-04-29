using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool haveTargetB, targetInRangeB, wanderingAroundB, pullOutFromCollisionB;
    public bool typeA=true, typeB, typeC;
    public bool targetingPlayerB, targetingBaseB, targetingAsteroidB, targetingMonserB, targetingSateliteB, targetingOthersB=true;
    public float speedMaximum, speedCurent, speedRotate, speedingUpFActor, extraSpaceRadius=0, toCloseRange=0;
    Vector3 pos, velocity, direction;
    public float fireInterval = 1f, fireRange = 4, scanerInterval = 0.1f, scanerRange=5;
    public Transform theTarget, newTarget;
    public List<Transform> objectsInRange;
    public GameObject firePrefab;
    public Controll myControll;
    float fireTimer=0, scanerTimer=0, zAngle=0, distanceToTarget=0, optimalnullTargetRAnge=0;
    Quaternion desiredRot;
    RaycastHit2D[] hit;
    public AudioSource fireSfx;
    public Transform[] gunpost;
    public int numberOfGunposts; int usedGunpost=0;
    


    void Start()
    {
        optimalnullTargetRAnge = fireRange;
        if(scanerRange > fireRange){optimalnullTargetRAnge = scanerRange;}
        optimalnullTargetRAnge += (float)GetComponent<DamageHandler>().size;
        
        toCloseRange = (float)GetComponent<DamageHandler>().size + fireRange/5;
    }

    
    void Update()
    {
        fireTimer += Time.deltaTime;
        scanerTimer += Time.deltaTime;
        if(scanerTimer > scanerInterval){
            scanerTimer = 0;
            Scanner();
            //if(haveTargetB == false){}
        }


        //move forward
        pos = transform.position;
		velocity = new Vector3 (0, speedCurent * Time.deltaTime, 0);
		pos += transform.rotation * velocity;
		transform.position = pos;


        //turn Toward Target
        if(theTarget != null){
            direction = theTarget.transform.position - transform.position; //distant vector
			direction.Normalize ();
			zAngle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg - 90; //this return radians, 0 angle facing right
			desiredRot = Quaternion.Euler (0, 0, zAngle);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, speedRotate * Time.deltaTime);

            distanceToTarget = Vector3.Distance(pos, theTarget.position);
            if(distanceToTarget > optimalnullTargetRAnge){
                theTarget = null;
            }

            //type A Aim then slow to stop and then fire
            if(typeA)
            if(desiredRot == transform.rotation){
                if( distanceToTarget < fireRange){
                    if(speedCurent > 0){speedCurent -= speedingUpFActor * Time.deltaTime; }
                    if(speedCurent < 0) speedCurent = 0;

                    if(speedCurent == 0){                                            
                        if(fireTimer > fireInterval){
                            fireTimer = 0;
                            FireNormalBullet();
                        }
                    }    
                }else if(speedCurent < speedMaximum){ speedCurent += speedingUpFActor * Time.deltaTime; }                
            }//type A


            //typeB got into fire range then start to move into side
            if(typeB)
            if( distanceToTarget < fireRange){
                if(speedCurent > 0){speedCurent -= speedingUpFActor * Time.deltaTime; }
                if(speedCurent < 0) speedCurent = 0;
                if(speedCurent == 0){
                    velocity = new Vector3 (0, (0.5f) * Time.deltaTime, 0);
                    Quaternion diferentRotation = transform.rotation;
                    diferentRotation.z +=0.3f;
		            pos += diferentRotation * velocity;
		            transform.position = pos;
                }

                if(desiredRot == transform.rotation){
                    if(fireTimer > fireInterval){
                        fireTimer = 0;
                        FireNormalBullet();
                    }
                }

            }else if(speedCurent < speedMaximum){ speedCurent += speedingUpFActor * Time.deltaTime; }  
            //type B

            if(typeC)
            if( distanceToTarget < fireRange){
                if(fireTimer > fireInterval){
                        fireTimer = 0;
                        FireFromGunPost();
                    }
                if(distanceToTarget < toCloseRange){
                    desiredRot = Quaternion.Euler (0, 0, -zAngle);
                    transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, 2 *speedRotate * Time.deltaTime);
                }
            }


        }else{
            if(speedCurent < speedMaximum){
                speedCurent += speedingUpFActor * Time.deltaTime;
            }
            
        }

        

        

        if(pullOutFromCollisionB){
            for(int i=0; i < objectsInRange.Count; i++){
                if(objectsInRange[i] != null){
                    direction = (pos - objectsInRange[i].position);
                    if( direction.sqrMagnitude < 1 ){
                        direction.Normalize ();
                        GetComponent<Rigidbody2D> ().AddForce (-direction * speedCurent);
                    }
                }
            }
        }


        /*
        //if targetOutOfRange
        if(theTarget != null){
            sqrDistance = (pos - theTarget.position).sqrMagnitude;
            if(sqrDistance > scanerRange + 1){
                theTarget = null;
                print("target nulled because distance is: "+ sqrDistance);
            }    
        }
        */
        

    }

    void FireNormalBullet(){
        GameObject myBullet = (GameObject)Instantiate(firePrefab, transform.position, transform.rotation);        
        myBullet.GetComponent<DamageHandler>().SetSide( this.GetComponent<DamageHandler>().warSide );
        myBullet.transform.SetParent(Controll.GameController.folderForBullets.transform);
        
        fireSfx.Play();
    }

    void FireFromGunPost(){
        //print("Monster fire");
        GameObject myBullet = (GameObject)Instantiate(firePrefab, gunpost[usedGunpost].position, gunpost[usedGunpost].rotation);        
        myBullet.GetComponent<DamageHandler>().SetSide( this.GetComponent<DamageHandler>().warSide );
        myBullet.transform.SetParent(Controll.GameController.folderForBullets.transform);
        Controll.GameController.statisticNormalBulletFired++;
        if(myBullet.GetComponent<Thorpedo>() != null){
            myBullet.GetComponent<Thorpedo>().warSide = this.GetComponent<DamageHandler>().warSide; 
        }
        fireSfx.Play();
        usedGunpost ++;
        if(usedGunpost >= numberOfGunposts) usedGunpost = 0;
    }

    void Scanner(){
        hit = Physics2D.CircleCastAll (pos, scanerRange, new Vector2 (-1,1));
        bool isPresentInListB=false; 
        if(hit != null){
            for (int i = 0; i < hit.Length; i++){
                if(hit [i].transform.GetComponent<DamageHandler>() != null){
                    if(hit [i].transform.GetComponent<DamageHandler>().bulletB == false)
                    if(hit [i].transform.GetComponent<DamageHandler>().warSide != transform.GetComponent<DamageHandler>().warSide){
                        isPresentInListB = false;
                        for(int j=0; j<objectsInRange.Count; j++){
                            if(objectsInRange[j] != null)
                                if(objectsInRange[j] == hit[i]) isPresentInListB = true;
                        }

                        if(isPresentInListB == false){
                            objectsInRange.Add(hit [i].transform);
                        }

                        
                        if( targetingPlayerB == true)
                            if(hit [i].transform.GetComponent<DamageHandler>().playerB == true){
                                newTarget = hit[i].transform;
                            }
                        
                        if( targetingBaseB == true)
                            if(hit [i].transform.GetComponent<DamageHandler>().baseB == true){
                                newTarget = hit[i].transform;
                            }
                    
                        if( targetingAsteroidB == true)
                            if(hit [i].transform.GetComponent<DamageHandler>().asteroidB == true){
                                newTarget = hit[i].transform;
                            }

                        if(targetingMonserB == true)
                            if(hit [i].transform.GetComponent<DamageHandler>().monsterB == true){
                                newTarget = hit[i].transform;
                            }
                        if( targetingSateliteB == true)
                            if(hit [i].transform.GetComponent<DamageHandler>().sateliteB == true){
                                newTarget = hit[i].transform;
                            }
                        if( targetingOthersB == true)
                            if(hit [i].transform.GetComponent<DamageHandler>().otherCraftB == true){
                                newTarget = hit[i].transform;
                            }

                        

                        if(theTarget == null){theTarget = newTarget;}
                        
                    }
                }
            }
        }
    }

}
