using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.UI;
using System.Text;

public class Data : MonoBehaviour
{
    public static Data data;
    
    public bool menuIsOnB= true, hudIsOnB = false, hudPlayerB, hudBaseB;
    public bool abortGAmeQuestionB, exitGameQuestionB, databaseIsOpenB; 


    public GameObject basePrefab, playerPrefab, asteroidSmallPrefab, asteroidLargePrefab, resourcePrefab;
    public GameObject[] monstersPrefabs, otherHazards;
    public GameObject jumpDriveEffect, pointer;
    public Color[] theCollors;
    
    
    public int[] xpOfTargetLevels;

    public Transform menuPanel, startInfoTextPanel, aboutThisGAmeTExtPanel, collorsButtonsPanel;
    public Transform buttonLaunchNewGame, buttonContinueGame, buttonAbortGame, buttonEndApp;
    public Transform buttonAboutThisGame;
    public Transform[] buttonColor;
    public Text xpValueText; public Slider xpValueSlider;
    public Image craftDisplayOnMenu;
    public Transform HudPanel, HudPanelPlayer, HudPanelBAse, HudPanelSpeed;
    public Image playerImageHp, playerImageR, playerImageAmmo1 ,playerImageS;
    public Text playerTextHp, playerTextR, playerTextAmmo1, playerTextS;
    float hp1c, hp1m, r1c, r1m, a1c, r2s, r2d, a1m, hp2c, hp2m, xp, sc, sm;
    public float xplastLevel=0;
    float valueHp=0, valueR=0, valueAmmo1=0, valueHpBase=0, valueXpForLevel=0, valueSpeed=0; 
    float fuelProcesvalue=0;
    public Image baseImageHp, colectedFuelImage, fuelProcesImage, levelImage, returnToBaseImage;
    public Text baseTextHp, colectedFuelText, levelText;
    public Transform respawnPanel; public Image respawnImage; public float valueRespawn=0;
    public Transform endPanel, endTextPanel1, endTextPanel2;
    public bool endTextPanel2DisplayedB, returnToShipDisplayedB, respawnDisplayIsOnB, upgTextIsDisplayedB; 
    public Text endText1, endText2, upgradeText, detectorText;
    public Image endImageOnEndText;
    public Sprite[] endSpriteToEndPanel;
    public Transform QuestionPanenl;


    public Transform theDatabasePanel, theButtonsPanelContent;
    public Transform[] theDatabaseButtonsArray; public int nOfButtonsMade=0; public bool databaseCreatedB;
    public GameObject prefabDatabaseButton;
    public Image[] databaseImages; public Image secondaryWeaponIsReadyImage; bool secondaryWeaponIsReadyB=false;
    public Text databaseTextName, databaseTextData;
    [SerializeField]
    public DataOfStuff[] databaseOfStuff;
    public SecondaryWeponSlotOnHud[] swOnHud;
    [SerializeField]
    public UpgradeData[] Upgrades;
    public int[] upgradeDoneArray, secondaryWeaponsOnThoseIds;
    public string stringOfUpgrades;
    string extraString;
    public string[] aboutGameStrings, endMesages;
    public int shovedAboutGameStringIs = -1; public Text aboutThisGameText;
    

    public Transform SliderPanelZoom, zoomButton; public bool zoomPageOnB; public float cameraZoom=5;
    public Slider slinerOnZoom;
    


    


    public Transform motherShip, player;
    public Controll myControl;

    


    public AudioSource[] musicsBase, musicSort, secondaryWeaponsAS;
    public int selectedMusic=0, numberOfMusic=4;
    public AudioSource death, clickSfx, levelUpSfx, fanfareSfx, victorySfx, defeatSfx;


    [System.Serializable]    
    public class DataOfStuff {
        public string name;
        [TextArea]
        public string details;
        public Sprite[] images = new Sprite[3];
    }

    [System.Serializable]
    public class UpgradeData{
        public string name;
        public int id, maxLevels;    
    }

    
    public void CloseOpenMenu(){
        menuIsOnB = false;
        menuPanel.gameObject.SetActive(false);
        HudOpenClose(true);        
    }
    
