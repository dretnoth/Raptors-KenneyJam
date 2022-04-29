using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.UI;
using System.Text;

public class Controll : MonoBehaviour
{
    public static Controll GameController;

    public int level=1, nextLevelFuel=10, targetFuel = 100, collectCurrent=0, collectCapacity=5;
    public float spaceRadius = 30, gameInterval=1, gameTik=0, playerIsDeadInterval=10, posholdInterval=3;
    public bool gameIsOnB, playerIsDestroyedB, cameraMovedB, posholdOperationsB, beginingOfGameB, endingOfGameB;
    public bool victoryB, defeatB;
    public int collorOfPlayer = 1, collorOfMonsterSideA=6, colorOfSateliteSentries = 7, colorOfRandomGuy=8;
    public int asteroidsSmallOnSpace, asteroidSmallMax=10, asteroidSmallIncrease=5;
    public int asteroidLargeMax=1, asteroidLargeIncrease=5;
    public int monstersValuePresent=0, monstersValueMax=0, monsterValueLevelIncrease=2;
    public bool[] monsterAlovedB; public float[] monsterSpawnChance;
    public float colidingMeteorSpawnChance, otherCraftAsHazardSpawnChance;
    
    public Vector3 centerPoint;
        
    public Data mydata;
    public CameraFollow myCamera;
    public Transform folderForBullets, folderForObjects, folderForREsources, theBlackCircle, chorsar;
    public Transform pointZero, pointEnd;
    public ParticleSystem myStarMap;
    public ParticleSystem.ShapeModule ps;

    float gameTimer=0, playerIsDeadTimer=0, posholdTimer=0;

    public List<Transform> listOfAsteroids, listOfMonsters, listOfOtherShips, listOfMeteors;
    public int[] arrayOfAwaitingMonsters;

    


    public int statisticWorkCraftLost=0, statisticNormalBulletFired=0;
    public int statisticMonsterLoses=0, statisticSatelitesLoses=0;
    public int[] statisticSecondaryFired, statistocOthers;

    float timerEsc=0.5f; bool escPressedB;
    
    



    private void Awake() {
        GameController = this;
    }
    

    void Start()
    {
        mydata.HudOpenClose(false);
        mydata.OpenMenu();   
        mydata.ProcessToCreateTheDatabaseOfStuff(); 
        mydata.zoomButton.gameObject.SetActive(false);    

        //randomize music track?
        bool doneB=false;
        int value=0;        
        for(int i=0; i< mydata.numberOfMusic; i++){
            doneB = false;
            for(int j=0; j<100; j++){
                if(doneB == false){
                    value =(int)Random.Range(-0.4f, mydata.numberOfMusic - 0.6f);
                    if(value < 0) value =0;
                    if(value > mydata.numberOfMusic -1) value = mydata.numberOfMusic -1;
                    if(mydata.musicSort[value] == null){
                        mydata.musicSort[value] = mydata.musicsBase[i];
                        doneB = true;
                    }
                }
            }            
        }
        for(int i=0; i< mydata.numberOfMusic; i++){
        if(mydata.musicSort[i].isPlaying == true)  mydata.musicSort[i].Stop();
        }
        if(mydata.musicSort[mydata.selectedMusic].isPlaying == false)
            mydata.musicSort[mydata.selectedMusic].Play();
        
        TargetXPFuelLevel();
    }

    
    void Update()
    {
        if(Input.GetKey("escape")){
            EscapeToFromMenu();
        }

        if(mydata.motherShip != null){
            if(mydata.motherShip.GetComponent<MotherShip>().fuelStoraged > nextLevelFuel - 1){
                LevelUP();
            }
        }

        if(gameIsOnB){
            gameTimer += Time.deltaTime;
            if(gameTimer > (gameTik * gameInterval) ){
                gameTik++;
                GameTikEvent();
            }
        }

        //respawning the player
        if(playerIsDestroyedB){
            playerIsDeadTimer += Time.deltaTime;
            mydata.valueRespawn = playerIsDeadTimer / playerIsDeadInterval;

            if(cameraMovedB == false){                
                if(playerIsDeadTimer > playerIsDeadInterval / 2){
                    if(mydata.motherShip != null)
                    myCamera.GetComponent<CameraFollow>().MoveCamereaTo(mydata.motherShip);
                    cameraMovedB = false;
                }
            }
            if(playerIsDeadTimer >= playerIsDeadInterval){
                if(gameIsOnB == true)
                    SpawnPlayer();
                playerIsDestroyedB = false;
            }
        }

        //are music on?
        if(mydata.musicSort[mydata.selectedMusic].isPlaying == false){
            mydata.selectedMusic++;
            if(mydata.selectedMusic >= mydata.numberOfMusic){mydata.selectedMusic =0;}
            mydata.musicSort[mydata.selectedMusic].Play();            
        }

        for(int i=0; i< mydata.musicSort.Length; i++){
                if(i != mydata.selectedMusic)
                    if(mydata.musicSort[i].isPlaying == true){
                        Debug.Log("detected aditional song playing " + mydata.musicSort[i].name + 
                        ",  Contra the current playing "+ mydata.musicSort[mydata.selectedMusic].name);
                        mydata.musicSort[i].Stop();
                    }
            }

        if(posholdOperationsB){
            posholdTimer += Time.deltaTime;
            if(posholdTimer  > posholdInterval){
                posholdTimer = 0;
                posholdOperationsB = false;
                if(beginingOfGameB){
                    beginingOfGameB = false;
                    GameEventGameIsStarted();
                }
                if(endingOfGameB){
                    endingOfGameB = false;
                    //print("Wictory!");
                    GameEventGameIsEnding();
                }
            } 
        }

        if(escPressedB){
            timerEsc -= Time.deltaTime;
            if(timerEsc < 0){
                timerEsc = 0.5f;
                escPressedB = false;
            }
        }
    }






