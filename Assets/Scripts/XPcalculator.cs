using UnityEngine;
using System.Collections;

public class XPcalculator : MonoBehaviour {

    public static XPcalculator instance = null;
    [HideInInspector]

    public int startXP;
    public int endXP;
    public int levelNR;
    public int XPplayer;
    public float XPfillFloat;

    public int[,,] arrayXP;

    private Player playerDummy;

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
        //playerDummy = GetComponent<Player>();
        FillXParray();
        CheckContentXParray();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LevelCalculator()
    {
        XPplayer = playerDummy.playerXP;

        if (0 < XPplayer && XPplayer <= 20)
        {
            PortXPdata(0, 20, 1);
        }
        else if (20 < XPplayer && XPplayer <= 50)
        {
            PortXPdata(20, 50, 2);
        }
        else if (50 < XPplayer && XPplayer <= 75)
        {
            PortXPdata(50, 100, 3);
        }
        else if (100 < XPplayer && XPplayer <= 180)
        {
            PortXPdata(100, 180, 4);
        }
        else if (180 < XPplayer && XPplayer <= 220)
        {
            PortXPdata(180, 220, 5);
        }
        else if (220 < XPplayer && XPplayer <= 300)
        {
            PortXPdata(220, 300, 6);
        }
        else if (300 < XPplayer && XPplayer <= 400)
        {
            PortXPdata(300, 400, 7);
        }

    }

    private void PortXPdata(int a, int b, int c)
    {
        levelNR = c; startXP = a; endXP = b;
        FloatCalc(XPplayer - startXP, endXP);
        TextManager.instance.LevelTxt.text = "" + levelNR;
    }

    private void FloatCalc(int start, int end)
    {
        int levelSpan = end - start;

        float teller = (int)((double)start);
        float noemer = (int)((double)levelSpan);
        XPfillFloat = teller / noemer;
        TextManager.instance.fillAmountXP = XPfillFloat;
    }

    private void FillXParray()
    {
        int start = 0;
        int end = 50;
        int lvl = 1;

        int a = 0;
        int b = 0;
        int c = 0;

        //arrayXP[0, 0, 0] = lvl;
        //arrayXP[0, 1, 0] = start;
        //arrayXP[0, 0, 1] = end;


        for (int i = 0; i < 101; i++)
        {
            arrayXP[a, b, c] = lvl;
            b++;
            arrayXP[a, b, c] = start;
            c++;
            arrayXP[a, b, c] = end;
            a++;

            start = end;
            end = 2 * end;
            lvl++;
        }
    }

    private void CheckContentXParray()
    {
         Debug.Log("De XP array inhoud weergegeven: ");

       
        for (int i = 0; i < arrayXP.GetLength(2); i++)
        {
            for (int y = 0; y < arrayXP.GetLength(1); y++)
            {
                for (int x = 0; x < arrayXP.GetLength(0); x++)
                {
                    Debug.Log(arrayXP[x, y, i]);
                }
                Debug.Log("...");
            }
            Debug.Log("...");

        }
    }

    public void CheckXP(int LVLint, int XPint)
    {



    }

}
