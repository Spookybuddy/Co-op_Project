using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStuff : MonoBehaviour
{
    private bool active;

    public GameObject generator;
    private Rigidbody rigid;

    public bool wire;
    public bool fire;
    public bool cube;

    private bool spawn = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(delay(0.7f));
        if (wire) {

        } else if (fire) {

        } else {

        }
    }

    void Update()
    {
        while (spawn) {
            StartCoroutine(create());
            spawn = false;
        }

        //Reset drip once it falls off screen
        if (transform.position.y < 5) {
            transform.position = generator.transform.position;
            rigid.velocity = Vector3.zero;
        }
    }

    //Delay at start of level
    IEnumerator delay(float wait)
    {
        yield return new WaitForSeconds(wait);
        spawn = true;
    }

    //Rate of spawning
    IEnumerator create()
    {
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(delay(1.5f));

        //Fire appears and disappears
        if (fire) {
            //Invert state
            active = gameObject.activeSelf;
            gameObject.SetActive(!active);
        }
    }
}