    void LevelUP(){
        mydata.levelUpSfx.Play();
        level++;
        mydata.xplastLevel = nextLevelFuel;
        nextLevelFuel += 10 * level;
        spaceRadius += 5;
        theBlackCircle.localScale =(new Vector3(spaceRadius *2, spaceRadius *2, 1));        
        pointZero.eulerAngles = new Vector3(0,0,0);
        pointEnd.localPosition = new Vector3(0, spaceRadius, 0);

        
        asteroidSmallMax += asteroidSmallIncrease;
        asteroidLargeMax += (int)Random.Range(1, asteroidLargeIncrease + 1);
        for(int i=0; i<asteroidLargeMax; i++){
            SpawnLargeAsteroid();
        }

        int numberToSpawn = (int)Random.Range(0, level * 2);
        for(int i=0; i< numberToSpawn; i++)
            SpawnSatelite();  

        monstersValueMax += monsterValueLevelIncrease * (level );
        //if(level == 2){monsterAlovedB[0] = true;}
        if(level == 2){monsterAlovedB[3] = true;}
        if(level == 3){monsterAlovedB[1] = true;}
        if(level == 5){monsterAlovedB[2] = true;}
        if(level == 4){monsterAlovedB[4] = true;}

        colidingMeteorSpawnChance += level;
        SpawnColidingMeteor();
        SpawnOtherCraftHazard();


        bool doneB= false;
        int lastUpgrade=-1, chosenUpgrade=0, upgradesLength = mydata.Upgrades.Length;
        for(int j=0; j<2; j++){
            doneB = false;
            for(int k=0; k<100; k++){
                if(doneB == false){
                    chosenUpgrade = (int)Random.Range(-0.3f, (float)upgradesLength -0.7f);
                    if(chosenUpgrade < 0) chosenUpgrade = 0;
                    if(chosenUpgrade > upgradesLength -1) chosenUpgrade = upgradesLength -1;

                    if(chosenUpgrade != lastUpgrade){
                        if(mydata.upgradeDoneArray[chosenUpgrade] < mydata.Upgrades[chosenUpgrade].maxLevels){
                            lastUpgrade = chosenUpgrade;
                            mydata.upgradeDoneArray[chosenUpgrade]++;
                            mydata.motherShip.GetComponent<MotherShip>().DoUpgrade(mydata.Upgrades[chosenUpgrade].id, mydata.upgradeDoneArray[chosenUpgrade]);
                            doneB = true;
                        }
                    }
                }
            }
        }
        mydata.DisplayStringOfUpgrades();
    }




    public void EscapeToFromMenu(){
        
        if(escPressedB == false && posholdOperationsB == false){
            escPressedB = true;
            if(mydata.abortGAmeQuestionB == false && mydata.exitGameQuestionB == false 
            && mydata.databaseIsOpenB == false && mydata.zoomPageOnB == false)
            if(mydata.menuIsOnB == false){
                print("Opening menu");
                    mydata.menuIsOnB = true;
                    mydata.menuPanel.gameObject.SetActive(true);
                    mydata.HudPanel.gameObject.SetActive(false);
                    Time.timeScale = 0; 
                    if(gameIsOnB){
                        mydata.buttonLaunchNewGame.gameObject.SetActive(false);
                        mydata.buttonContinueGame.gameObject.SetActive(true);
                        mydata.buttonAbortGame.gameObject.SetActive(true);
                        mydata.collorsButtonsPanel.gameObject.SetActive(false);
                    }else{
                        //probably dont need this one
                        mydata.buttonLaunchNewGame.gameObject.SetActive(true);
                        mydata.buttonContinueGame.gameObject.SetActive(false);
                        mydata.buttonAbortGame.gameObject.SetActive(false);
                        mydata.collorsButtonsPanel.gameObject.SetActive(true);
                    }               
                }else{
                    ButtonContinueGame();
                }
            
            if(mydata.abortGAmeQuestionB == true || mydata.exitGameQuestionB == true){
                mydata.QuestionPanenl.gameObject.SetActive(false);
                mydata.abortGAmeQuestionB = false;
                mydata.exitGameQuestionB = false;
            }

            if(mydata.databaseIsOpenB){
                mydata.ButtonOpenDatabase();
            }

            if(mydata.zoomPageOnB){ButtonZoom();}    
        }
        print("ecp menu, go" + gameIsOnB +" mio"+ mydata.menuIsOnB +" ts" + Time.timeScale);
    }

