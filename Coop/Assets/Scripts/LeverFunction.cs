using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverFunction : MonoBehaviour
{
    public bool leverOn;
    private bool locked = false;

    //Connected objects that move on lever switch
    public GameObject[] connection;

    private Material matt;
    public Texture green;
    public Texture red;

    void Start()
    {
        matt = GetComponent<Renderer>().material;
    }

    void Update()
    {
        //Change texture on/off
        if (leverOn) {
            matt.mainTexture = green;
        } else {
            matt.mainTexture = red;
        }
    }

    //While inside trigger if the player presses interact preform the lever functions
    void OnTriggerStay(Collider other)
    {
        if (other.name == "Player1") {
            if (Input.GetAxis("ButtonOne2") > 0 && locked == false) {
                function();
            }
        } else if (other.name == "Player2") {
            if ((Input.GetAxis("ButtonTwo2") > 0 || (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Keypad8))) && locked == false) {
                function();
            }
        }
    }

    //The actual lever function
    private void function()
    {
        //Invert all objects attached
        foreach (GameObject platform in connection) {
            bool status = platform.gameObject.activeSelf;
            platform.gameObject.SetActive(!status);
        }
        //Invert lever boolian and lock lever to prevent spam
        leverOn = !leverOn;
        locked = true;
        StartCoroutine(unlock());
    }

    //Unlock lever when player exits trigger
    void OnTriggerExit(Collider other)
    {
        locked = false;
    }

    //Unlock timer after 7/10 sec
    IEnumerator unlock()
    {
        yield return new WaitForSeconds(0.7f);
        locked = false;
    }
}