using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public bool inPlayerInventory = false;

    private Player player;
    private WeaponComponents[] weaponsComps;
    private bool weaponUsed = false;

    private WeaponBlade currentBlade;
    private int localSelectedBlade;

    public void AcquireWeapon()
    {
        player = GetComponentInParent<Player>();
        currentBlade = GetComponentInChildren<WeaponBlade>();
        weaponsComps = GetComponentsInChildren<WeaponComponents>();
    }
    
    // Update is called once per frame
    private void Update ()
    {
        if (inPlayerInventory)
        {
            //extra check the weapon is in the same position as the player
            transform.position = player.transform.position;
            if(weaponUsed == true)
            {
                //arc of movement weapon parameters
                float degreeX = 0, degreeY = 0, degreeZ = -90f, degreeZMax = 275f;
                Vector3 returnVecter = Vector3.zero;

                if (Player.isFacingRight)
                {
                    degreeY = 0;
                    //degreeZMax = 185f;
                    returnVecter = Vector3.zero;
                    //returnVecter = new Vector3(0, 0, 185);
                }
                else if (!Player.isFacingRight)
                {
                    degreeY = 180;
                    //degreeZ = 90f;
                    //degreeZMax = 5f;
                    returnVecter = new Vector3(0, 180, 0);
                }
                else
                if (Player.isFacingUp)
                {
                    //degreeX = 90;
                    degreeY = 0;
                    //degreeZ = -90f;
                    degreeZMax = 185f;
                    returnVecter = new Vector3(0, 0, 185);
                    //returnVecter = Vector3.zero;
                }
                //else if (!Player.isFacingUp)
                //{
                //    degreeY = 0;
                //    //degreeZ = -130f;
                //    //degreeZMax = 5f;
                //    returnVecter = Vector3.zero;
                //    //returnVecter = new Vector3(0, 270, 0);
                //}
                //the actual movement animation
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(degreeX, degreeY, degreeZ), Time.deltaTime * 20f);
                if(transform.eulerAngles.z <= degreeZMax)
                {
                    //after swinging the sword, cancel action and visibility
                    transform.eulerAngles = returnVecter;
                    weaponUsed = false;
					enableSpriteRender (false);
                }
            }
        }
	
	}

    public void useWeapon()
    {
        enableSpriteRender(true);
        weaponUsed = true;
    }

    public void enableSpriteRender(bool isEnabled)
    {
        
        //currentBlade.getSpriteRendererBlade(2).enabled = isEnabled;
        currentBlade.getSpriteRendererBlade(localSelectedBlade).enabled = isEnabled;

        foreach (WeaponComponents comp in weaponsComps)
        {
            comp.getSpriteRenderer().enabled = isEnabled;
        }
    }

    public Sprite getComponentImageBlade()
    {
        localSelectedBlade = currentBlade.blade;
        return currentBlade.getSpriteRendererBlade(localSelectedBlade).sprite;
    }




}
