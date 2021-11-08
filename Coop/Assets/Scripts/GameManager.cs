using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> levels;
    public GameObject finalRoom;
    private int index = 0;

    public bool hardMode;
    private float time;
    private bool count;

    public TextMeshProUGUI title;
    public TextMeshProUGUI pause;
    public TextMeshProUGUI victory;
    public TextMeshProUGUI failure;
    public TextMeshProUGUI timer;

    public GameObject background;

    private PlayerControl player1;
    private PlayerControl player2;

    private int hearts_P1;
    public GameObject heart1;
    private int hearts_P2;
    public GameObject heart2;

    private GoalControl goal_P1;
    private GoalControl goal_P2;

    public GameObject spawnP1;
    public GameObject spawnP2;

    private bool resetDelay = false;
    private bool mainMenu = true;
    private bool pauseMenu = false;
    private bool successful = false;
    private bool gameOver = false;

    public bool activeGame;
    public bool levelUp;

    //Inverted levels beaten. Uses levels.Count to see how many levels remain
    private int levelsRemain = 5;

    void Start()
    {
        //Preset menus
        mainMenu = true;
        pauseMenu = false;
        successful = false;
        gameOver = false;
        activeGame = false;

        //Player 1 & 2 scripts
        player1 = GameObject.Find("Player1").GetComponent<PlayerControl>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerControl>();

        //Start with tutorial in scene (Tutorial is 0 indexed)
        setNew();
        getLevelData();
    }

    void Update()
    {
        //Set all menus to respective on/off states
        title.gameObject.SetActive(mainMenu);
        pause.gameObject.SetActive(pauseMenu);
        victory.gameObject.SetActive(successful);
        failure.gameObject.SetActive(gameOver);

        //Background in front or behind levels
        if (mainMenu || pauseMenu || gameOver || successful) {
            background.transform.position = new Vector3(background.transform.position.x, 0, -2);
            timer.gameObject.SetActive(false);
        } else {
            background.transform.position = new Vector3(background.transform.position.x, 0, 8);
            timer.gameObject.SetActive(true);
        }

        //Set difficulty and start game
        if (mainMenu && Input.GetKeyDown(KeyCode.Alpha1)) {
            hardMode = false;
            mainMenu = false;
            //Easy = 7min
            time = 420;
            count = true;
        } else if (mainMenu && Input.GetKeyDown(KeyCode.Alpha2)) {
            hardMode = true;
            mainMenu = false;
            //Hard = 4min
            time = 240;
            count = true;
        }

        //Pause menu & quit
        if (Input.GetKeyDown(KeyCode.Escape)) {
            activeGame = false;
            if (pauseMenu) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            } else {
                pauseMenu = true;
            }
        }

        //Resume
        if (Input.GetKeyDown(KeyCode.Return) && pauseMenu) {
            activeGame = true;
            pauseMenu = false;
        }

        //Hearts
        if (hearts_P1 != player1.lives) {
            hearts_P1 = health(player1.lives, hearts_P1, -1, heart1, "P1Heart");
        }
        if (hearts_P2 != player2.lives) {
            hearts_P2 = health(player2.lives, hearts_P2, 1, heart2, "P2Heart");
        }

        //Main update functions
        if (activeGame && !mainMenu && !pauseMenu && !successful && !gameOver) {
            //Timer while not paused or main menus or etc
            if (count) {
                count = false;
                StartCoroutine(countdown());
            }

            //Both players press reset (Added numpad)
            if (Input.GetAxis("ButtonOne3") > 0 && (Input.GetAxis("ButtonTwo3") > 0 || (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Keypad9))) && resetDelay == false) {
                resetDelay = true;
                StartCoroutine(delayed());
                cleanSlate();
                setNew();
                getLevelData();
            }

            //Advance to next level
            if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true) {
                StartCoroutine(recheck());
            }

            //Once X levels are complete, end game
            if (levels.Count < levelsRemain) {
                cleanSlate();
                Debug.Log(Instantiate(finalRoom, Vector3.zero, transform.rotation));
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
        Instantiate(levels[index], Vector3.zero, transform.rotation);
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

    //Display and remove hearts
    private int health(int player, int hearts, int side, GameObject love, string tag)
    {
        if (player > hearts) {
            //Spawn hearts
            for (int i = 0; i < player; i++) {
                Instantiate(love, new Vector3((i * side * 0.75f) + side * 3.5f, 5.25f, 1), love.transform.rotation);
            }
            hearts = player;
        } else {
            //Find all hearts with right tag and remove 1 until they match
            if (hearts > player) {
                GameObject[] erase = GameObject.FindGameObjectsWithTag(tag);
                Destroy(erase[0]);
                hearts = hearts - 1;
            }
        }

        //Update hearts
        return hearts;
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

    //Recheck goals to see if they stayed in place for 1sec
    IEnumerator recheck()
    {
        yield return new WaitForSeconds(1);
        if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true) {
            newLevel();
        }
    }

    //Cooldown on resets
    IEnumerator delayed()
    {
        yield return new WaitForSeconds(2);
        resetDelay = false;
    }

    //Timer countdown at 1/10 sec
    IEnumerator countdown()
    {
        yield return new WaitForSeconds(0.1f);
        time = time - 0.1f;
        timer.text = time.ToString("F1") + " ";
        count = true;
    }
}