    public void ButtonContinueGame(){
        print("closing menu");
        if(gameIsOnB){
            mydata.menuIsOnB = false;
            
            mydata.menuPanel.gameObject.SetActive(false);
            mydata.HudPanel.gameObject.SetActive(true);  
            }
                    
                    Time.timeScale = 1; 
    }

    


    public void SpawnPlayer(){
        
        collectCurrent =0;
        GameObject Go = (GameObject)Instantiate(mydata.playerPrefab, centerPoint, Quaternion.identity);
        mydata.player = Go.transform;
        myCamera.GetComponent<CameraFollow>().myTarget = Go.transform;
        Go.GetComponent<PlayerControll>().motherShip = mydata.motherShip;
        Go.GetComponent<PlayerControll>().myControll = this; 
        //Go.GetComponent<DamageHandler>().warSide = collorOfPlayer;
        Go.GetComponent<DamageHandler>().SetSide(collorOfPlayer);
        mydata.motherShip.GetComponent<MotherShip>().theWorkCraft = Go.transform;
        
    }

    public void ButtonStartNewGame(){
        mydata.CloseOpenMenu();
        
        GameObject go = (GameObject)Instantiate(mydata.jumpDriveEffect, pointZero.position, Quaternion.identity);
        //go.GetComponent<ParticleSystem>().startColor = mydata.theCollors[collorOfPlayer];
        //go.GetComponent<ParticleSystem>().main
        //go.GetComponent<ParticleSystem>().MainModule
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = mydata.theCollors[collorOfPlayer];

        mydata.menuIsOnB = false;
        
        victoryB = false;
        defeatB = false;
        gameTimer = 0;
        gameTik = 1;
        collectCurrent = 0;
        collectCapacity = 5;
        level = 1;
        nextLevelFuel = 10;
        playerIsDeadInterval = 10;
        monstersValueMax = 1;
        colidingMeteorSpawnChance = 5;
        otherCraftAsHazardSpawnChance = 10;


        statisticWorkCraftLost = 0;
        statisticNormalBulletFired = 0;
        for(int i=0; i< statisticSecondaryFired.Length; i++){
            statisticSecondaryFired[i] = 0;
        }
        statisticMonsterLoses = 0;
        statisticSatelitesLoses = 0;
        

        //chosing collor of monsters side A
        bool doneB = false;
        int chosenONe=0;
        for(int i=0; i< 150; i++){
            if(doneB == false){
                chosenONe = (int)Random.Range(0.7f, 9.3f);
                if(chosenONe < 1) chosenONe = 1;
                if(chosenONe > 9) chosenONe = 9;

                if(chosenONe != collorOfPlayer){
                    collorOfMonsterSideA = chosenONe;
                    doneB = true;
                }                
            }                
        }

        //chosing collor of satelite sentries
        doneB = false;
        chosenONe=0;
        for(int i=0; i< 150; i++){
            if(doneB == false){
                chosenONe = (int)Random.Range(0.7f, 9.3f);
                if(chosenONe < 1) chosenONe = 1;
                if(chosenONe > 9) chosenONe = 9;

                if(chosenONe != collorOfPlayer
                && chosenONe != collorOfMonsterSideA){
                    colorOfSateliteSentries = chosenONe;
                    doneB = true;
                }                
            }                
        }

        //chosing collor of random hazard
        doneB = false;
        chosenONe=0;
        for(int i=0; i< 150; i++){
            if(doneB == false){
                chosenONe = (int)Random.Range(0.7f, 9.3f);
                if(chosenONe < 1) chosenONe = 1;
                if(chosenONe > 9) chosenONe = 9;

                if(chosenONe != collorOfPlayer
                && chosenONe != collorOfMonsterSideA
                && chosenONe != colorOfSateliteSentries){
                    colorOfRandomGuy = chosenONe;
                    doneB = true;
                }                
            }                
        }
        

        Time.timeScale = 1;
        posholdOperationsB = true;
        beginingOfGameB = true;
        
    }

