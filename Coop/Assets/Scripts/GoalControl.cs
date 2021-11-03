using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalControl : MonoBehaviour
{
    public bool goal_P1;
    private Material colored;

    public bool beaten_P1 = false;
    public bool beaten_P2 = false;

    void Start()
    {
        //Set color and tag to respective goals
        colored = GetComponent<Renderer>().material;
        if (goal_P1) {
            colored.color = Color.blue;
            gameObject.tag = "Player1Goal";
        } else {
            colored.color = Color.red;
            gameObject.tag = "Player2Goal";
        }
    }

    void Update()
    {

    }

    //When player reaches goal
    void OnTriggerEnter(Collider other)
    {
        if (goal_P1) {
            if (other.name == "Player1") {
                beaten_P1 = true;
            }
        } else {
            if (other.name == "Player2") {
                beaten_P2 = true;
            }
        }
    }
}
