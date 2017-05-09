using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    protected Player selfPlayer;
    protected Player chara;
    protected SteamVR_TrackedObject trackedObject;
    protected SteamVR_Controller.Device device;
    protected Transform cameraRig;
    protected Rigidbody rb;
    public NetworkedPlayer networkPlayer;

    protected bool allied = false; // Used to delay the heal action. 

    // Use this for initialization
    public void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        selfPlayer = gameObject.transform.root.GetComponentInChildren<Player>();
        cameraRig = gameObject.GetComponentInParent<Transform>();
        rb = GetComponentInParent<Rigidbody>();
        networkPlayer = transform.root.GetComponentInChildren<NetworkedPlayer>();

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

        if (selfPlayer.CheckCooldown() == true)
        {
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))  //The player has to press before collision. 
            {
                if (allied == true)
                {
                    selfPlayer.Healing(chara);
                    selfPlayer.healPlayer = true;
                    Debug.Log("Healing was applied...");                                                        // Print
                }
            }
        }
        // Search by use of the ApplicationMenu

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            selfPlayer.Search();
        }
    }

    // Moving the player by use of a button. Gaze/orientation directed. 
    protected void Move(float x, float z)
    {
        Vector3 forward = new Vector3(gameObject.transform.TransformDirection(Vector3.forward).x, 0, gameObject.transform.TransformDirection(Vector3.forward).z);
        Vector3 right = new Vector3(gameObject.transform.TransformDirection(Vector3.right).x, 0, gameObject.transform.TransformDirection(Vector3.right).z);
        rb.AddRelativeForce(forward * z * 50);
        rb.AddRelativeForce(right * x * 50);
        
    }

    // Detects collision, and performs heal if friend
    void OnCollisionEnter(Collision target)
    {
        Debug.Log("Target touched");
        switch (target.gameObject.tag.ToString())
        {

            case "Player":
                chara = target.gameObject.GetComponent<Player>();
        Debug.Log("Target touched");
        if (target.gameObject.tag.Equals("Player") == true)
                {
                    selfPlayer.Healing(chara);
                    networkPlayer.lastHitPlayer = target.gameObject.GetComponent<Player>();
                    selfPlayer.healPlayer = true; 
                    allied = true;
                }
                break;
            default:
                Debug.Log("The target does not have any of the tags defined in this switch.");
                break;
        }
    }

    void OnCollisionExit(Collision target)
    {
        Debug.Log("Target exited");
        if (allied == true)
            allied = false; 
    }


    IEnumerator WaitFor() //This is needed to getComponent after parenting action. 
    {
        yield return new WaitForSeconds(0.1f);


    }
}
