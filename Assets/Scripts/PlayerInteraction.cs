using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    protected Player selfPlayer;
    protected Character chara;
    protected SteamVR_TrackedObject trackedObject;
    protected SteamVR_Controller.Device device;
    protected Transform cameraRig;
    protected Rigidbody rb; 

    protected bool allied = false; // Used to delay the heal action. 
 

    // Use this for initialization
    public void Start () {
        trackedObject = GetComponentInChildren<SteamVR_TrackedObject>();
        selfPlayer = gameObject.GetComponent<Player>();
        cameraRig = gameObject.GetComponentInParent<Transform>();
        rb = GetComponentInParent<Rigidbody>();

    }

    // Update is called once per frame
    public virtual void Update() {

        device = SteamVR_Controller.Input((int)trackedObject.index);
        // Should move the player depending on direction of gaze. Consider making it 
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && (device.GetAxis().x != 0 || device.GetAxis().y != 0)) {
            Move(device.GetAxis().x, device.GetAxis().y);
        }


        // Delays the heal until the user is holding the controller inside the other player and pressing the trigger. 
        // Note: Hasn't been tested yet. 
        if (allied == true) 
        {
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) //The player has to press before collision. 
                if (selfPlayer.CheckCooldown() == true)
                {
                    selfPlayer.Heal(chara);
                    Debug.Log("Healing was applied...");                                                        // Print
                }
        }

        // Search by use of the ApplicationMenu
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            selfPlayer.Search();

    }

    // Moving the player by use of a button. Gaze/orientation directed. 
    protected void Move(float x, float z)
    {
        rb.AddRelativeForce(gameObject.transform.TransformDirection(Vector3.forward)*z * 50);
        rb.AddRelativeForce(gameObject.transform.TransformDirection(Vector3.right) * x * 50);
    }

    // Detects collision, and performs heal if friend
    void OnCollisionEnter(Collision target)
    {

        switch (target.gameObject.tag.ToString())
        {
            case "Ally":
                chara = target.gameObject.GetComponent<Character>();
                allied = true; 
                break;
            default:
                Debug.Log("The target does not have any of the tags defined in this switch.");
                break;
        }  
    }

    void OnCollisionExit(Collision target)
    {
        if (allied == true)
            allied = false; 
    }
}
