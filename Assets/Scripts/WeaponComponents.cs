using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class WeaponComponents : MonoBehaviour {

    public Sprite[] modules;

    private Weapon parent;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    private void Start ()
    {
        parent = GetComponentInParent<Weapon>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = modules[Random.Range(0, modules.Length)];
        
    }
	
	// Update is called once per frame
	private void Update ()
    {
        transform.eulerAngles = parent.transform.eulerAngles;
	}

    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }
    public SpriteRenderer getSpriteRendererBlade(int i)
    {
        spriteRenderer.sprite = modules[i];
        return spriteRenderer;
    }
    public int getWeaponCompIndex()
    {
        int i = Random.Range(0, modules.Length);
        return i;
    }

}
