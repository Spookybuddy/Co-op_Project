using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSet : MonoBehaviour
{
    public bool P1_Spawn;
    private Material colored;

    void Start()
    {
        //Set color and tag to respective spawns
        colored = GetComponent<Renderer>().material;
        if (P1_Spawn) {
            colored.color = Color.blue;
            gameObject.tag = "Player1Spawn";
        } else {
            colored.color = Color.red;
            gameObject.tag = "Player2Spawn";
        }
    }
}