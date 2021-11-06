using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public bool p1;
    public bool jump;
    private bool locked;
    public int lives;

    private float rise;
    private float side;
    //Button1 = Jump, Button2 = Interact, Button3 = Reset (in GameManager)
    private float button1;
    private float button2;

    private GameManager control;
    private Rigidbody stiff;

    void Start()
    {
        //Set lives based on difficulty
        lives = 5;
        stiff = GetComponent<Rigidbody>();
        control = GameObject.Find("Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        inputs();

        rotation();

        //Movement & jump
        stiff.AddForce(Vector3.right * side * Time.deltaTime * 50, ForceMode.Impulse);
        stiff.velocity = new Vector3(side * Mathf.Min(Mathf.Abs(stiff.velocity.x), 4.0f), stiff.velocity.y, 0.0f);

        //Jump with either up or button1
        if ((button1 > 0 || rise > 0) && jump == true && locked == false) {
            stiff.AddForce(Vector3.up * 5, ForceMode.Impulse);
            jump = false;
            locked = true;
            StartCoroutine(pause());
        }

        //Level up respawn
        if (control.levelUp == true) {
            StartCoroutine(levelupSpawn());
        }

        //Fallout
        if (transform.position.y < -10) {
            hurted();
            respawn();
        }
    }

    //Rotate to face direction
    private void rotation()
    {
        //Face camera when idle
        if (side == 0) {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        } else if (side > 0) {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (side < 0) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    //Take different inputs for player 1 and 2
    private void inputs()
    {
        if (p1) {
            rise = Input.GetAxis("RiseOne");
            side = Input.GetAxis("SideOne");
            button1 = Input.GetAxis("ButtonOne1");
            button2 = Input.GetAxis("ButtonOne2");
        } else {
            rise = Input.GetAxis("RiseTwo");
            side = Input.GetAxis("SideTwo");
            button1 = Input.GetAxis("ButtonTwo1");
            button2 = Input.GetAxis("ButtonTwo2");
        }
    }

    //Reset position to spawn points
    private void respawn()
    {
        stiff.velocity = Vector3.zero;
        if (p1) {
            transform.position = control.spawnP1.gameObject.transform.position;
        } else {
            transform.position = control.spawnP2.gameObject.transform.position;
        }
    }

    //Reduce health
    private void hurted()
    {
        lives = lives - 1;
        if (lives < 0) {
            //Game End
        }
    }

    //On ground // Hit obstacle
    private void OnCollisionStay(Collision collision)
    {
        //When touching ground
        if (collision.gameObject.CompareTag("Ground") && jump == false && locked == false) {
            //Should prevent wall climbing
            stiff.velocity = new Vector3(stiff.velocity.x, -5.0f, 0.0f);
            jump = true;
        }

        //Hit obstacle
        if (collision.gameObject.CompareTag("Hazard")) {
            hurted();
            respawn();
        }
    }

    //Check for enter laser
    void OnTriggerEnter(Collider other)
    {
        if (p1) {
            if (other.gameObject.CompareTag("LaserRed")) {
                hurted();
                respawn();
            }
        } else {
            if (other.gameObject.CompareTag("LaserBlue")) {
                hurted();
                respawn();
            }
        }
    }

    //Move to spawns after 1/50 second
    //NOTE >>> This delay allows for scripts to find new spawns
    IEnumerator levelupSpawn()
    {
        yield return new WaitForSeconds(0.02f);
        respawn();
    }

    //Lock jump for 1/2 second
    IEnumerator pause()
    {
        yield return new WaitForSeconds(0.5f);
        locked = false;
    }
}