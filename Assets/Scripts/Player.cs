using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;   //Allows us to use UI.
using Random = UnityEngine.Random;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
	public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
    public Text weaponPower;
    public int countDown;
    public bool countDownOn;
    private Animator animator;					//Used to store a reference to the Player's animator component.
	private int health;							//Used to store player health points total during level.
    private int villt;
    private int actions;
    private int capsuleMod;
    private int playerXP;
    private int horizontal = 0;     //Used to store the horizontal move direction.
    private int vertical = 0;       //Used to store the vertical move direction.
    private RaycastHit2D hit ;
    private bool canMove;
    public static Vector2 position;
    public bool onWorldBoard;
    public bool dungeonTransition;
    public Image glove;
    public Image boot;
    public Image capsule;
    //public GameObject weaponIm;    
    public Sprite weaponEmpty;
    public Image currentWeaponImage;
    public Image yourTurn;
    private Weapon weapon;
    //public Image weaponComp1;
    //public Image weaponComp2, weaponComp3;
    //public Image copyWeaponComp1, copyWeaponComp2, copyWeaponComp3;
    public int weaponCompIndex0;
    public int weaponCompIndex1;
    public int weaponCompIndex2;
    private WeaponBlade weaponBlade;
    public int wp;
   

    private SpriteRenderer spriteRenderer;

    public static bool isFacingRight;
    public static bool isFacingUp;
    //public static bool isFacingDown;

    public AudioClip[] walking;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip getVillt;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;
    public AudioClip equipItemSound;
    public AudioClip equipSwordSound;
    public AudioClip transitionSound;
    //public Camera UI_camera;

    public int attackMod = 0, defenseMod = 0;

    private Dictionary<String, Item> inventory;

    private void Awake()
    {
        walking = Resources.LoadAll<AudioClip>("Audio/Walking") as AudioClip[];

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Start overrides the Start function of MovingObject
    protected override void Start ()
	{
		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();
		
		//Get the current health point total stored in GameManager.instance between levels.
		health = GameManager.instance.healthPoints;
        playerXP = GameManager.instance.XPPoints;

        //Get the current health point total stored in GameManager.instance between levels.
        villt = GameManager.instance.villtPoints;

        actions = GameManager.instance.actionPoints;

        //Set the healthText to reflect the current player health total.
        TextManager.instance.healthText.text = "Health: " + health;
        TextManager.instance.villtText.text = "Villt: " + villt;
        TextManager.instance.weaponPower.text = " ";
        TextManager.instance.AttackModText.text = " ";
        TextManager.instance.DefenseModText.text = " ";
        TextManager.instance.countDownText.text = " ";
        TextManager.instance.capModText.text = " ";

        countDownOn = false;
        countDown = 30;

        //Set  initial position of Player.
        position.x = position.y = 2;

        onWorldBoard = true;
        dungeonTransition = false;

        inventory = new Dictionary<String, Item>();

        currentWeaponImage.sprite = weaponEmpty;

        //UI_camera = GetComponent<Camera>();
        //UI_camera.clearFlags = CameraClearFlags.SolidColor;

        //Call the Start function of the MovingObject base class.
        base.Start ();
        
	}
	
	private void Update ()
	{
        if (!GameManager.instance.playersTurn)
        {
            yourTurn.color = Color.red;
            return;
        }

        yourTurn.color = Color.green;

        CheckStateCountDown();

        TryMovePlayer();
	}

    private void CheckStateCountDown()
    {
        if (countDownOn && countDown < 1)
        {
            ResetCountDown();
        }
    }

    private void TryMovePlayer()
    {
        canMove = false;
        GetInputMovePlayer();

        if (horizontal != 0 || vertical != 0)       //Check if we have a non-zero value for horizontal or vertical
        {
            if (!dungeonTransition)
            {
                CheckMovePlayer();      //Check what is in the path of the player
                ChangeTextAndStats();
                
                CheckIfGameOver();      //Check to see if game has ended.

                CheckPlayerCollison();
                MovePlayer();
            }
        }
    }

    private void GetInputMovePlayer()
    {
        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }
    }


    private void CheckMovePlayer()
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(horizontal, vertical);
        base.boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, base.blockingLayer);
        base.boxCollider.enabled = true;
    }

    private void ChangeTextAndStats()
    {
        if (onWorldBoard)
        {
            villt--;
            GameManager.instance.villtPoints = villt;
        }

        TextManager.instance.healthText.text = " Health: " + health;
        TextManager.instance.villtText.text = "Villt: " + villt;
        actions++;
        if(actions % 10 == 0)
        {
            AddXP(5);
        }
        else if (actions % 25 == 0)
        {
            AddXP(10);
        }
        else if (actions % 50 == 0)
        {
            AddXP(20);
        }
        else if (actions % 100 == 0)
        {
            AddXP(30);
        }
        GameManager.instance.actionPoints = actions;
        GameManager.instance.XPPoints = playerXP;
        TextManager.instance.XPText.text = GameManager.instance.XPPoints + " XP";

        if (countDownOn)
        {
            countDown--;
            TextManager.instance.countDownText.text = " " + countDown;
            TextManager.instance.capModText.text = " " + capsuleMod;
        }
    }

    private void CheckPlayerCollison()
    {
        if (hit.transform != null)
        {
            switch (hit.transform.gameObject.tag)
            {
                //Call AttemptMove passing in the generic parameter Wall
                case "Wall":
                    canMove = AttemptMove<Wall>(horizontal, vertical);
                    break;
                //Call AttemptMove passing in the generic parameter Chest
                case "Chest":
                    canMove = AttemptMove<Chest>(horizontal, vertical);
                    break;
                //Call AttemptMove passing in the generic parameter Enemy
                case "Enemy":
                    canMove = AttemptMove<Enemy>(horizontal, vertical);
                    break;
            }
        }
        else
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            canMove = AttemptMove<Wall>(horizontal, vertical);
        }
    }

    private void MovePlayer()
    {
        if (canMove && onWorldBoard)
        {
            int walk = Random.Range(0, walking.Length);
            SoundManager.instance.RandomizeSfx(walking[walk]);

            position.x += horizontal;
            position.y += vertical;
            GameManager.instance.updateBoard(horizontal, vertical);
        }
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override bool AttemptMove <T> (int xDir, int yDir)
	{	
        if(xDir == 1 && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if(xDir == -1 && isFacingRight)
        {
            isFacingRight = false;
        }

        if(yDir == 1 && !isFacingUp)
        {
            isFacingUp = true;
        }
        else if(yDir == -1 && isFacingUp)
        {
            isFacingUp = false;
        }

		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		bool hit = base.AttemptMove <T> (xDir, yDir);
		
		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		GameManager.instance.playersTurn = false;

		return hit;
	}
	
	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove <T> (T component)
	{
        //Player move is blocked by either Wall or Chest.
        if(typeof(T) == typeof(Wall))
        {
            Wall blockingObj = component as Wall;
            blockingObj.DamageWall(wallDamage);
        }
        else if(typeof(T) == typeof(Chest))
        {
            Chest blockingObj = component as Chest;
            blockingObj.Open();
        }
        else if (typeof(T) == typeof(Enemy))
        {
            Enemy blockingObj = component as Enemy;
            blockingObj.DamageEnemy(wallDamage);
        }

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger ("playerChop");

        if (weapon)
        {
            weapon.useWeapon();
        }
	}
	
	//LoseHealth is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
	public void LoseHealth (int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger ("playerHit");
		
		//Subtract lost health points from the players total.
		health -= loss;
        GameManager.instance.healthPoints = health;

        //Update the health display with the new total.
        TextManager.instance.healthText.text = "-" + loss + " Health: " + health;
		
        //Check to see if game has ended.
        CheckIfGameOver ();
	}
	
	
	//CheckIfGameOver checks if the player is out of health points and if so, ends the game.
	private void CheckIfGameOver ()
	{
		//Check if health point total is less than or equal to zero.
		if (health <= 0 || villt <= 0) 
		{
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver ();
		}
	}

    private void GoDungeonPortal()
    {
        if (onWorldBoard)
        {
            onWorldBoard = false;
            GameManager.instance.enterDungeon();
            transform.position = DungeonManager.startPos;
        }
        else
        {
            onWorldBoard = true;
            GameManager.instance.exitDungeon();
            transform.position = position;
        }
    }

    private void GoStagePortal()
    {
        GameManager.instance.stageTransition();
    }

    private void UpdateHealth(Collider2D item)
    {
        if (health < 100)
        {
            if (item.tag == "Food")
            {
                health += Random.Range(4, 11);
            }
            else
            {
                health += Random.Range(1, 4);
            }
            GameManager.instance.healthPoints = health;
            TextManager.instance.healthText.text = "Health: " + health;
        }
    }

    private void UpdateVillt(Collider2D item)
    {
        if(villt < 200)
        {
            villt += Random.Range(8, 18);
            GameManager.instance.villtPoints = villt;
            TextManager.instance.villtText.text = "Villt: " + villt;
        }
    }

    private void UpdateInventory(Collider2D item)
    {
        Item itemData = item.GetComponent<Item>();
        switch (itemData.type)
        {
            case itemType.glove:
                if (!inventory.ContainsKey("glove"))
                {
                    inventory.Add("glove", itemData);
                }
                else
                {
                    inventory["glove"] = itemData;
                }

                glove.color = itemData.level;
                break;

            case itemType.boot:
                if (!inventory.ContainsKey("boot"))
                {
                    inventory.Add("boot", itemData);
                }
                else
                {
                    inventory["boot"] = itemData;
                }

                boot.color = itemData.level;
                break;

            case itemType.capsule:
                if (!inventory.ContainsKey("capsule"))
                {
                    inventory.Add("capsule", itemData);
                }
                else
                {
                    inventory["capsule"] = itemData;
                }

                capsule.color = itemData.level;
                StartCountdown(30);
                break;
        }

        attackMod = 0;
        defenseMod = 0;

        foreach(KeyValuePair<String, Item> gear in inventory)
        {
            attackMod += gear.Value.attackMod;
            defenseMod += gear.Value.defenseMod;
            capsuleMod = gear.Value.capMod;
            attackMod += capsuleMod;
            defenseMod += capsuleMod;
        }

        if (weapon)
        {
            wallDamage = attackMod + 3;
        }

        TextManager.instance.AttackModText.text = " " + attackMod;
        TextManager.instance.DefenseModText.text = " " + defenseMod;
    }

    private void StartCountdown(int power)
    {
        countDownOn = true;
        countDown = power;
        TextManager.instance.countDownText.text = " " + countDown;
        TextManager.instance.capModText.text = " " + capsuleMod;
    }

    private void ResetCountDown()
    {
        countDownOn = false;
        TextManager.instance.countDownText.text = " ";
        capsule.color = Color.white;
        attackMod -= capsuleMod;
        defenseMod -= capsuleMod;
        TextManager.instance.AttackModText.text = " " + attackMod;
        TextManager.instance.DefenseModText.text = " " + defenseMod;
        TextManager.instance.capModText.text = " ";
    }

    private void AdaptDifficulty()
    {
        if(wallDamage >= 10)
        {
            GameManager.instance.enemiesSmarter = true;
        }

        if (wallDamage >= 15)
        {
            GameManager.instance.enemiesFaster = true;
        }

        if (wallDamage >= 20)
        {
            GameManager.instance.enemySpawnRatio = 10;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "ExitLevel")
        {
            Invoke("GoStagePortal", 0.5f);
            Destroy(other.gameObject);
            AddXP(30);

            SoundManager.instance.PlaySingle2(transitionSound);
        }
        else if (other.tag == "Exit")
        {
            dungeonTransition = true;
            Invoke("GoDungeonPortal", 0.5f);
            Destroy(other.gameObject);
            AddXP(15);

            SoundManager.instance.PlaySingle2(transitionSound);
        }
        else if(other.tag == "Food")
        {
            UpdateHealth(other);
            Destroy(other.gameObject);
            AddXP(5);

            SoundManager.instance.RandomizeSfx2(eatSound1, eatSound2);
        }
        else if (other.tag == "Soda")
        {
            UpdateHealth(other);
            Destroy(other.gameObject);
            AddXP(5);

            SoundManager.instance.RandomizeSfx2(drinkSound1, drinkSound2);

        }
        else if (other.tag == "Item")
        {
            UpdateInventory(other);
            Destroy(other.gameObject);
            AdaptDifficulty();
            AddXP(25);

            SoundManager.instance.PlaySingle2(equipItemSound);
        }
        else if (other.tag == "Villt")
        {
            Destroy(other.gameObject);
            UpdateVillt(other);
            AddXP(5);

            SoundManager.instance.PlaySingle2(getVillt);
        }
        else if (other.tag == "Weapon")
        {
            AddXP(25);

            if (weapon)
            {
                Destroy(transform.GetChild(0).gameObject);
            }

            other.enabled = false;
            other.transform.parent = transform;
            weapon = other.GetComponent<Weapon>();
            weapon.AcquireWeapon();

            SoundManager.instance.PlaySingle2(equipSwordSound);

            weapon.inPlayerInventory = true;
            weapon.enableSpriteRender(false);

            currentWeaponImage.sprite = weapon.getComponentImageBlade();
            UpdateWeaponText();
            AdaptDifficulty();


            if (weapon.inPlayerInventory == true)
            {
                weapon.enableSpriteRender(false);
            }
        }
    }

    private void AddXP(int xp)
    {
        playerXP += xp;
        GameManager.instance.XPPoints = playerXP;
    }

    private void UpdateWeaponText()
    {
        wp = TextManager.instance.weaponPowerPoints;

        TextManager.instance.weaponPower.text = " " + wp;
        wallDamage = attackMod + wp;
    }

}



