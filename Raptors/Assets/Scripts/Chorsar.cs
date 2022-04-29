using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chorsar : MonoBehaviour
{
    public Transform target, player;
    

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            if(target != null){
                transform.position = target.position;
            }else{this.gameObject.SetActive(false);}   
        }else{this.gameObject.SetActive(false);}        
    }

    public void ChangeCollor(int option){
        GetComponentInChildren<SpriteRenderer>().material.color = Data.data.theCollors[option];
    }
}
