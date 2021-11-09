using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public bool p1;
    public bool jump;
    private bool locked;
    private bool iFrame;
    public int lives;
    private bool set;
    private bool stepWait;

    private float rise;
    private float side;

    //Button1 = Jump, Button2 = Interact, Button3 = Reset (in GameManager)
    private float button1;
    private float button2;

    private GameManager control;
    private Rigidbody stiff;
    //private Animator amine;

    private AudioSource noises;
    public AudioClip walkin;
    public AudioClip jumped;
    public AudioClip oof;

    void Start()
    {
        set = false;
        iFrame = false;
        stepWait = true;
        stiff = GetComponent<Rigidbody>();
        noises = GetComponent<AudioSource>();
        control = GameObject.Find("Manager").GetComponent<GameManager>();
        //amine = GetComponent<Animator>();
        lives = 5;
    }

    void Update()
    {
        //Set lives based on difficulty
        if (control.hardMode && !set) {
            lives = lives - 2;
            set = true;
        }

        //Level up respawn
        if (control.levelUp == true) {
            StartCoroutine(levelupSpawn());
        }

        if (control.activeGame) {
            inputs();

            rotation();

            //Movement & jump
            stiff.AddForce(Vector3.right * side * Time.deltaTime * 50, ForceMode.Impulse);
            stiff.velocity = new Vector3(side * Mathf.Min(Mathf.Abs(stiff.velocity.x), 4.0f), stiff.velocity.y, 0.0f);

            //EDITOR NOTE >>> Animations not working :( -------------------------------------------------------------------
            /*if (stiff.velocity.x != 0) {
                amine.SetBool("Run", true);
            } else {
                amine.SetBool("Run", false);
            }*/

            //Walking sound
            if (Mathf.Abs(stiff.velocity.x) > 1 && Mathf.Abs(stiff.velocity.y) < 0.1f && stepWait && jump) {
                stepWait = false;
                noises.PlayOneShot(walkin, 0.3f);
                StartCoroutine(footstep());
            }

            //Jump with either up or button1
            if ((button1 > 0 || rise > 0) && jump == true && locked == false) {
                stiff.AddForce(Vector3.up * 5.5f, ForceMode.Impulse);
                noises.PlayOneShot(jumped, 1.0f);
                jump = false;
                locked = true;
                StartCoroutine(pause());
            }

            //Fallout
            if (transform.position.y < -5 && !iFrame) {
                hurted();
            }
        }
    }

    //Rotate to face direction
    private void rotation()
    {
        //Rotate to falce direction inputed
        if (side > 0) {
            transform.rotation = Quaternion.Euler(-90, 90, 0);
        } else if (side < 0) {
            transform.rotation = Quaternion.Euler(-90, 90, 180);
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
            //Player 2 also can use numpad
            if (Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Keypad7)) {
                button1 = Mathf.Min(1, button1 + Time.deltaTime);
            }
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
        noises.PlayOneShot(oof, 1.0f);
        iFrame = true;
        transform.position = new Vector3(0, 20, 0);
        StartCoroutine(respawnDelay());
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
        if (collision.gameObject.CompareTag("Hazard") && !iFrame) {
            hurted();
        }
    }

    //Check for enter laser
    void OnTriggerEnter(Collider other)
    {
        if (p1) {
            if (other.gameObject.CompareTag("LaserRed") && !iFrame) {
                hurted();
            }
        } else {
            if (other.gameObject.CompareTag("LaserBlue") && !iFrame) {
                hurted();
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

    //Delay on move to spawn when killed
    IEnumerator respawnDelay()
    {
        yield return new WaitForSeconds(0.2f);
        iFrame = false;
        respawn();
    }

    //Footstep delay
    IEnumerator footstep()
    {
        yield return new WaitForSeconds(0.5f);
        stepWait = true;
    }
}