    void GameEventGameIsStarted(){
        GameObject Go = (GameObject)Instantiate(mydata.basePrefab, centerPoint, Quaternion.identity);
        mydata.motherShip = Go.transform;
        Go.GetComponent<MotherShip>().fuelTargeted = targetFuel;
        //Go.GetComponent<DamageHandler>().warSide = collorOfPlayer;
        Go.GetComponent<DamageHandler>().SetSide(collorOfPlayer);
        Go.GetComponent<MotherShip>().ChangeThoseSprites(collorOfPlayer);
        //Go.transform.SetParent(folderForBullets);

        myCamera.GetComponent<CameraFollow>().Reset();

        SpawnPlayer();

        gameIsOnB = true;

        mydata.HudOpenClose(true);
        mydata.motherShip.GetComponent<MotherShip>().SetBaseStatistic();
        SpawnLargeAsteroid();

        int chosenUpgrade = (int)Random.Range(-0.2f, (float)mydata.secondaryWeaponsOnThoseIds.Length-0.7f);
        if(chosenUpgrade < 0) chosenUpgrade = 0;
        if(chosenUpgrade > mydata.secondaryWeaponsOnThoseIds.Length -1) chosenUpgrade = mydata.secondaryWeaponsOnThoseIds.Length -1;
        chosenUpgrade = mydata.secondaryWeaponsOnThoseIds[chosenUpgrade];
        print("Chosen start upgrade is: " + chosenUpgrade + " " + mydata.Upgrades[chosenUpgrade].name);
        
        mydata.upgradeDoneArray[chosenUpgrade]++;
        mydata.motherShip.GetComponent<MotherShip>().DoUpgrade(mydata.Upgrades[chosenUpgrade].id, mydata.upgradeDoneArray[chosenUpgrade]);
        mydata.upgradeDoneArray[10]++;
        mydata.motherShip.GetComponent<MotherShip>().DoUpgrade(mydata.Upgrades[10].id, mydata.upgradeDoneArray[10]);
        
        mydata.DisplayStringOfUpgrades();
        mydata.zoomButton.gameObject.SetActive(true);
    }

    void GameEventGameIsEnding(){
        gameIsOnB = false;
        
        mydata.HudOpenClose(false);
        mydata.endPanel.gameObject.SetActive(true);
        if(victoryB){
            mydata.endText1.text = mydata.endMesages[0];
            mydata.endImageOnEndText.sprite = mydata.endSpriteToEndPanel[0];
            mydata.victorySfx.Play();
        }
        if(defeatB){
            mydata.endText1.text = mydata.endMesages[1];
            mydata.endImageOnEndText.sprite = mydata.endSpriteToEndPanel[1];
            mydata.defeatSfx.Play();
        }
        mydata.endTextPanel2DisplayedB = false;
        mydata.endTextPanel1.gameObject.SetActive(true);
        mydata.endText2.text ="";
        mydata.endText2.text += "Tyme: " +( (int)gameTimer).ToString()+ " ("+ ( (int)(gameTimer/60) ).ToString()+ "m) \n";
        mydata.endText2.text +=" WorkCraftLost: " +statisticWorkCraftLost +",";
        mydata.endText2.text +=" Monster Loses: " +statisticMonsterLoses +",";
        mydata.endText2.text +=" Satelites: " +statisticSatelitesLoses +", ";

        if(statistocOthers[0] > 0){
            mydata.endText2.text +="\n Others Travelers: (arived) " +statistocOthers[0] +
            ", (depart) "+statistocOthers[1] +", (fall) "+statistocOthers[2];  
        }

        mydata.endText2.text +="\n Bullets Fired: " +statisticNormalBulletFired +", ";
        if(statisticSecondaryFired[0] > 0)
            mydata.endText2.text +="Shotgun Fired: " +statisticSecondaryFired[0] +", ";
        if(statisticSecondaryFired[1] > 0)
            mydata.endText2.text +="Canon Fired: " +statisticSecondaryFired[1] +", ";
        if(statisticSecondaryFired[2] > 0)
            mydata.endText2.text +="Torpedos Launched: " +statisticSecondaryFired[2] +", ";
        if(statisticSecondaryFired[3] > 0)
            mydata.endText2.text +="Sentry Satelites Deployed: " +statisticSecondaryFired[3] +", ";
            
           
        mydata.endText2.text +="\n Upgrades: " +mydata.stringOfUpgrades +" ";
        mydata.endTextPanel2.gameObject.SetActive(false);
        GameCleanTheScene();
    }

    public void EventBaseJumpOut(){
        GameObject go = (GameObject)Instantiate(mydata.jumpDriveEffect, mydata.motherShip.position, Quaternion.identity);
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = mydata.theCollors[collorOfPlayer];
        endingOfGameB = true;
        posholdOperationsB = true;
        gameIsOnB = false;
    }

