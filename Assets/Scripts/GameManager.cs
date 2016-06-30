using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float textDelay = 3f;

    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int healthPoints = 100;                          //Starting value for Player health points.
    public int villtPoints = 100;
    public int actionPoints;
    public int XPPoints;
    public int stageNumber;
    public static readonly Player currentPlayer;
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector]
    public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
    private GameObject levelImage;      //To display on screen text for the player
    private Text levelText;             //The text that must be displayed
    private GameObject scoreImage;
    private Text scoreText;

    private BoardManager boardscript;
    private stage1boardman stage1BoardScript;
    private BoardManager stage2BoardScript;
    private DungeonManager dungeonScript;
    private Player playerScript;
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;                             //Boolean to check if enemies are moving.

    private bool playerInDungeon;
    public AudioClip[] ambient;
    //public AudioClip ambientSolo;

    public bool enemiesFaster = false;
    public bool enemiesSmarter = false;
    public int enemySpawnRatio = 20;
    public int enemyPower = 3;

    
    //Awake is always called before any Start functions
    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        ambient = Resources.LoadAll<AudioClip>("Audio/AmbientDungeon") as AudioClip[];

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

        stageNumber = 0;
        boardscript = GetComponent<BoardManager>();
        stage1BoardScript = GetComponent<stage1boardman>();
        stage2BoardScript = GetComponent<BoardManager>();

        dungeonScript = GetComponent<DungeonManager>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //This is called each time a scene is loaded.
    private void OnLevelWasLoaded(int index)
    {
        //Call InitGame to initialize our level.
        InitGame();
    }

    //Initializes the game for each level.
    private void InitGame()
    {
        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();
        boardscript.BoardSetup();
        playerInDungeon = false;

        //levelImage = GameObject.Find("LevelImage");
        //levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.text = "Welcome \n \n Search and Kill to \n Survive";
        //levelImage.SetActive(true);
        //Invoke("HideLevelImage", 0f);

        //SoundManager.instance.PlaySingle();

    }

    private void InitGameStage1()
    {
        enemies.Clear();
        stage1BoardScript.S1_BoardSetup();
        playerInDungeon = false;
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
    }

    private void setText(string Text)
    {
        levelText.text = Text;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", textDelay);
    }

    //Update is called every frame.
    private void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //Start moving enemies.
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void RemoveEnemyFromList(Enemy script)
    {
        enemies.Remove(script);
    }

    //GameOver is called when the player reaches 0 health points
    public void GameOver()
    {
        setText("Sadly you succumbed \n");

        //Disable this GameManager.
        enabled = false;
    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true, player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        //Create list enemiesToDestroy to keep track of enemies walking out of view
        List<Enemy> enemiesToDestroy = new List<Enemy>();

        for (int i = 0; i < enemies.Count; i++)
        {
            if (playerInDungeon)
            {
                yield return CheckVisibleDungeonEnemies();
            }
            else
            {
                enemiesToDestroy.AddRange(ClearWorldBoardEnemies());
            }
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;

        //Remove all enemies from enemiesToDestroy out of total enemies List.
        for (int i = 0; i < enemiesToDestroy.Count; i++)
        {
            enemies.Remove(enemiesToDestroy[i]);
            Destroy(enemiesToDestroy[i].gameObject);
        }
        //empty out the list enemiesToDestroy
        enemiesToDestroy.Clear();
    }

    private IEnumerable CheckVisibleDungeonEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].getSpriteRenderer().isVisible)
            {
                if (i == enemies.Count - 1)
                {
                    yield return new WaitForSeconds(enemies[i].moveTime);
                    break;
                }
                break;
            }
            
        }
    }

    private IEnumerable<Enemy> ClearWorldBoardEnemies()
    {
        for (var i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].getSpriteRenderer().isVisible || !boardscript.checkValidTile (enemies[i].transform.position))
            {
                yield return enemies[i];
            }
            
        }
    }


    public void updateBoard(int horizontal, int vertical)
    {
        boardscript.addToBoard(horizontal, vertical);
    }

    private void playAmbient()
    {
        //StartCoroutine(SoundManager.AudioFade.FadeOut(SoundManager.instance.musicSource, 0.1f, 0.5f));

        SoundManager.instance.musicSource.volume = 0.5f;
        
        AudioClip ambience = ambient[Random.Range(0, ambient.Length)];
        SoundManager.instance.ambientSource.clip = ambience;

        StartCoroutine(SoundManager.AudioFade.FadeIn(SoundManager.instance.ambientSource, 0.1f));
    }

    private void stopAmbient()
    {
        //StartCoroutine(SoundManager.AudioFade.TurnUp(SoundManager.instance.musicSource, 0.1f));
        SoundManager.instance.musicSource.volume = 1.0f;
        
        StartCoroutine(SoundManager.AudioFade.FadeOut(SoundManager.instance.ambientSource, 0.1f, 0f));
    }

    public void enterDungeon()
    {
        dungeonScript.StartDungeon();
        boardscript.SetDungeonBoard(dungeonScript.gridPositions, dungeonScript.maxBound, dungeonScript.endPos);

        playerScript.dungeonTransition = false;
        playerInDungeon = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i].gameObject);
        }

        enemies.Clear();

        playAmbient();

        //Voorbeeld static variabele!
        //currentPlayer.countDown = 5;
    }

    public void stageTransition()
    {
        boardscript.clearScene();

        //stageNumber++;
        stageNumber = 1;

        Destroy(boardscript);
        boardscript = GetComponent<BoardManager>();

        InitGame();
    }

    public void selectNextStage()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {

            //SceneManager.LoadScene("kloemp");
           
            //SceneManager.LoadScene("stage0");

            //SceneManager.LoadScene("stage1");

            //InitStage1();
        }
        else if (SceneManager.GetActiveScene().name == "stage1")
        {
            stageNumber = 2;
            SceneManager.LoadScene("stage2");
            //InitStage2();
        }
    }

    public void exitDungeon()
    {
        stopAmbient();

        boardscript.SetWorldBoard();
        playerScript.dungeonTransition = false;
        playerInDungeon = false;
        enemies.Clear();
    }

}