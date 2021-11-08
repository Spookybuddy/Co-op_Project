using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgrounds : MonoBehaviour
{
    private float bound;
    private float store;
    public float based;

    void Update()
    {
        //Move space background constantly to the left
        transform.Translate(-Vector3.right * Time.deltaTime);

        //Cycle background
        if (transform.position.x < -bound) {
            transform.position = new Vector3(transform.position.x + 2*bound, 0, transform.position.z);
        }

        //Reset on shift
        if (transform.position.z != store) {
            if (transform.position.z < 0) {
                transform.position = new Vector3(based*2, 0, transform.position.z);
            } else {
                transform.position = new Vector3(based, 0, transform.position.z);
            }
            store = transform.position.z;
        }

        //Rescale based on position
        if (transform.position.z < 0) {
            transform.localScale = new Vector3(2, 1, 1.25f);
            bound = 20;
        } else {
            transform.localScale = new Vector3(1, 1, 0.666f);
            bound = 10;
        }
    }
}