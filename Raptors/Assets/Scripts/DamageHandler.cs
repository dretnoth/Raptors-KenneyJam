using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int warSide, ramDamage=1, hpCurrent=1, hpMax=1, size=1, resourceDensity=1;

    public bool iamAliveB = true, selfDestructTimeB;
    public bool baseB, playerB, asteroidB, monsterB, sateliteB, bulletB, otherCraftB;
    public float selfDestructTimer=2;

    public Renderer myRenderer;
    public Sprite[] myOtherSprites;

    public GameObject explosion;
    public ParticleSystem exhaust;
    public Transform[] turrets;
    
    
    void OnTriggerEnter2D(Collider2D other){
        if(other.GetComponent<DamageHandler>() != null){
            int otherSide = other.GetComponent<DamageHandler>().warSide;


            if(otherSide != warSide){
                if(other.GetComponent<DamageHandler>().iamAliveB == true && iamAliveB == true){
                    int otherRamDamage = other.GetComponent<DamageHandler>().ramDamage;
                    hpCurrent -= otherRamDamage;
                }
                
            }    
        }

        if(transform.tag == "H"){
            if(other.tag == "R"){
                if(this.GetComponent<PlayerControll>() != null){
                   this.GetComponent<PlayerControll>().scanerTarget = other.transform; 
                }
            }
        }
    }
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if(hpCurrent <= 0){SelfDestruct();}
        if(selfDestructTimeB){
            selfDestructTimer -=Time.deltaTime;
            if(selfDestructTimer < 0){
                SelfDestruct();
            }
        }
    }

    public void SelfDestruct(){
        iamAliveB = false;
        int oversized = 1;
        if(size > 1){oversized = size + (int)Random.Range(1, resourceDensity) ;}
        
        //options
        if(baseB){Controll.GameController.EventBaseDestroyed();}
        if(playerB){
            Controll.GameController.EventPlayerDestroyed();
            Controll.GameController.EventCreateResource(this.transform.position, Controll.GameController.collectCurrent);
            }
        if(asteroidB){Controll.GameController.EventCreateResource(this.transform.position, resourceDensity);}
        if(monsterB){
            Controll.GameController.statisticMonsterLoses += 1;
            Controll.GameController.EventCreateResource(this.transform.position, oversized);
            }
        if(sateliteB){
            if(warSide != Controll.GameController.collorOfPlayer)
                Controll.GameController.statisticSatelitesLoses += 1;
                if(size > 1) Controll.GameController.EventCreateResource(this.transform.position, oversized);
            }
        if(otherCraftB){
            Controll.GameController.EventCreateResource(this.transform.position, oversized);
            Controll.GameController.statistocOthers[2] +=1;
            }

        //if(baseB || playerB || asteroidB){}
        if(explosion != null){
            Instantiate(explosion, transform.position, Quaternion.identity);            
        }
        Destroy(gameObject);
    }

    public void RandomSprite(){
        if(myOtherSprites.Length > 0){
            //int rndNumber = (int)(Random.Range)
        }
    }

    public void SetSide(int value){
        warSide = value;
        //myRenderer.material.color = Controll.GameController.mydata.theCollors[warSide];
        if(warSide > -1)
            myRenderer.material.color = Data.data.theCollors[warSide];

        if(turrets.Length > 0){
            for(int i=0; i< turrets.Length; i++){
                if(turrets[i] != null){
                    turrets[i].GetComponent<Turret>().warSide = warSide;
                }
            }
        }

        if(exhaust != null){
            exhaust.startColor = Data.data.theCollors[warSide];
        }
       
    }
}