    public void OpenMenu(){
        menuPanel.gameObject.SetActive(true);
        menuIsOnB = true;
        HudOpenClose(false);
        if(myControl.gameIsOnB == true){
            buttonLaunchNewGame.gameObject.SetActive(false);
            buttonContinueGame.gameObject.SetActive(true);
            buttonAbortGame.gameObject.SetActive(true);
        }else{
            buttonLaunchNewGame.gameObject.SetActive(true);
            buttonContinueGame.gameObject.SetActive(false);
            buttonAbortGame.gameObject.SetActive(false);
        }
    }

    private void Update() {
        
        //hud dispalay
        if(hudIsOnB){
            if(player != null){
                if(hudPlayerB == false){
                    HudPanelPlayer.gameObject.SetActive(true);
                    HudPanelSpeed.gameObject.SetActive(true);
                    hudPlayerB = true;
                }
                hp1c = player.GetComponent<DamageHandler>().hpCurrent;
                hp1m = player.GetComponent<DamageHandler>().hpMax;
                valueHp = hp1c / hp1m;
                playerTextHp.text = hp1c.ToString() +" / "+ hp1m.ToString() +" HP";
                playerImageHp.fillAmount = valueHp;

                r1c = myControl.collectCurrent;
                r1m = myControl.collectCapacity;
                valueR = r1c / r1m;
                playerTextR.text = r1c.ToString() +" / "+ r1m.ToString() +" R";
                playerImageR.fillAmount = valueR;

                a1c = player.GetComponent<PlayerControll>().ammo1Current;
                a1m = player.GetComponent<PlayerControll>().ammo1Max;
                valueAmmo1 = a1c / a1m;
                playerTextAmmo1.text = a1c.ToString() +" / "+ a1m.ToString( )+" A";
                playerImageAmmo1.fillAmount = valueAmmo1;

                sc = player.GetComponent<PlayerControll>().speedCurent;
                sm = player.GetComponent<PlayerControll>().speedMaximum;
                valueSpeed = sc / sm;
                playerTextS.text = ( (int)(sc * 10f) ).ToString();
                playerImageS.fillAmount = valueSpeed;

                for(int i=0; i<10; i++){
                    swOnHud[i].MakeChange(  player.GetComponent<PlayerControll>().secondaryBank[i] );
                }

                if(player.GetComponent<PlayerControll>().secondaryReadyB == true 
                    && player.GetComponent<PlayerControll>().secondaryBank[0] > 0){
                    if(secondaryWeaponIsReadyB == false){
                        secondaryWeaponIsReadyImage.gameObject.SetActive(true);
                        secondaryWeaponIsReadyB = true;
                    }
                }else{
                    if(secondaryWeaponIsReadyB == true ||
                        player.GetComponent<PlayerControll>().secondaryBank[0] == 0){
                        secondaryWeaponIsReadyImage.gameObject.SetActive(false);
                        secondaryWeaponIsReadyB = false;
                    }
                }

                
            }else if(hudPlayerB == true){
                    HudPanelPlayer.gameObject.SetActive(false);
                    HudPanelSpeed.gameObject.SetActive(false);
                    hudPlayerB = false;
                }

            

            if(motherShip != null){
                if(hudBaseB == false){
                    HudPanelBAse.gameObject.SetActive(true);
                    hudBaseB = true;
                }
                hp2c = motherShip.GetComponent<DamageHandler>().hpCurrent;
                hp2m = motherShip.GetComponent<DamageHandler>().hpMax;
                valueHpBase = hp2c / hp2m;
                baseTextHp.text = hp2c.ToString() +" / "+ hp2m.ToString() +" HP";
                baseImageHp.fillAmount = valueHpBase;

                r1c = motherShip.GetComponent<MotherShip>().fuelStoraged;
                r1m = motherShip.GetComponent<MotherShip>().fuelTargeted;
                valueR = r1c / r1m;
                r2s = motherShip.GetComponent<MotherShip>().recievedResorces;
                r2d = motherShip.GetComponent<MotherShip>().resorceDeficit;
                colectedFuelText.text = "(+"+r2s.ToString()+"-"+r2d.ToString() +") "+ r1c.ToString() +" / "+ r1m.ToString() +" F";
                colectedFuelImage.fillAmount = valueR;

                fuelProcesvalue = motherShip.GetComponent<MotherShip>().timerFuelProces / motherShip.GetComponent<MotherShip>().fuelProcesInterval;
                fuelProcesImage.fillAmount = fuelProcesvalue;

                

                if(motherShip.GetComponent<MotherShip>().fuelFullAndReadyB){
                    if(returnToShipDisplayedB == false){
                        returnToBaseImage.gameObject.SetActive(true);
                        returnToShipDisplayedB = true;
                    }
                }else{
                    if(returnToShipDisplayedB == true){
                        returnToBaseImage.gameObject.SetActive(false);
                        returnToShipDisplayedB = false;
                    }
                }
            }else if(hudBaseB == true){
                    HudPanelBAse.gameObject.SetActive(false);
                    hudBaseB = false;
                }

            extraString = (myControl.nextLevelFuel - r1c ).ToString();
            levelText.text = myControl.level.ToString()  +" LV - " + myControl.nextLevelFuel.ToString() +" ("+extraString+")";
            valueXpForLevel = ( (r1c -xplastLevel) / (r1c -xplastLevel) );
            levelImage.fillAmount = valueXpForLevel;

            if(myControl.playerIsDestroyedB == true){
                if(respawnDisplayIsOnB == false){
                    respawnPanel.gameObject.SetActive(true);
                    respawnDisplayIsOnB = true;
                }else{
                    respawnImage.fillAmount = valueRespawn;
                }
            }else{
                if(respawnDisplayIsOnB == true){
                    respawnPanel.gameObject.SetActive(false);
                    respawnDisplayIsOnB = false;
                }
            }

        }//HUd
    }

