using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCraft : MonoBehaviour
{
    public Transform theCraft;
    bool phase1JumpInB, phase2JumpOutB, phase3ColectingB, slowDownB;
    float timerPhase=0;
    public float timerDeath=5, intervalPhase=4, sizeOfCraft = 2;
    public int warSide, resorceColected, resorceTargeted=20;
    public LineRenderer lineRenderer;
    public Controll myControll;
    
    Vector3 pos, velocity, direction, centerPoint, fromOriginToObject, newLocation;
    public float speedMaximum= 1.5f, speedCurent=1.5f, speedRotate, speedingUpFActor, extraSpaceRadius=0, toCloseRange=0;
    public float fireInterval = 1f, fireRange = 4, scanerInterval = 0.1f, scanerRange=12, resorceColectingRange=1;
    public Transform newHostileTarget, resourceTargter, newAsteroidTarget, asteroidTarget;
    public GameObject firePrefab;
    float fireTimer=0, scanerTimer=0, zAngle=0, distanceToResorce, distanceToAsteroid;
    
    Quaternion desiredRot;

    RaycastHit2D[] scanerHit;
    float spaceRadius, distanceFromCenter, distanceOfTheResouceFromTheCenter, resourcePullUpStrenght=1, distanceAsteroidToCenter;
    Color myColor;

    float randomDirectionInterval=15, randomDirectionTimer=0; bool doingRandomDirectionPhaseB;

    
    
    public void StartTheCraft(int newSite, int additionalOtpion)
    {
        myControll = Controll.GameController;
        warSide = newSite;
        myColor = myControll.mydata.theCollors[warSide];
        GameObject go = (GameObject)Instantiate(myControll.mydata.jumpDriveEffect, transform.position, Quaternion.identity);
        go.GetComponent<ParticleSystem>().startColor = myColor;
        theCraft.GetComponent<DamageHandler>().SetSide(warSide);
        theCraft.gameObject.SetActive(false);
        phase1JumpInB = true;  

        myControll.statistocOthers[0] += 1;  

        //primary stats
        int value = (int)Random.Range(15, 50);
        resorceTargeted += value;
        value = (int)Random.Range(10, 60);
        theCraft.GetComponent<DamageHandler>().hpCurrent += value;
        theCraft.GetComponent<DamageHandler>().hpMax += value;

        //secondaries stats
        resorceColectingRange += (sizeOfCraft *0.5f) + 0.25f;
        resourcePullUpStrenght = speedMaximum +0.25f;
        toCloseRange = (float)sizeOfCraft + 0.25f;
    }

    
    void Update()
    {
        // special operations of the craft
        if(phase1JumpInB == true || phase2JumpOutB == true){
            timerPhase += Time.deltaTime;
            if(timerPhase > intervalPhase){
                timerPhase = 0;
                if(phase1JumpInB){
                    phase1JumpInB = false;
                    theCraft.gameObject.SetActive(true);
                    phase3ColectingB = true;
                    myControll.SpawnAPointerOnPlayerToThisOne(theCraft);
                }
                if(phase2JumpOutB){
                    phase2JumpOutB = false;
                    if(theCraft != null){
                       theCraft.gameObject.SetActive(false);
                       myControll.statistocOthers[1] += 1;   
                    }                    
                    print("Craft Jump out");
                }
            }
        }

        //kill the craft if all gone
        if(theCraft == null){
            timerDeath -= Time.deltaTime;
            if(timerDeath < 0)
                Destroy(gameObject);
            if(phase3ColectingB == true) phase3ColectingB = false;
            }
        
        
        //solving colecting
        if(phase3ColectingB){
            fireTimer += Time.deltaTime;
            scanerTimer += Time.deltaTime;
            if(scanerTimer > scanerInterval){
                scanerTimer = 0;
                Scanner();
            }


            //move forward
            pos = transform.position;
            velocity = new Vector3 (0, speedCurent * Time.deltaTime, 0);
            pos += transform.rotation * velocity;
            transform.position = pos;

            if(slowDownB){
                if(speedCurent > 0){speedCurent -= speedingUpFActor * Time.deltaTime;}
                if(speedCurent < 0) speedCurent = 0;
            }else{
                if(speedCurent < 0){speedCurent += speedingUpFActor * Time.deltaTime;}
                if(speedCurent > speedMaximum) speedCurent = speedMaximum;
            }

            
            //stay in area
            spaceRadius = Controll.GameController.spaceRadius + extraSpaceRadius;
            centerPoint = Controll.GameController.centerPoint;
            distanceFromCenter = Vector3.Distance(pos, centerPoint);
            if(distanceFromCenter > spaceRadius){

                fromOriginToObject = pos - centerPoint;
                fromOriginToObject *= spaceRadius / distanceFromCenter;
                newLocation = centerPoint + fromOriginToObject;

                transform.position = newLocation;
            }



            if(resorceColected >= resorceTargeted){
                phase3ColectingB = false;
                phase2JumpOutB = true;
                GameObject go = (GameObject)Instantiate(myControll.mydata.jumpDriveEffect, transform.position, Quaternion.identity);
                go.GetComponent<ParticleSystem>().startColor = myColor;
            }

            
        }

        //doing collecting
        if(phase3ColectingB)
        if(resourceTargter != null){
            TurnTovardsTheTarget(resourceTargter);

            //collecting resources
            distanceToResorce = Vector3.Distance(this.transform.position, resourceTargter.position);                
            if( distanceToResorce < resorceColectingRange){
                    resourceTargter.position = Vector3.MoveTowards(resourceTargter.position, this.transform.position, resourcePullUpStrenght * Time.deltaTime);
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, resourceTargter.position);
                    
                    if( distanceToResorce < 0.5f){
                        if(resourceTargter.GetComponent<DestroyMe>() != null)
                        resourceTargter.GetComponent<DestroyMe>().InstantDestroy();
                        resorceColected ++;
                        lineRenderer.positionCount = 0;
                    }
                }
            
            if(doingRandomDirectionPhaseB){
                doingRandomDirectionPhaseB = false;
                randomDirectionTimer = 0;
            }
            slowDownB = false;

        }else{
            lineRenderer.positionCount = 0;


            
            if(asteroidTarget == null)
                if(newAsteroidTarget != null){ asteroidTarget = newAsteroidTarget;}
            if(asteroidTarget != null){
                distanceAsteroidToCenter  = Vector3.Distance(centerPoint, asteroidTarget.position);
                if(distanceAsteroidToCenter < spaceRadius + 1){asteroidTarget = null;}
            }

            if(asteroidTarget != null){
                TurnTovardsTheTarget(asteroidTarget);
                distanceToAsteroid = Vector3.Distance(pos, asteroidTarget.position);
                if(distanceToAsteroid < toCloseRange){
                    slowDownB = true;
                }else{slowDownB = false;}

                if(doingRandomDirectionPhaseB){
                    doingRandomDirectionPhaseB = false;
                    randomDirectionTimer = 0;
                }

                if(fireTimer > fireInterval){
                    fireTimer = 0;
                    LaunchAWeapon();
                }

            }else{

                if(newHostileTarget != null){
                    TurnTovardsTheTarget(newHostileTarget);
                    distanceToAsteroid = Vector3.Distance(pos, newHostileTarget.position);
                    if(distanceToAsteroid < toCloseRange){
                        slowDownB = true;
                    }else{slowDownB = false;}
                    
                    if(fireTimer > fireInterval){
                        fireTimer = 0;
                        LaunchAWeapon();
                    }

                }else{
                    if(doingRandomDirectionPhaseB == false){
                        doingRandomDirectionPhaseB = true;
                        randomDirectionTimer = 0;
                        zAngle = transform.rotation.z + Random.Range(0 , 360);
                    }
                    randomDirectionTimer ++;
                    if(randomDirectionTimer > randomDirectionInterval) doingRandomDirectionPhaseB = false;
                    desiredRot = Quaternion.Euler (0, 0, zAngle);
                    transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, speedRotate * Time.deltaTime);
                    slowDownB = false;
                }

                
            }

        }

    }

    void TurnTovardsTheTarget(Transform theTarget){
        direction = theTarget.transform.position - transform.position; //distant vector
		direction.Normalize ();
		zAngle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg - 90; //this return radians, 0 angle facing right
		desiredRot = Quaternion.Euler (0, 0, zAngle);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, speedRotate * Time.deltaTime);
    }

    void LaunchAWeapon(){
        GameObject myBullet = (GameObject)Instantiate(firePrefab, transform.position, transform.rotation);        
        if(myBullet != null){
            if(myBullet.GetComponent<DamageHandler>() != null)
                myBullet.GetComponent<DamageHandler>().SetSide( this.GetComponentInChildren<DamageHandler>().warSide );
            myBullet.transform.SetParent(Controll.GameController.folderForBullets.transform);    
        }
        
    }

    void Scanner(){
        scanerHit = null;
        scanerHit = Physics2D.CircleCastAll(this.transform.position, scanerRange, new Vector2 (-1,1) );
        if(scanerHit != null){
             float disNew=0, disLast= scanerRange + 1;
             float disNew2=0, disLast2= scanerRange + 1;
             float disNew3=0, disLast3= scanerRange + 1;

            for (int i = 0; i < scanerHit.Length; i++){

                //resource
                if(scanerHit[i].transform.tag == "R"){
                    disNew = Vector3.Distance(pos, scanerHit[i].transform.position);
                    distanceOfTheResouceFromTheCenter = Vector3.Distance(scanerHit[i].transform.position, centerPoint);
                    if(distanceOfTheResouceFromTheCenter < spaceRadius +1)
                        if(disNew < disLast){
                            resourceTargter = scanerHit[i].transform;
                            disLast = disNew;
                        }                    
                }

                //hostiles and asteroids
                if(scanerHit [i].transform.GetComponent<DamageHandler>() != null){
                    if(scanerHit [i].transform.GetComponent<DamageHandler>().bulletB == false)
                    if(scanerHit [i].transform.GetComponent<DamageHandler>().warSide != warSide){

                        if(scanerHit [i].transform.GetComponent<DamageHandler>().asteroidB == true){
                            disNew2 = Vector3.Distance(pos, scanerHit[i].transform.position);
                            distanceAsteroidToCenter  = Vector3.Distance(centerPoint, scanerHit[i].transform.position);
                            if(distanceAsteroidToCenter < spaceRadius + 1)
                            if(disNew2 < disLast2){
                                newAsteroidTarget = scanerHit[i].transform;
                                disLast2 = disNew2;
                                } 
                            }
                    }else{
                            disNew3 = Vector3.Distance(pos, scanerHit[i].transform.position);
                            if(disLast3 < fireRange +1)
                            if(disNew3 < disLast3){
                                newHostileTarget = scanerHit[i].transform;
                                disLast3 = disNew3;
                                } 
                    }
                }
            }

            
            
            
        }
    }

}
