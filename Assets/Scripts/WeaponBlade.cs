using UnityEngine;
using System.Collections;

public class WeaponBlade : MonoBehaviour {

    public Sprite[] bladeModules;

    private Weapon parent;
    private SpriteRenderer spriteRenderer;
    private int randomBladeSelect = 1;

    public int blade;
    public int RandomWeaponPower;      //Set random power of the weapon
    private Player player;

    // Use this for initialization
    private void Start()
    {
        parent = GetComponentInParent<Weapon>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        randomBlade();
        spriteRenderer.sprite = bladeModules[blade];
    }

    // Update is called once per frame
    private void Update()
    {
        transform.eulerAngles = parent.transform.eulerAngles;
    }

    public SpriteRenderer getSpriteRendererBlade(int i)
    {
        //randomBladeSelect = i;
        //spriteRenderer.sprite = bladeModules[i];
        return spriteRenderer;
    }
    public int getWeaponCompIndex()
    {
        int i = Random.Range(0, bladeModules.Length);
        return i;
    }

    private void randomBlade()
    {
        blade = Random.Range(0, 3);

        switch (blade)
        {
            case 0:
                {
                    RandomWeaponPower = Random.Range(20, 30);
                    break;
                }

            case 1:
                {
                    RandomWeaponPower = Random.Range(12, 20);
                    break;
                }

            case 2:
                {
                    RandomWeaponPower = Random.Range(5, 11);
                    break;
                }

            case 3:
                {
                    RandomWeaponPower = Random.Range(1, 5);
                    break;
                }
        }

        TextManager.instance.weaponPowerPoints = RandomWeaponPower;
        //UpdateWeaponText();
    }

    private void UpdateWeaponText()
    {
        //player.weaponPower.text = " " + RandomWeaponPower;
        
        player.weaponPower.text = "test";
        //player.wallDamage = player.attackMod + RandomWeaponPower;
    }
}
