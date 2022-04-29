using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerHolder : MonoBehaviour
{
    public static PointerHolder myPointerHolder;
    public Data mydata;
    
    private void Awake() {
        myPointerHolder = this;
    }
    void Update()
    {
        if(mydata.player != null){
            transform.position = mydata.player.transform.position;
        }else{transform.position = new Vector3(0,0,0);}
    }
}
