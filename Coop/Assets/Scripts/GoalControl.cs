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
        //Set color based on goal type
        colored = GetComponent<Renderer>().material;
        if (goal_P1) {
            colored.color = Color.red;
        } else {
            colored.color = Color.blue;
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
