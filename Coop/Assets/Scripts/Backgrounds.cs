using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgrounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.right * Time.deltaTime);
        if (transform.position.x < -10) {
            transform.position = new Vector3(transform.position.x + 20, 1, 8);
        }
    }
}
