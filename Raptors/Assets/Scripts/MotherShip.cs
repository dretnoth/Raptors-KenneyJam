using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherShip : MonoBehaviour
{
    public float mainProcesInterval=1, secondaryProcesInterval = 0.5f;

    public int recievedResorces=0, fuelStoraged=0, fuelTargeted=100 , resorceDeficit=0;
    int storedRepairPower=0;
    

    public bool fuelFullAndReadyB= false, conectedToCraftB = false, workCraftOnBoardB, procesigFuelB;

    public float workDistanceCurent=0, workDistanceMax=4, inBasedistance =2.2f;
    public float fuelProcesInterval=3f, timerFuelProces=0;
    public Transform theWorkCraft;
    public DamageHandler myDamageHandler;
    float timerMainOperations=0, timerSecondOperations=0;

    
    public int[] levelOfSecondaryWeapons;
    public int levelOfExtraSecArmamentHolder, howmanySecWeaponsCanCraftHold, extraRepairEfficiency;
    public int levelOfResuplyA1;
    public int levelOfCraftArmor, levelOfCraftManevrublable, levelOfCraftMaxSpeed;


    public int pbHp, pmHp, pbA1, pmA1, mbHp, mcHp;
    public float pbSpd, pmSpd, pbRot, pmRot, pbAcc, pmAcc, pbfrA1, pmfrA1, pullUpR, pullUpS;
    public bool bulletPowerB;

    public int[] secondaries = new int [21];
    public bool[] secondaryOnB = new bool[10];
    public LineRenderer lineRenderer;

    public SpriteRenderer[] basepaintparts;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerMainOperations += Time.deltaTime;
        if(timerMainOperations >= mainProcesInterval){
            timerMainOperations = 0;
            
            if(recievedResorces > 0 && fuelFullAndReadyB == false){
                recievedResorces--;
                //fuelStoraged++;
                procesigFuelB = true;
            }

            if(recievedResorces > 0 && resorceDeficit > 0){
                recievedResorces --;
                resorceDeficit--;
            }

            if(myDamageHandler.hpCurrent < myDamageHandler.hpMax){
                myDamageHandler.hpCurrent += 1 + extraRepairEfficiency;                
                storedRepairPower--;
                if(storedRepairPower < 0){
                    storedRepairPower += 10 + 5*extraRepairEfficiency;
                    resorceDeficit ++;
                }
            }


        }

        timerSecondOperations +=Time.deltaTime;
        if(timerSecondOperations >= secondaryProcesInterval){
            timerSecondOperations = 0;

            if(conectedToCraftB)
            if(theWorkCraft != null){
                if(Controll.GameController.collectCurrent > 0){
                    Controll.GameController.collectCurrent--;
                    recievedResorces++;
                }
                
                if(theWorkCraft.GetComponent<DamageHandler>().hpCurrent < theWorkCraft.GetComponent<DamageHandler>().hpMax){
                    theWorkCraft.GetComponent<DamageHandler>().hpCurrent += 1 + 1 * extraRepairEfficiency;
                    if(theWorkCraft.GetComponent<DamageHandler>().hpCurrent > theWorkCraft.GetComponent<DamageHandler>().hpMax){
                        theWorkCraft.GetComponent<DamageHandler>().hpCurrent = theWorkCraft.GetComponent<DamageHandler>().hpMax;
                    }
                }

                if(theWorkCraft.GetComponent<PlayerControll>().ammo1Current < theWorkCraft.GetComponent<PlayerControll>().ammo1Max){
                    theWorkCraft.GetComponent<PlayerControll>().ammo1Current += 2 + levelOfResuplyA1;
                    if(theWorkCraft.GetComponent<PlayerControll>().ammo1Current > theWorkCraft.GetComponent<PlayerControll>().ammo1Max){
                        theWorkCraft.GetComponent<PlayerControll>().ammo1Current = theWorkCraft.GetComponent<PlayerControll>().ammo1Max;
                    }
                }

                
                SecondaryAmmoResuplyProtocol();
                CheckForUpdateTheCraft();
            }

        }//second operations

        if(procesigFuelB){
            timerFuelProces +=Time.deltaTime;
            if(timerFuelProces >= fuelProcesInterval){
                timerFuelProces = 0;
                fuelStoraged++;
                procesigFuelB = false;
            }
        }


        if(theWorkCraft != null){
            workDistanceCurent = Vector3.Distance(this.transform.position, theWorkCraft.position);
            if(workDistanceCurent <= workDistanceMax){
                conectedToCraftB = true;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, theWorkCraft.position);
            }else{
                conectedToCraftB = false;
                lineRenderer.positionCount = 0;
                }
        }else{lineRenderer.positionCount = 2;}


        if(fuelFullAndReadyB == false){
            if(fuelStoraged >= fuelTargeted){
                fuelFullAndReadyB = true;
                print("Fuel colected, ready to go home! Return to base;");
                Controll.GameController.mydata.fanfareSfx.Play();
            }
        }

        if(fuelFullAndReadyB){
            if(workCraftOnBoardB == false){
                if(workDistanceCurent <= inBasedistance){
                    workCraftOnBoardB = true;
                    //theWorkCraft.SetParent(this.transform);
                    Controll.GameController.EventBaseJumpOut();
                }
            }
        }
    }


    public void DoUpgrade(int option, int value){
        bool doneB =false;

        //reduce fire interval of mother ship cannon
        if(option == 0){
            if(myDamageHandler.turrets[0].GetComponent<Turret>().fireInterval > 0.5f)
            for(int i =0; i< myDamageHandler.turrets.Length; i++){
                myDamageHandler.turrets[i].GetComponent<Turret>().fireInterval -= 0.5f;
            }
            doneB = true;
        }

        //hp player / base
        if(option == 1){
            pmHp += pbHp;
            mcHp += mbHp/2;
            doneB = true;
            myDamageHandler.hpMax = mcHp;
            myDamageHandler.hpCurrent += mbHp/2;
        }

        //max speed Of craft
        if(option == 2){
            pmSpd += 1;
            doneB = true;            
        }

        //flight control
        if(option == 3){
            pmAcc += 1;            
            pmRot += 25;
            doneB = true;
        }

        //gun fire rate
        if(option == 4){
            pmfrA1 -= 0.1f; 
            doneB = true;                       
        }

        //Gun magazine
        if(option == 5){
            pmA1 += pbA1/2;  
            doneB = true;          
        }

        //workraft cargo storage
        if(option == 6){
            Controll.GameController.collectCapacity += 5;  
            doneB = true;          
        }

        //bullets resuply
        if(option == 7){
            levelOfResuplyA1 += 1; 
            doneB = true;           
        }

        //fuel process speed
        if(option == 8){
            fuelProcesInterval -= 1; 
            Controll.GameController.collectCapacity += 1; 
            workDistanceMax += 1;
            doneB = true; 
        }

        //BulletPower
        if(option == 9){
            bulletPowerB = true;   
            doneB = true;       
        }

        //extra secondary space
        if(option == 10){
            levelOfExtraSecArmamentHolder++;
            howmanySecWeaponsCanCraftHold++; 
            doneB = true;         
        }

        //ShotgunShell
        if(option == 11){
            levelOfSecondaryWeapons[0]++;
            howmanySecWeaponsCanCraftHold++;
            doneB = true;
            secondaryOnB[0] = true;           
        }

        //canon
        if(option == 12){
            levelOfSecondaryWeapons[1]++;
            howmanySecWeaponsCanCraftHold++;
            doneB = true;
            secondaryOnB[1] = true;             
        }

        //thorpedo
        if(option == 13){
            levelOfSecondaryWeapons[2]++;
            howmanySecWeaponsCanCraftHold++;  
            doneB = true;  
            secondaryOnB[2] = true;         
        }

        //sentry
        if(option == 14){
            levelOfSecondaryWeapons[3]++;
            howmanySecWeaponsCanCraftHold++;  
            doneB = true;
            secondaryOnB[3] = true;           
        }

        
        // faster respawn of player
        if(option == 15){
            Controll.GameController.playerIsDeadInterval = Controll.GameController.playerIsDeadInterval / 2; 
            doneB = true;           
        }

        
        // turets turn faster
        if(option == 16){
            for(int i=0; i<myDamageHandler.turrets.Length; i++){
                if(myDamageHandler.turrets[i] != null)
                    myDamageHandler.turrets[i].GetComponent<Turret>().speedRotate *= 1.5f; 
            }
            doneB = true;
        }

        //second operations interval = faster repair resupky
        if(option == 17){
            secondaryProcesInterval /=2;
            doneB = true;
        }
        
        //HP extra repair efecienci
        if(option == 18){
            extraRepairEfficiency ++;
            pmHp += 1;
            doneB = true;
        }




        if(howmanySecWeaponsCanCraftHold > 20) {
          howmanySecWeaponsCanCraftHold = 20; 
          Debug.Log("breach on value how manny weapon one can spawn" + levelOfSecondaryWeapons[0] +
          levelOfSecondaryWeapons[1]+ levelOfSecondaryWeapons[2]+ levelOfSecondaryWeapons[3]+ levelOfSecondaryWeapons[4]);
        }
        
        
        if(doneB == false){Debug.Log("Doing upgrade: "+ option +" failed!" );}

    }


    public void SetBaseStatistic(){
        //for level up purpose and upgrading the craft on go
        PlayerControll pc = theWorkCraft.GetComponent<PlayerControll>();

        pbHp = theWorkCraft.GetComponent<DamageHandler>().hpMax;
        pmHp = pbHp;
        mbHp = myDamageHandler.hpMax;
        mcHp = mbHp;
        pbSpd = theWorkCraft.GetComponent<PlayerControll>().speedMaximum;
        pmSpd = pbSpd;
        pbRot = theWorkCraft.GetComponent<PlayerControll>().speedRotate;
        pmRot = pbRot;
        pbAcc = theWorkCraft.GetComponent<PlayerControll>().speedingUpFActor;
        pmAcc = pbAcc;
        pbA1 = theWorkCraft.GetComponent<PlayerControll>().ammo1Max;
        pmA1 = pbA1;
        pbfrA1 = theWorkCraft.GetComponent<PlayerControll>().fireInterval;
        pmfrA1 = pbfrA1;
        pullUpR = pc.scanerPullUpRange;
        pullUpS = pc.scanerPullUpStreng;
    }


    public void CheckForUpdateTheCraft(){
        DamageHandler craftDH = theWorkCraft.GetComponent<DamageHandler>();
        PlayerControll pc = theWorkCraft.GetComponent<PlayerControll>();

        if(theWorkCraft.GetComponent<DamageHandler>().hpMax < pmHp){
            theWorkCraft.GetComponent<DamageHandler>().hpMax = pmHp;
        }
        if(theWorkCraft.GetComponent<PlayerControll>().speedMaximum < pmSpd){
            theWorkCraft.GetComponent<PlayerControll>().speedMaximum = pmSpd;
        }
        if(theWorkCraft.GetComponent<PlayerControll>().speedRotate < pmRot){
            theWorkCraft.GetComponent<PlayerControll>().speedRotate = pmRot;
        }
        if(theWorkCraft.GetComponent<PlayerControll>().speedingUpFActor < pmAcc){
            theWorkCraft.GetComponent<PlayerControll>().speedingUpFActor = pmAcc;
        }
        if(theWorkCraft.GetComponent<PlayerControll>().ammo1Max < pmA1){
            theWorkCraft.GetComponent<PlayerControll>().ammo1Max = pmA1;
        }
        if(theWorkCraft.GetComponent<PlayerControll>().fireInterval > pmfrA1){
            theWorkCraft.GetComponent<PlayerControll>().fireInterval = pmfrA1;
        }
        if(bulletPowerB)
        if(theWorkCraft.GetComponent<PlayerControll>().bulletPowerB == false){
            theWorkCraft.GetComponent<PlayerControll>().bulletPowerB = true;
        }

        if(pc.scanerPullUpRange < pullUpR){
            pc.scanerPullUpRange = pullUpR;
            pc.scanerPullUpStreng = pullUpS;
        }
    }

    void SecondaryAmmoResuplyProtocol(){
        PlayerControll pc = theWorkCraft.GetComponent<PlayerControll>();
        //because standart dealing not enought

        bool doneB = false;
        int chosenOne = 0, k=0;
        //int[] pool = new int[21];
        //for(int i=0; i<21; i++){pool[i] = secondaries[i];}


        //int[] secondariesToLad = new int[]
        if(howmanySecWeaponsCanCraftHold == 0) print("howmanySecWeaponsCanCraftHold is 0");
        //print("Launching sw input protocol");


        int[] mustSpawn = new int [levelOfSecondaryWeapons.Length];
        int[] optionsToSpawn = new int [levelOfSecondaryWeapons.Length];
        int candidates =0;
        for(int i=0; i< levelOfSecondaryWeapons.Length; i++){
            mustSpawn[i] = levelOfSecondaryWeapons[i]; 
            optionsToSpawn[i] =0;
            candidates += mustSpawn[i];
        }
        if(candidates < 1){print("We have problem with candidates: "+ candidates);}
        
        
        
        


        for(int i=0; i< howmanySecWeaponsCanCraftHold; i++){
            if(doneB == false){


                //now i try to deal with the weapon load
                if(candidates > 0)
                if(pc.secondaryBank[i] == 0){

                    //clearing the field for rulete
                    k=0;
                    for(int j=0; j< levelOfSecondaryWeapons.Length; j++){
                        optionsToSpawn[j] =0;
                        if(secondaryOnB[j] == true){ optionsToSpawn[k] = j; }
                        

                        }
                    if(k > 0){
                        chosenOne = (int)Random.Range(-0.3f, k +0.3f);
                        chosenOne = optionsToSpawn[chosenOne];
                        if(mustSpawn[chosenOne] > 0){
                            doneB = true;
                            pc.secondaryBank[i] = chosenOne+1; // + 1; // because we dont do 0?
                            print("Putted in secondary in new mod frst try: " + chosenOne+1);
                        }
                    }
                }


                if(pc.secondaryBank[i] != 0){
                    mustSpawn[pc.secondaryBank[i] -1] --;
                    candidates --;
                }

                

                //this one function to certain extend
                if(pc.secondaryBank[i] == 0){
                    for(int j=0; j< 20; j++){
                        if(doneB == false){
                            chosenOne = (int)Random.Range(0, levelOfSecondaryWeapons.Length) ;                            
                            //chosenOne = (int)Random.Range(0, levelOfSecondaryWeapons.Length-0.7f) ; 
                            //if(chosenOne =) 
                            if(secondaryOnB[chosenOne] == true){
                                doneB = true;
                                pc.secondaryBank[i] = chosenOne + 1; // + 1; // because we dont do 0?
                                //print("Putted in secondary is: " + chosenOne);
                            }
                        }
                    }
                }

                //this one work to
                if(pc.secondaryBank[i] == 0){ 
                    pc.secondaryBank[i] = 3; 
                    print("WS third pull out: " + 3);
                    }


            }//false
        }
        
        

        
        
    }

    public void ChangeThoseSprites(int warSide){
        //int warSide = myDamageHandler.warSide;
        Color myColor = Controll.GameController.mydata.theCollors[warSide];

        for(int i=0; i< basepaintparts.Length; i++){
            if(basepaintparts[i] != null){
                basepaintparts[i].material.color = myColor;
            }
        }
    }

}