    public void ButtonAbortGane(){
        mydata.abortGAmeQuestionB = true;
        mydata.QuestionPanenl.gameObject.SetActive(true);
    }
    public void ButtonExitAplication(){
        mydata.exitGameQuestionB = true;
        mydata.QuestionPanenl.gameObject.SetActive(true);
    }

    public void ButtonCollor(int theCollor){
        collorOfPlayer = theCollor;
        
        Color myColor = Color.white;
        myColor = mydata.theCollors[collorOfPlayer];
        print("Changed collor to: "+ theCollor +" and that is: "+ myColor);
        //print(Color.red);
        /*
        if(theCollor == 1) myColor = Color.cyan;
        if(theCollor == 2) myColor = Color.green;
        if(theCollor == 3) myColor = Color.red;
        if(theCollor == 4) myColor = Color.yellow;
        */

        mydata.buttonLaunchNewGame.GetComponent<Image>().color = myColor;
        mydata.craftDisplayOnMenu.color = myColor;
        chorsar.GetComponent<Chorsar>().ChangeCollor(collorOfPlayer);
        
        for(int i=1; i<5; i++){
            if(mydata.buttonColor[i] != null){
                //mydata.buttonColor[i].GetComponent<Button>().pressed = false;
            }
        }
        //if(mydata.buttonColor[theCollor] != null){mydata.buttonColor[theCollor].GetComponent<Button>(). = false;}
    }

    public void ButtonNextOnLastPage(){
        if(mydata.endTextPanel2DisplayedB == false){
            mydata.endTextPanel1.gameObject.SetActive(false);
            mydata.endTextPanel2.gameObject.SetActive(true);
            mydata.endTextPanel2DisplayedB = true;
        }else{
            mydata.endPanel.gameObject.SetActive(false);
            mydata.OpenMenu();
            }
    }

    public void ButtonShowHideUpgradeText(){
        mydata.SwitchTheTextDisplay();
    }

    public void ButtonYesOnQuestion(){
        if(mydata.abortGAmeQuestionB){
            GameCleanTheScene();
            mydata.abortGAmeQuestionB = false;
            mydata.QuestionPanenl.gameObject.SetActive(false);
            mydata.buttonLaunchNewGame.gameObject.SetActive(true);
                        mydata.buttonContinueGame.gameObject.SetActive(false);
                        mydata.buttonAbortGame.gameObject.SetActive(false);
                        mydata.collorsButtonsPanel.gameObject.SetActive(true);
        }
        if(mydata.exitGameQuestionB){ EndTheAplication();}
    }

    public void ButtonAboutThisGame(){
        mydata.startInfoTextPanel.gameObject.SetActive(false);
        mydata.aboutThisGAmeTExtPanel.gameObject.SetActive(true);
        mydata.shovedAboutGameStringIs++;
        if(mydata.shovedAboutGameStringIs >= mydata.aboutGameStrings.Length){
            mydata.shovedAboutGameStringIs = 0;
        }
        mydata.aboutThisGameText.text = mydata.aboutGameStrings[mydata.shovedAboutGameStringIs];
    }


    public void ButtonZoom(){
        if(mydata.zoomPageOnB == false){
            gameIsOnB = false;
            mydata.zoomPageOnB = true;
            Time.timeScale = 0; 
            mydata.SliderPanelZoom.gameObject.SetActive(true);
            mydata.cameraZoom = myCamera.GetComponent<Camera>().orthographicSize;
            mydata.slinerOnZoom.value = mydata.cameraZoom;
        }else{
            gameIsOnB = true;
            mydata.zoomPageOnB = false;
            Time.timeScale = 1; 
            mydata.SliderPanelZoom.gameObject.SetActive(false);
        }
    }


    public void SliderZoom(){
        mydata.cameraZoom = mydata.slinerOnZoom.value;
        myCamera.GetComponent<Camera>().orthographicSize = mydata.cameraZoom;
        //print("Camera size is: "+ myCamera.GetComponent<Camera>().orthographicSize);
    }


    public void TargetXPFuelLevel(){
        int value = (int)mydata.xpValueSlider.value;
        //print("target xp level is: " +value);
        targetFuel = mydata.xpOfTargetLevels[value];
        mydata.xpValueText.text = "TargetFuel "+ targetFuel.ToString();
    }



    


