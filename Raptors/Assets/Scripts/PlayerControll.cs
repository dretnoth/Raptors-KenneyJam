using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    
    public float speedMaximum, speedCurent, speedRotate, speedingUpFActor, extraSpaceRadius=0;
    //public float speedMaxCombined, speedTurboMax=2, 
    public int ammo1Current=50, ammo1Max=50;
    public int[] secondaryBank = new int[11];
    float moveVertical, moveHorizontal, rotationZ;
    Quaternion nextRotation;
    Vector3 pos, velocity;


    public bool hittingBarrierB, bulletPowerB, havingAimTargetB, secondaryReadyB;


    public float fireInterval=0.5f, secondaryInterval=2f;
    public float scanerInterval = 0.1f, scanerPullUpStreng=3, scanerRange=2, scanerColectRange = 0.5f, scanerPullUpRange = 2f;
    public float workDistanceToMotherShip = 3, aimScannerRange = 7;
    float fireTimer=0, scanerTimer=0, secondartTimer=0, distanceToMotherShip=0, distanceTOScanerTArget=0, distanceToAIMTarget=0;
    public GameObject firePrefab, secondaryPrefab;
    public Controll myControll;
    public Transform motherShip;


    Vector3 newLocation; float distanceFromCenter, spaceRadius;
    Vector3 fromOriginToObject, centerPoint;


    RaycastHit2D[] scanerHit, aimHit;
    RaycastHit2D aimSingleHit;
    public Transform scanerTarget, aimTarget; 
    Transform secondAimTarget;
    public LineRenderer lineRenderer;
    //bool lineIsOnB;
    

    


    //public int statisticNormalBulletFired=0;
    public SpriteRenderer engineSprite; float engValue=0; Color engineColor;
    //public Transform aimerCursorChorsar;
    public AudioSource fireSfx, secondarySfx, collectSfx, emptyClickSfx;
    public ParticleSystem engineParticleSystem;
    public ParticleSystem.EmissionModule myEngine;
    public GameObject hitter;

    void Start()
    {
        if(engineParticleSystem == null){
            engineParticleSystem = GetComponentInChildren<ParticleSystem>();
        }
        //myEngine = engineParticleSystem.GetComponent; // GetComponent<EmissionModule>();
    }

    
    void Update()
    {
        
        //movement
        moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");

        rotationZ -= moveHorizontal * speedRotate * Time.deltaTime; //change the z angle
		nextRotation = Quaternion.Euler (0, 0, rotationZ); //recreate the quaternion
		transform.rotation = nextRotation; //feed quaternion into our rotation

        if(moveVertical != 0){
            speedCurent += moveVertical * speedingUpFActor * Time.deltaTime;
            if(speedCurent > speedMaximum){speedCurent = speedMaximum;}
            if(speedCurent < 0){speedCurent = 0;}
            
            
            engValue = speedCurent / speedMaximum;
            
            
            engValue = speedCurent * 0.3f;
            engineSprite.transform.localScale = new Vector3(0.5f, engValue, 1);

            //myEngine.rateOverTime = (int) (speedCurent * 10);
            engineParticleSystem.emissionRate = (int) (speedCurent * 10);
        }

        
        
        

        pos = transform.position;
		velocity = new Vector3 (0, speedCurent * Time.deltaTime, 0);
		pos += transform.rotation * velocity;
		transform.position = pos;


        if(secondaryBank[0] == 3){AimScan();}
        


        //firing mechanizmus
        fireTimer += Time.deltaTime;
        if(Input.GetButton ("Fire1")){
            if(fireTimer >= fireInterval){
                fireTimer = 0;
                if(ammo1Current > 0){
                    FireNormalBullet();
                    ammo1Current--;
                }else{emptyClickSfx.Play();}               
            }
        }

        secondartTimer += Time.deltaTime;
        if(secondartTimer >= secondaryInterval){secondaryReadyB = true;}else{secondaryReadyB = false;}
        if(Input.GetButton ("Fire2")){
            if(secondartTimer >= secondaryInterval){
                secondartTimer = 0;
                if( secondaryBank[0] > 0){
                    FireSecondary(secondaryBank[0] );
                    for(int i=0; i< 10; i++){
                        secondaryBank[i] = secondaryBank[i + 1];
                    }
                }else{emptyClickSfx.Play();}                  
            }
        }


        //dont leave the battle area
        spaceRadius = Controll.GameController.spaceRadius + extraSpaceRadius;
        centerPoint = Controll.GameController.centerPoint;
        distanceFromCenter = Vector3.Distance(pos, centerPoint);
        if(distanceFromCenter > spaceRadius){
            hittingBarrierB = true;

            fromOriginToObject = pos - centerPoint;
            fromOriginToObject *= spaceRadius / distanceFromCenter;
            newLocation = centerPoint + fromOriginToObject;

            transform.position = newLocation;
        }else{hittingBarrierB = false;}

        
        //scanning for object
        scanerTimer += Time.deltaTime;
        if(scanerTimer > scanerInterval){
            scanerTimer = 0; 
            if(scanerTarget == null){                
                ScanForResources();
            }   
        }

        
        
        if(scanerTarget != null){
            if(myControll.collectCurrent < myControll.collectCapacity){

                distanceTOScanerTArget = Vector3.Distance(this.transform.position, scanerTarget.position);

                
                if( distanceTOScanerTArget < scanerPullUpRange){
                    scanerTarget.position = Vector3.MoveTowards(scanerTarget.position, this.transform.position, scanerPullUpStreng * Time.deltaTime);
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, scanerTarget.position);
                    //lineIsOnB = true;
                    //lineRenderer.enabled = true;
                }
                
                
                

                if( distanceTOScanerTArget < scanerColectRange){
                        if(scanerTarget.GetComponent<DestroyMe>() != null)
                        scanerTarget.GetComponent<DestroyMe>().InstantDestroy();
                        //if(scanerTarget != null) {scanerTarget = null;}
                        myControll.collectCurrent++;
                        collectSfx.Play();
                    }
                
                if( distanceTOScanerTArget > scanerRange + 0.05f){scanerTarget = null;}        

            }
            
            
            
        }else{
            lineRenderer.positionCount = 0;
            //lineRenderer.SetPosition(0, transform.position);
            //lineRenderer.SetPosition(1, transform.position);
        }


        if(motherShip != null){
            distanceToMotherShip = Vector3.Distance(transform.position, motherShip.position);
            if(distanceToMotherShip < workDistanceToMotherShip){
            }
            if(motherShip.GetComponent<MotherShip>().workCraftOnBoardB == true){
                transform.position = motherShip.transform.position;
            }
        }


        if(aimTarget != null){
            havingAimTargetB = true;
            
            distanceToAIMTarget = Vector3.Distance(pos , aimTarget.position);
            /*
            string theString = aimTarget.name +" p: "+ aimTarget.position + " "+ distanceToAIMTarget.ToString();
            myControll.mydata.DetecorTextInput(theString);
            */  
            }else{havingAimTargetB = false;}

        
    }

    

    void ScanForResources(){
        scanerHit = null;
        scanerHit = Physics2D.CircleCastAll(this.transform.position, scanerRange, new Vector2 (-1,1) );
        if(scanerHit != null){
             float disNew=0, disLast= scanerRange + 1;
            Vector3 myPos = this.transform.position;

            for (int i = 0; i < scanerHit.Length; i++){

                if(scanerHit[i].transform.tag == "R"){
                   disNew = Vector3.Distance(myPos, scanerHit[i].transform.position);
                    if(disNew < disLast){
                        scanerTarget = scanerHit[i].transform;
                         disLast = disNew;
                    }                    
                }
            }
        }
    }

    void AimScan(){
        //print("Aim Sanning");
        aimHit = null;
        //Vector2 direction = Vector2.
        //Vector2 dir = (Vector2)(Quaternion.Euler(0,0,degree) * Vector2.right);
        Vector2 dir = (Vector2)(transform.rotation * Vector2.right);
        //aimHit = Physics2D.RaycastAll(pos, dir, aimScannerRange );
        aimHit = Physics2D.RaycastAll(pos, transform.TransformDirection(Vector2.up), aimScannerRange );
        
        
        //aimSingleHit = null;
        aimSingleHit = Physics2D.Raycast(pos, transform.TransformDirection(Vector2.up), aimScannerRange);
        if(aimSingleHit.transform != null) {
            Instantiate(hitter, aimSingleHit.transform.position, Quaternion.identity);
            string theString = aimSingleHit.transform.name +" "+ aimSingleHit.transform.position;
            myControll.mydata.DetecorTextInput(theString);
        }else{myControll.mydata.DetecorTextInput("");}
        
        
        if(aimHit != null){
            //print("Aim hit");
            aimTarget = null;
            float distanceCurrent=aimScannerRange+1, distanceNew=aimScannerRange +1;
            for(int i=0; i< aimHit.Length; i++){
                if(aimHit [i].transform.GetComponent<DamageHandler>() != null){
                    if(aimHit [i].transform.GetComponent<DamageHandler>().warSide != this.GetComponent<DamageHandler>().warSide ){
                        if(aimHit [i].transform.GetComponent<DamageHandler>().asteroidB == true
                        || aimHit [i].transform.GetComponent<DamageHandler>().monsterB == true
                        || aimHit [i].transform.GetComponent<DamageHandler>().sateliteB == true){
                            secondAimTarget = aimHit[i].transform;
                            if(Vector3.Distance(aimHit[i].transform.position, centerPoint) < spaceRadius)
                            if(aimTarget == null){
                                aimTarget = secondAimTarget;
                            }else{
                                distanceCurrent = Vector3.Distance(pos, aimTarget.position);
                                distanceNew = Vector3.Distance(pos, secondAimTarget.position);
                                if(distanceNew < distanceCurrent){
                                    aimTarget = secondAimTarget;
                                    havingAimTargetB = true;                                    
                                }
                            }
                        }
                    }
                }
            }
        }
        if(aimTarget != null){
            //print("Aim have  t");
            myControll.EventAimChorsarGotData(aimTarget);
        }
    }


    void FireNormalBullet(){
        GameObject myBullet = (GameObject)Instantiate(firePrefab, transform.position, transform.rotation);
        //myBullet.GetComponent<DamageHandler>().warSide = this.GetComponent<DamageHandler>().warSide;
        myBullet.GetComponent<DamageHandler>().SetSide( this.GetComponent<DamageHandler>().warSide );
        if(bulletPowerB){myBullet.GetComponent<DamageHandler>().ramDamage += 1;}
        myBullet.transform.SetParent(Controll.GameController.folderForBullets.transform);
        Controll.GameController.statisticNormalBulletFired++;
        fireSfx.Play();
    }

    void FireSecondary(int option){
        GameObject myBullet = (GameObject)Instantiate(secondaryPrefab, transform.position, transform.rotation);        
        myBullet.GetComponent<SecondaryWeaponCase>().DoIt(this.GetComponent<DamageHandler>().warSide, option, havingAimTargetB, aimTarget);
        //secondarySfx.Play();
        myControll.mydata.PlaySecondaryWeaponSfx(option -1);
        myControll.statisticSecondaryFired[option-1] += 1;
    }

}
