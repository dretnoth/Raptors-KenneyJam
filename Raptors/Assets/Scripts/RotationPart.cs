using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPart : MonoBehaviour
{
    public float speed=0.15f;

    void Start()
    {
        int value = Random.Range(-49, 50);
        if(value > 0){}else{speed = -speed;}    
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( new Vector3(0, 0, speed) );
    }
}
