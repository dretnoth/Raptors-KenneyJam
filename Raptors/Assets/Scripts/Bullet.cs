using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 pos, velocity;
    public float speed = 4;

    void Update()
    {
        pos = transform.position;
		velocity = new Vector3 (0, speed * Time.deltaTime, 0);
		pos += transform.rotation * velocity;
		transform.position = pos; 
    }
}
