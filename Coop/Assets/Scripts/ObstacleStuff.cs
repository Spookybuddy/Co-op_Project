using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStuff : MonoBehaviour
{
    //Generator is where the obstacle spawns
    public GameObject generator;
    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        transform.position = generator.transform.position;
    }

    void Update()
    {
        //Reset once it falls off screen
        if (transform.position.y < -5) {
            transform.position = generator.transform.position;
            transform.rotation = Quaternion.identity;
            rigid.velocity = Vector3.zero;
        }
    }
}