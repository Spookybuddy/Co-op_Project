using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgrounds : MonoBehaviour
{
    void Update()
    {
        //Move space background constantly to the left
        transform.Translate(-Vector3.right * Time.deltaTime);
        //Cycle background
        if (transform.position.x < -10) {
            transform.position = new Vector3(transform.position.x + 20, 1, 8);
        }
    }
}