    void GameCleanTheScene(){
        foreach (Transform prr in folderForObjects)
        {
            if(prr.GetComponent<DestroyMe>() != null){
                prr.GetComponent<DestroyMe>().InstantDestroy();
            }
        }
        foreach (Transform prr in folderForBullets)
        {
            if(prr.GetComponent<DestroyMe>() != null){
                prr.GetComponent<DestroyMe>().InstantDestroy();
            }
        }
        foreach (Transform prr in folderForREsources)
        {
            if(prr.GetComponent<DestroyMe>() != null){
                prr.GetComponent<DestroyMe>().InstantDestroy();
            }
        }
        
        if(mydata.player != null){mydata.player.GetComponent<DestroyMe>().InstantDestroy();}
        if(mydata.motherShip != null){mydata.motherShip.GetComponent<DestroyMe>().InstantDestroy();}
        spaceRadius = 15;
        theBlackCircle.localScale =(new Vector3(spaceRadius *2, spaceRadius *2, 1));
        
        pointZero.eulerAngles = new Vector3(0,0,0);
        pointEnd.localPosition = new Vector3(0, spaceRadius, 0);

        asteroidsSmallOnSpace = 0;
        asteroidSmallMax = 10;
        asteroidLargeMax = 1;
        for(int i=0 ; i< monsterAlovedB.Length; i++){
            monsterAlovedB[i] = false;            
        } 
        monsterAlovedB[0] = true;       
        monstersValuePresent = 0;
        monstersValueMax = 0;   

        //myStarMap.shape.radius = (float)spaceRadius;     
        //ps = myStarMap.shape;
        //ps.radius = spaceRadius;

        for(int i=0; i<5; i++){ arrayOfAwaitingMonsters[i] = -1;}
        for(int i=0; i<mydata.upgradeDoneArray.Length; i++){
            mydata.upgradeDoneArray[i] = 0;
        }
        for(int i=0; i<3; i++){statistocOthers[i] = 0;}

        listOfAsteroids.Clear();
        listOfMeteors.Clear();
        listOfMonsters.Clear();
        listOfOtherShips.Clear();

        mydata.zoomButton.gameObject.SetActive(false);
    }

    void EndTheAplication(){        
        print("Order to Quit!");
        Application.Quit();
    }

    void AbortTheGame(){
        gameIsOnB = false;
        GameCleanTheScene();
        mydata.buttonLaunchNewGame.gameObject.SetActive(true);
        mydata.buttonContinueGame.gameObject.SetActive(false);
        mydata.buttonAbortGame.gameObject.SetActive(false);
        mydata.collorsButtonsPanel.gameObject.SetActive(true);
    }


    public void EventBaseDestroyed(){
        gameIsOnB = false;
        defeatB = true;
        print("defeat");
        GameEventGameIsEnding();
        //GameCleanTheScene();
        
        //mydata.OpenMenu();
    }

    public void EventPlayerDestroyed(){
        playerIsDestroyedB = true;
        playerIsDeadTimer =0;
        cameraMovedB = false;
        mydata.death.Play();
        mydata.motherShip.GetComponent<MotherShip>().resorceDeficit++;
        statisticWorkCraftLost++;
        mydata.defeatSfx.Play();
    }

    public void EventCreateResource(Vector3 targetPosition, int resourceDensity){
        mydata.death.Play();
        GameObject go;
        Quaternion rot;
        for(int i=0; i<resourceDensity; i++){
            go = (GameObject)Instantiate(mydata.resourcePrefab, targetPosition, Quaternion.identity);
            rot = go.transform.rotation;
            rot.z = Random.Range(0,360);
            go.transform.rotation = rot;
            go.GetComponent<Bullet>().speed = Random.Range(0.05f, 0.15f);
            //go.GetComponent<DamageHandler>().SetSide
            go.transform.SetParent(folderForREsources);            
        }
    }



