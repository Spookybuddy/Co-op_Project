using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStuff : MonoBehaviour
{
    private bool active;

    //Generator is where the obstacle spawns (Wire sparks/Cube)
    public GameObject generator;

    private Rigidbody rigid;

    public bool fire;
    private bool spawn = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(delay(0.7f));
        if (!fire) {
            transform.position = generator.transform.position;
        }
    }

    void Update()
    {
        //While spawn is true, spawn
        while (spawn) {
            StartCoroutine(create());
            spawn = false;
        }

        //Reset drip once it falls off screen
        if (transform.position.y < -5) {
            transform.position = generator.transform.position;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rigid.velocity = Vector3.zero;
        }
    }

    //Method to invert active state of fire
    private void inversion()
    {
        if (fire) {
            active = gameObject.activeSelf;
            gameObject.SetActive(!active);
        }
    }

    //Delay at start of level
    IEnumerator delay(float wait)
    {
        yield return new WaitForSeconds(wait);
        inversion();
        spawn = true;
    }

    //Rate of spawning
    IEnumerator create()
    {
        yield return new WaitForSeconds(2.5f);

        //Fire appears and disappears
        inversion();

        //Stays active for 1.5 sec
        StartCoroutine(delay(1.5f));
    }
}