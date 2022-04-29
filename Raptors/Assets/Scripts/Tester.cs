using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public bool putInUpgradeB;     public int thisUpdate= 13;
    public bool spawnMonsterB; public int thisMonster = 3;
    public bool spawnHazardB; public int thisHazard =1;
    public bool setFuelLevelToB; public int thisLevel = 6;
    public bool putInShotgunB, putInCanonB, putInThorpedoB, putInSateliteB;


    public Controll myControl;
    public Data mydata;

    // Update is called once per frame
    void Update()
    {
        if(myControl.gameIsOnB == true)
        if(mydata.motherShip != null){
            if(putInUpgradeB){
                putInUpgradeB = false;
                PutThisUpdate(thisUpdate);
            }

            if(putInShotgunB){
                putInShotgunB = false;
                PutThisUpdate(11);
            }

            if(putInCanonB){
                putInCanonB = false;
                PutThisUpdate(12);
            }

            if(putInThorpedoB){
                putInThorpedoB = false;
                PutThisUpdate(13);
            }

            if(putInSateliteB){
                putInSateliteB = false;
                PutThisUpdate(14);
            }

            if(setFuelLevelToB){
                setFuelLevelToB = false;
                mydata.motherShip.GetComponent<MotherShip>().fuelStoraged = mydata.xpOfTargetLevels[thisLevel];
            }

        }// mother ship


        if(myControl.gameIsOnB == true){
            if(spawnMonsterB){
                spawnMonsterB = false;
                SpawnMonster(thisMonster);
            }
            if(spawnHazardB){
                spawnHazardB = false;
                SpaenHazard(thisHazard);
            }
            
        }

    }

    void PutThisUpdate(int chosenUpgrade){
        mydata.upgradeDoneArray[chosenUpgrade]++;
        mydata.motherShip.GetComponent<MotherShip>().DoUpgrade(mydata.Upgrades[chosenUpgrade].id, mydata.upgradeDoneArray[chosenUpgrade]);
    }

    void SpawnMonster(int theOption){
        Vector3 myposition = new Vector3(0,15,0);
        GameObject go = (GameObject)Instantiate(mydata.monstersPrefabs[theOption], myposition, Quaternion.identity);
        float z  = 180 + Random.Range(-50, 50);
        go.transform.eulerAngles = new Vector3(0,0,z);
        go.GetComponent<DamageHandler>().SetSide(myControl.collorOfMonsterSideA);
        go.GetComponent<Enemy>().myControll = myControl;
        //go.transform.SetParent(myControl.folderForObjects);
        myControl.SpawnAPointerOnPlayerToThisOne(go.transform);
    }

    void SpaenHazard(int theOption){
        Vector3 myposition = new Vector3(0,15,0);
        GameObject go = (GameObject)Instantiate(mydata.otherHazards[theOption], myposition, Quaternion.identity);
        float z  = 180 + Random.Range(-50, 50);
        go.transform.eulerAngles = new Vector3(0,0,z);
        go.GetComponent<DamageHandler>().SetSide(myControl.colorOfRandomGuy);
        go.GetComponent<Enemy>().myControll = myControl;
        //go.transform.SetParent(myControl.folderForObjects);
        myControl.SpawnAPointerOnPlayerToThisOne(go.transform);
    }
    
}