    void GameTikEvent(){   
        //spawn one thing per tik?     
        bool spawnedB = false, doneB = false;
        asteroidsSmallOnSpace = 0;
        float spawnChance=0; int numberOfCraft=0;
        if(listOfAsteroids.Count > 0){
            for(int i=0 ; i< listOfAsteroids.Count; i++){
                if(listOfAsteroids[i] != null){
                    asteroidsSmallOnSpace++;
                }
            }
        }
        monstersValuePresent = 0;
        if(listOfMonsters.Count > 0) {
            for(int i=0; i< listOfMonsters.Count; i++){
                if(listOfMonsters[i] != null){
                    //for now only light
                    //monstersValuePresent ++;
                    monstersValuePresent += listOfMonsters[i].GetComponent<DamageHandler>().size;
                }
            }
        }


        
        if(asteroidsSmallOnSpace < asteroidSmallMax){
            SpawnSmallAsteroid();
            spawnedB = true;
        }

        if(arrayOfAwaitingMonsters[0] != -1){
            SpawnMonster(arrayOfAwaitingMonsters[0]);
            for(int i=0; i<monsterAlovedB.Length; i++){
                arrayOfAwaitingMonsters[i] = arrayOfAwaitingMonsters[i+1];
            }
            spawnedB = true;
        }

        if(spawnedB == false)
        for(int i=0; i< monsterAlovedB.Length; i++){
            
            if(monsterAlovedB[i] == true)
                if(monstersValuePresent < monstersValueMax)
                    if(Random.Range(0, 100) > monsterSpawnChance[i]){
                        
                        doneB = false;
                        for(int j=0; j<monsterAlovedB.Length; j++){
                            if(doneB == false){
                                if(arrayOfAwaitingMonsters[j] == -1){
                                    arrayOfAwaitingMonsters[j] = i;
                                    doneB = true;
                                }
                            }
                        }
                    }
        }

        spawnChance   = Random.Range(0, 100);
        int numberOfMeteors = 0;
        if(spawnChance < colidingMeteorSpawnChance){
            
            foreach (var meteor in listOfMeteors)
            {
                if(meteor != null)
                numberOfMeteors++;
            }
            if(numberOfMeteors < level+2)SpawnColidingMeteor();
        }
        
        numberOfCraft=0;
        foreach (var craft in listOfOtherShips)
        {
            if(craft != null)
            if(craft.GetComponent<OtherCraft>() != null){                
                numberOfCraft ++;
            }
        }
        if(numberOfCraft < level){
            spawnChance   = Random.Range(0, 100);
            if(spawnChance < colidingMeteorSpawnChance){
            SpawnOtherCraftHazard(); 
            }
        }

    }



    void SpawnMonster(int theOption){
        float z  = Random.Range(0, 360);        
        pointZero.eulerAngles = new Vector3(0,0,z);        

        GameObject go = (GameObject)Instantiate(mydata.monstersPrefabs[theOption], pointEnd.position, pointZero.rotation);
        z  += 180 + Random.Range(-50, 50);
        go.transform.eulerAngles = new Vector3(0,0,z);
        go.GetComponent<DamageHandler>().SetSide(collorOfMonsterSideA);
        go.GetComponent<Enemy>().myControll = this;
        go.transform.SetParent(folderForObjects);
        //monstersValuePresent += 1;
        if(go.GetComponent<DamageHandler>().size > 2){SpawnAPointerOnPlayerToThisOne(go.transform);}

        bool doneB = false;
        if(listOfMonsters.Count > 0){
            for(int i=0 ; i< listOfMonsters.Count; i++){
                if(doneB == false)
                    if(listOfMonsters[i] == null){
                        listOfMonsters.Insert(i, go.transform);
                        doneB = true;
                    }
            }
        }
        if(doneB == false){listOfMonsters.Add(go.transform)    ;}
    }


    void SpawnSmallAsteroid(){
        asteroidsSmallOnSpace++;        
        float z  = Random.Range(0, 360);        
        pointZero.eulerAngles = new Vector3(0,0,z);        

        GameObject go = (GameObject)Instantiate(mydata.asteroidSmallPrefab, pointEnd.position, pointZero.rotation);
        z  += 180 + Random.Range(-50, 50);
        go.transform.eulerAngles = new Vector3(0,0,z);
        
        go.GetComponent<Bullet>().speed = Random.Range(0.2f, 0.6f);
        go.GetComponent<DamageHandler>().SetSide(0);
        go.transform.SetParent(folderForObjects);

        bool doneB = false;
        if(listOfAsteroids.Count > 0){
            for(int i=0 ; i< listOfAsteroids.Count; i++){
                if(doneB == false)
                    if(listOfAsteroids[i] == null){
                        listOfAsteroids.Insert(i, go.transform);
                        doneB = true;
                    }
            }
        }
        if(doneB == false){listOfAsteroids.Add(go.transform)    ;}
    }

    void SpawnLargeAsteroid(){
        float z  = Random.Range(0, 360);
        pointZero.eulerAngles = new Vector3(0,0,z);
        GameObject go = (GameObject)Instantiate(mydata.asteroidLargePrefab, pointEnd.position, pointZero.rotation);
        z  = Random.Range(1, 4);
        go.transform.position = Vector3.MoveTowards(go.transform.position, centerPoint, z);
        go.transform.SetParent(folderForObjects);
        z  = Random.Range(0, 10);
        if(z > 5){
            float k  = (int)Random.Range(1, 10);
            Vector3 scale = go.transform.localScale;
            scale.x += k/10f;
            scale.y += k/10f;  
            go.transform.localScale = scale;
            go.GetComponent<DamageHandler>().hpCurrent += (int)k *2;
            go.GetComponent<DamageHandler>().resourceDensity += (int)k;
            go.GetComponent<DamageHandler>().ramDamage += (int)k;
        }
    }

