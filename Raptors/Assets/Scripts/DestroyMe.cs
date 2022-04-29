using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public bool TimerOnB= false;
    public float theTime = 2f;

    public void InstantDestroy(){ Destroy(gameObject);}
    public void DestroyWhitDellay(float delayTime){
        if(delayTime > 0){
            Destroy(gameObject, delayTime);
        }else{
            InstantDestroy();
        }
    }

    private void Start() {
        if(TimerOnB){
            DestroyWhitDellay(theTime);
        }
    }
}
