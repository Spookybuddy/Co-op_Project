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
    private bool onlyOne;

    public bool hardMode;
    private float time;
    private bool count;

    public TextMeshProUGUI title;
    public GameObject titleCard;
    public TextMeshProUGUI pause;
    public GameObject pauseCard;
    public TextMeshProUGUI victory;
    public GameObject victoryCard;
    public TextMeshProUGUI failure;
    public GameObject failureCard;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI guide;

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
    private bool checking;

    //Beat 11 levels, including Tutorial, and then Finale
    private int levelsRemain = 6;

    private AudioSource tunes;
    public AudioClip victorySong;

    private AudioSource gameBGM;
    private AudioSource pauseBGM;
    private AudioSource mainBGM;

    void Start()
    {
        //Preset menus
        mainMenu = true;
        pauseMenu = false;
        successful = false;
        gameOver = false;
        activeGame = false;
        onlyOne = false;

        guide.gameObject.SetActive(false);

        //Player 1 & 2 scripts
        player1 = GameObject.Find("Player1").GetComponent<PlayerControl>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerControl>();

        //Musics
        tunes = GetComponent<AudioSource>();
        mainBGM = GameObject.Find("MainMusic").GetComponent<AudioSource>();
        pauseBGM = GameObject.Find("MenuMusic").GetComponent<AudioSource>();
        gameBGM = GameObject.Find("GameMusic").GetComponent<AudioSource>();
    }

    void Update()
    {
        //Set all menus to respective on/off states
        title.gameObject.SetActive(mainMenu);
        titleCard.gameObject.SetActive(mainMenu);
        pause.gameObject.SetActive(pauseMenu);
        pauseCard.gameObject.SetActive(pauseMenu);
        victory.gameObject.SetActive(successful);
        victoryCard.gameObject.SetActive(successful);
        failure.gameObject.SetActive(gameOver);
        failureCard.gameObject.SetActive(gameOver);

        //Background in front or behind levels
        if (mainMenu || pauseMenu || gameOver || successful) {
            background.transform.position = new Vector3(background.transform.position.x, 0, -2);
            timer.gameObject.SetActive(false);
        } else {
            background.transform.position = new Vector3(background.transform.position.x, 0, 8);
            timer.gameObject.SetActive(true);
        }

        //Game music manager
        musicSettings();

        //Set difficulty and start game
        if (mainMenu && Input.GetKeyDown(KeyCode.Alpha1)) {
            //Easy = 6:40min
            hardMode = false;
            time = 400;
            difficulty();
        } else if (mainMenu && Input.GetKeyDown(KeyCode.Alpha2)) {
            //Hard = 4:00min
            hardMode = true;
            time = 240;
            difficulty();
        }

        //Pause menu & quit
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu && !gameOver) {
            activeGame = false;
            if (pauseMenu) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            } else {
                pauseMenu = true;
            }
        }

        //Game over or victory quit
        if (Input.GetKeyDown(KeyCode.Escape) && (gameOver || successful)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true && !checking) {
                StartCoroutine(recheck());
                checking = true;
            }

            //Once X levels are complete, end game
            if (levels.Count < levelsRemain) {
                //Prevent multispawn
                if (!onlyOne) {
                    onlyOne = true;
                    cleanSlate();
                    Instantiate(finalRoom, Vector3.zero, transform.rotation);
                    getLevelData();
                }

                //Once final room is beaten, end screen with time remaining
                if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true) {
                    activeGame = false;
                    successful = true;
                    tunes.PlayOneShot(victorySong, 1.0f);
                    victory.text = "Time Remaining: \n" + time.ToString("F1") + " ";
                }
            }

            //Failure conditions 0 hearts/0 time
            if (hearts_P1 <= 0 || hearts_P2 <= 0 || time <= 0) {
                activeGame = false;
                gameOver = true;
            }
        }
    }

    //Select difficulty
    private void difficulty()
    {
        mainMenu = false;
        count = true;

        //Start with tutorial in scene (Tutorial is 0 indexed)
        setNew();
        getLevelData();
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
        //Tutorial text
        if (levels.Count == 16) {
            guide.gameObject.SetActive(true);
        } else {
            guide.gameObject.SetActive(false);
        }
    }

    //Get data for levels
    private void getLevelData()
    {
        //Slight delay to spawn
        StartCoroutine(load());

        //Move players to new spawns
        levelUp = true;
        activeGame = false;

        checking = false;
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

    //Set the volume based on what menus are up
    private void musicSettings()
    {
        if (pauseMenu || successful || gameOver) {
            pauseBGM.volume = 0.5f;
        } else {
            pauseBGM.volume = 0.0f;
        }
        if (mainMenu) {
            mainBGM.volume = 0.5f;
        } else {
            mainBGM.volume = 0.0f;
        }
        if (activeGame) {
            gameBGM.volume = 0.3f;
        } else {
            gameBGM.volume = 0.0f;
        }
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
        if (goal_P1.beaten_P1 == true && goal_P2.beaten_P2 == true && !successful) {
            newLevel();
        } else {
            checking = false;
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