    public void SpawnSatelite(){
        float z  = Random.Range(0, 360);
        pointZero.eulerAngles = new Vector3(0,0,z);
        GameObject go = (GameObject)Instantiate(mydata.otherHazards[0], pointEnd.position, pointZero.rotation);
        z  = Random.Range(1, 4);
        go.transform.position = Vector3.MoveTowards(go.transform.position, centerPoint, z);
        go.transform.SetParent(folderForObjects); 
        go.GetComponent<DamageHandler>().SetSide(colorOfSateliteSentries);       
        if(go.GetComponent<DamageHandler>().size > 2){SpawnAPointerOnPlayerToThisOne(go.transform);}
    }

    public void SpawnAPointerOnPlayerToThisOne(Transform newTarget){
        GameObject go = (GameObject)Instantiate(mydata.pointer, PointerHolder.myPointerHolder.transform.position, Quaternion.identity);
        go.transform.SetParent(PointerHolder.myPointerHolder.transform);
        go.GetComponent<Pointer>().InsertTheData(newTarget);
    }

    public void SpawnColidingMeteor(){
        float z  = Random.Range(0, 360);
        pointZero.eulerAngles = new Vector3(0,0,z);
        GameObject go = (GameObject)Instantiate(mydata.asteroidLargePrefab, pointEnd.position, pointZero.rotation);
        z  += 180;
        go.transform.eulerAngles = new Vector3(0,0,z);
        go.transform.GetComponent<Bullet>().speed = Random.Range(0.1f, 0.5f);
        z  = Random.Range(0, 1);
        go.transform.position = Vector3.MoveTowards(go.transform.position, centerPoint, z);
        go.transform.SetParent(folderForObjects);
        z  = Random.Range(0, 10);
        if(z > 5){
            float k  = (int)Random.Range(1, 10);
            Vector3 scale = go.transform.localScale;
            scale.x += k/10f;
            scale.y += k/10f;  
            go.transform.localScale = scale;
            go.GetComponent<DamageHandler>().hpCurrent += (int)k *2;
            go.GetComponent<DamageHandler>().resourceDensity += (int)k;
            go.GetComponent<DamageHandler>().ramDamage += (int)k;
        }
        
        bool doneB = false;
        if(listOfMeteors.Count > 0){
            for(int i=0 ; i< listOfMeteors.Count; i++){
                if(doneB == false)
                    if(listOfMeteors[i] == null){
                        listOfMeteors.Insert(i, go.transform);
                        doneB = true;
                    }
            }
        }
        if(doneB == false){listOfMeteors.Add(go.transform);}
        

        SpawnAPointerOnPlayerToThisOne(go.transform);        
    }

    public void SpawnOtherCraftHazard(){
        bool doneB = false;
        int chosenONe=0;
        for(int i=0; i< 150; i++){
            if(doneB == false){
                chosenONe = (int)Random.Range(0.7f, 9.3f);
                if(chosenONe < 1) chosenONe = 1;
                if(chosenONe > 9) chosenONe = 9;

                if(chosenONe != collorOfPlayer
                && chosenONe != collorOfMonsterSideA
                && chosenONe != colorOfSateliteSentries){
                    colorOfRandomGuy = chosenONe;
                    doneB = true;
                }                
            }                
        }
        float z  = Random.Range(0, 360);
        pointZero.eulerAngles = new Vector3(0,0,z);

        
        int chosenHazard = 0;
        if(level < 3){
            chosenHazard = 1;
        }else{
            float randomOption = Random.Range(0,30);
            if(randomOption < 20){
                chosenHazard = 1; //craft sized 2
            }else{
                chosenHazard = 2; //craft sized 3
            }
        }

        GameObject go = (GameObject)Instantiate(mydata.otherHazards[chosenHazard], pointEnd.position, pointZero.rotation);
        z  = Random.Range(4, 4 + level);
        go.transform.position = Vector3.MoveTowards(go.transform.position, centerPoint, z);
        go.GetComponent<OtherCraft>().StartTheCraft(colorOfRandomGuy, 0);
        go.transform.SetParent(folderForObjects);
        doneB = false;
        if(listOfOtherShips.Count > 0){
            for(int i=0 ; i< listOfOtherShips.Count; i++){
                if(doneB == false)
                    if(listOfOtherShips[i] == null){
                        listOfOtherShips.Insert(i, go.transform);
                        doneB = true;
                    }
            }
        }
        if(doneB == false){listOfOtherShips.Add(go.transform);}
        
        //SpawnAPointerOnPlayerToThisOne()
    }


    public void EventAimChorsarGotData(Transform thetarget){
        chorsar.gameObject.SetActive(true);
        chorsar.GetComponent<Chorsar>().target = thetarget;
        if(chorsar.GetComponent<Chorsar>().player == null){
           chorsar.GetComponent<Chorsar>().player = mydata.player; 
        }
        //string theString = thetarget.name;
        //mydata.DetecorTextInput(theString);        
    }
    

}
