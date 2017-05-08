using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagusInteraction : PlayerInteraction
{


    // Update is called once per frame
    public override void Update()
    {
        device = SteamVR_Controller.Input((int)trackedObject.index);

        // Should move the player depending on direction of controller + touchpad direction 
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && (device.GetAxis().x != 0 || device.GetAxis().y != 0))
        {
            Move(device.GetAxis().x, device.GetAxis().y);
        }


        // Delays the heal until the user is holding the controller inside the other player and pressing the trigger. 
        // Note: Hasn't been tested yet. 
        if (allied == true)
        {
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) //The player has to press before collision. 
                if (selfPlayer.CheckCooldown() == true)
                {
                    selfPlayer.Heal(chara);
                    Debug.Log("Healing was applied...");                                                        // Print
                }
        }

        // Search by use of the ApplicationMenu
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            selfPlayer.Search();

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            ThrowFireball();

    }

    void ThrowFireball()
    {
        if (selfPlayer.CheckCooldown() == true)
        {
            selfPlayer.SetCooldown(5);
            Object partiSystem = Instantiate(Resources.Load("Fireball"), transform);
        }
    }
}