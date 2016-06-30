using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextManager : MonoBehaviour {

    public static TextManager instance = null;              //Static instance of TextManager which allows it to be accessed by any other script.
    [HideInInspector]

    public int weaponPowerPoints;
    public Text XPText;
    public Image XPringPic;
    public Text LevelTxt;
    public float fillAmountXP;
    public Text healthText;                     //UI Text to display current player health total.
    public Slider healthSlider;
    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    public Text villtText;                     //UI Text to display current player Villterator total.
    public Slider villtSlider;
    public Text AttackModText;
    public Text DefenseModText;
    public Text weaponPower;
    public Text capModText;
    public Text countDownText;

    public bool playerDamaged = false;

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
        
    }
    // Use this for initialization
    void Start () {

        XPText.text = GameManager.instance.XPPoints + " XP";

        //XPringPic = GetComponent<Image>();

    }
	
	// Update is called once per frame
	private void Update()
    {
        
        //XPringPic.fillAmount -= 3.0f / 10.0f * Time.deltaTime;
        //XPringPic.fillAmount = fillAmountXP / (10.0f * Time.deltaTime);
        //fillAmountXP = 0.34f;

        XPringPic.fillAmount = fillAmountXP;
        if (playerDamaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        playerDamaged = false;

    }

    	
}