    public void HudOpenClose(bool theOption){
        if(theOption == true){
            HudPanel.gameObject.SetActive(true);
            hudIsOnB = true;
        }else{
            HudPanel.gameObject.SetActive(false);
            hudIsOnB = false;
            }
        PlayClickSfx();
    }

    public void SwitchTheTextDisplay(){
        if(upgTextIsDisplayedB){
            upgradeText.gameObject.SetActive(false);
            upgTextIsDisplayedB = false;
        }else{
            upgradeText.gameObject.SetActive(true);
            upgTextIsDisplayedB = true;
        }
        PlayClickSfx();
    }
    private void Awake() {
        data = this;
    }

    public void ButtonOpenDatabase(){
        if(databaseIsOpenB == false){
            theDatabasePanel.gameObject.SetActive(true);
            databaseIsOpenB = true;
        }
        else{
            theDatabasePanel.gameObject.SetActive(false);  
            databaseIsOpenB = false;  
        }
        PlayClickSfx();
    }

    public void ButtonOnDatabaseClick(int hisId){
        PlayClickSfx();
        databaseTextName.text = databaseOfStuff[hisId].name;
        databaseTextData.text = databaseOfStuff[hisId].details;
        for(int i=0; i<3; i++)
            databaseImages[i].sprite = databaseOfStuff[hisId].images[i];
    }

    public void PlayClickSfx(){
        if(clickSfx != null)
            clickSfx.Play();
    }

    public void PlaySecondaryWeaponSfx(int option){
        if(secondaryWeaponsAS[option] != null){
            secondaryWeaponsAS[option].Play();
        }
    }

    public void ProcessToCreateTheDatabaseOfStuff(){
        if(databaseCreatedB == false){
            GameObject go;
            databaseCreatedB = true;
            if(theDatabaseButtonsArray.Length < databaseOfStuff.Length){
                theDatabaseButtonsArray = new Transform[databaseOfStuff.Length +1];
            }

            for(int i=0; i< databaseOfStuff.Length; i++){
                if(databaseOfStuff[i] != null){
                    go = (GameObject)Instantiate(prefabDatabaseButton, theButtonsPanelContent.position, Quaternion.identity);
                    go.transform.SetParent(theButtonsPanelContent);
                    go.GetComponent<ButtonOnDatabaseStuff>().Inicialization(i, this);
                    theDatabaseButtonsArray[i] = go.transform;
                    nOfButtonsMade++;
                }
            }
        }
    }

    public void DisplayStringOfUpgrades(){
        stringOfUpgrades = "";
        for(int i=0; i<Upgrades.Length; i++){
            if(upgradeDoneArray[i] > 0){
                stringOfUpgrades += Upgrades[i].name;
                if(upgradeDoneArray[i] > 1){
                    stringOfUpgrades += "("+ upgradeDoneArray[i].ToString() +")";
                }
                stringOfUpgrades += ", ";
            }
        }
        upgradeText.text = stringOfUpgrades;
    }

    public void DetecorTextInput(string theString){
        detectorText.text = theString;
    }

}