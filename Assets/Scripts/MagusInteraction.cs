using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagusInteraction : PlayerInteraction {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            ThrowFireball(); 

    }

    void ThrowFireball()
    {
        if (selfPlayer.CheckCooldown() == true)
        {
            selfPlayer.SetCooldown(5);
            Object partiSystem = Instantiate(Resources.Load("Fireball"), transform);
            Debug.Log("Expelli-FIREBALL!");                                                        // Print
        }
    }
}
