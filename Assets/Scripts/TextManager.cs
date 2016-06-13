using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextManager : MonoBehaviour {

    public static TextManager instance = null;              //Static instance of TextManager which allows it to be accessed by any other script.
    [HideInInspector]

    public int weaponPowerPoints;
    public Text XPText;
    public Text healthText;                     //UI Text to display current player health total.
    public Text villtText;                     //UI Text to display current player Villterator total.
    public Text AttackModText;
    public Text DefenseModText;
    public Text weaponPower;
    public Text capModText;
    public Text countDownText;


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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
