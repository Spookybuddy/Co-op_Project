using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> levels;
    private int index = 0;

    public TextMeshProUGUI title;
    public TextMeshProUGUI victory;
    public TextMeshProUGUI failure;

    private PlayerControl player1;
    private PlayerControl player2;

    private GoalControl goal_P1;
    private GoalControl goal_P2;

    public GameObject spawnP1;
    public GameObject spawnP2;

    private bool resetDelay = false;
    public bool activeGame;
    public bool levelUp;

    //Inverted levels beaten. Uses levels.Count to see how many levels remain
    private int levelsRemain = 1;

    void Start()
    {
        //Player 1 & 2 scripts
        player1 = GameObject.Find("Player1").GetComponent<PlayerControl>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerControl>();

        //Start with tutorial in scene (Tutorial is 0 indexed)
        setNew();
        getLevelData();
    }

    void Update()
    {
        if (activeGame) {
            //Hide title
            title.gameObject.SetActive(false);

            //Level beaten
            if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true) {
                Debug.Log("Level Up!");
            }

            //Both players press reset
            if (Input.GetAxis("ButtonOne3") > 0 && Input.GetAxis("ButtonTwo3") > 0 && resetDelay == false) {
                Debug.Log("Reset Level!");
                resetDelay = true;
                StartCoroutine(delayed());
                cleanSlate();
                setNew();
                getLevelData();
            }

            //EDITOR >>> Enter next level -------------------------------------------------------------
            //Advance to next level
            if (Input.GetKeyDown(KeyCode.Return)) {
                newLevel();
            }

            //Once X levels are complete, end game
            if (levels.Count < levelsRemain) {
                //Debug.Log("Spawn end room!");
            }
        }
    }

    //New random level spawn
    private void newLevel()
    {
        //Remove old level from possible choices (Starting with the tutorial)
        levels.RemoveAt(index);

        cleanSlate();

        //Random new level from remaining
        index = Random.Range(0, levels.Count);
        setNew();

        getLevelData();
    }

    //Destroy levels for resets or levelups
    private void cleanSlate()
    {
        //Clear out old levels
        GameObject[] clear = GameObject.FindGameObjectsWithTag("Level");
        foreach (GameObject stage in clear) {
            Destroy(stage);
        }
    }

    //Spawn level
    private void setNew()
    {
        Instantiate(levels[index], new Vector3(0, 0, 0), transform.rotation);
    }

    //Get data for levels
    private void getLevelData()
    {
        //Slight delay to spawn
        StartCoroutine(load());

        //Move players to new spawns
        levelUp = true;
        activeGame = false;
    }

    //Pause for 1/100 second
    //NOTE >>> This pause helps the level load before finding the spawns and goals
    IEnumerator load()
    {
        yield return new WaitForSeconds(0.01f);

        //Get goals
        goal_P1 = GameObject.FindWithTag("Player1Goal").GetComponent<GoalControl>();
        goal_P2 = GameObject.FindWithTag("Player2Goal").GetComponent<GoalControl>();

        //Get spawns
        spawnP1 = GameObject.FindWithTag("Player1Spawn");
        spawnP2 = GameObject.FindWithTag("Player2Spawn");

        //Move players to new spawns
        levelUp = false;
        activeGame = true;
    }

    //Cooldown on resets
    IEnumerator delayed()
    {
        yield return new WaitForSeconds(2);
        resetDelay = false;
    }
}