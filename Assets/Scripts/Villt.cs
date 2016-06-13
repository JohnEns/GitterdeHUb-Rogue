using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Villt : MonoBehaviour {

    public static Villt instance = null;              //Static instance of Villt which allows it to be accessed by any other script.
    [HideInInspector]

    public Sprite[] villtEnergyUnits;

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

        villtEnergyUnits = Resources.LoadAll<Sprite>("Sprites/Villt");
    }

    // Use this for initialization
    void Start () {
	
	}
	
	public Sprite selectVillt()
    {
        int randomVillt = Random.Range(0, villtEnergyUnits.Length);
        return villtEnergyUnits[randomVillt];
    }
}
