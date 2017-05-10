//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MagusInteraction : PlayerInteraction
//{


//    // Update is called once per frame
//    public override void Update()
//    {
        
//        device = SteamVR_Controller.Input((int)trackedObject.index);
//        // Should move the player depending on direction of gaze. Consider making it 
//        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && (device.GetAxis().x != 0 || device.GetAxis().y != 0))
//        {
//            Move(device.GetAxis().x, device.GetAxis().y);
//        }

//        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))  //The player has to press before collision. 
//        {
//            Debug.Log("Grip pressed");
//            if (selfPlayer.CheckCooldown() == true)
//            {
//                Debug.Log("Entered cooldown");
//                selfPlayer.Healing(chara);
//                networkPlayer.lastHitPlayer = chara;
//            }
//        }
        
//        // Search by use of the ApplicationMenu
//        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
//        {
//            selfPlayer.Search();
//        }

//        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
//            ThrowFireball();

//    }

//    void ThrowFireball()
//    {
//        if (selfPlayer.CheckCooldown() == true)
//        {
//            selfPlayer.SetCooldown(5);
//            Object partiSystem = Instantiate(Resources.Load("Fireball"), transform);
//            //partiSystem. m.parent = null;
//        }
//    }
//}