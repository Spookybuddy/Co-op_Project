using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> levels;

    public TextMeshProUGUI title;
    public TextMeshProUGUI victory;
    public TextMeshProUGUI failure;

    private PlayerControl player1;
    private PlayerControl player2;

    private GoalControl goal_P1;
    private GoalControl goal_P2;

    public GameObject spawnP1;
    public GameObject spawnP2;

    public bool activeGame;
    public bool levelUp;

    private int rooms = 1;

    void Start()
    {
        //Player 1 & 2 scripts
        player1 = GameObject.Find("Player1").GetComponent<PlayerControl>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerControl>();

        //Start with tutorial in scene
        //getLevelData();

        //EDITOR >>> New level --------------------------------------------------------------------
        newLevel();
    }

    void Update()
    {
        if (activeGame) {
            //Hide title
            title.gameObject.SetActive(false);

            //Level beaten
            if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true)
            {
                Debug.Log("Level Up!");
            }

            //EDITOR >>> Enter next level -------------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.Return))
            {
                newLevel();
            }

            //Once X levels are complete, end game
            if (levels.Count < rooms)
            {
                //Debug.Log("Spawn end room!");
            }
        }
    }

    //New random level spawn
    private void newLevel()
    {
        //Clear out old levels
        GameObject[] clear = GameObject.FindGameObjectsWithTag("Level");
        foreach (GameObject stage in clear) {
            Destroy(stage);
        }

        //Spawn new random level
        int index = Random.Range(0, levels.Count);
        Instantiate(levels[index], new Vector3(0, -3, 0), transform.rotation);
        //EDITOR >>> Show level chosen ------------------------------------------------------------
        Debug.Log("Chose level: " + levels[index]);

        getLevelData();
        
        //Remove from possible choices
        levels.RemoveAt(index);
    }

    //Get data for levels
    private void getLevelData()
    {
        //Get goals
        goal_P1 = GameObject.FindWithTag("Player1Goal").GetComponent<GoalControl>();
        goal_P2 = GameObject.FindWithTag("Player2Goal").GetComponent<GoalControl>();

        //Get spawns
        spawnP1 = GameObject.FindWithTag("Player1Spawn");
        spawnP2 = GameObject.FindWithTag("Player2Spawn");

        //Move players to new spawns
        levelUp = true;
        StartCoroutine(load());
    }

    //Pause for 1/100 second
    IEnumerator load()
    {
        yield return new WaitForSeconds(0.01f);
        spawnP1 = GameObject.FindWithTag("Player1Spawn");
        spawnP2 = GameObject.FindWithTag("Player2Spawn");
        levelUp = false;
    }
}
