using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    Player selfPlayer;
    Character chara;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;
    private Transform cameraRig;

    bool allied = false; // Used to delay the heal action. 
 

    // Use this for initialization
    void Start () {
        trackedObject = GetComponentInChildren<SteamVR_TrackedObject>();
        selfPlayer = gameObject.GetComponentInParent<Player>();
        cameraRig = gameObject.GetComponentInParent<Transform>();


    }

    // Update is called once per frame
    void Update() {

        device = SteamVR_Controller.Input((int)trackedObject.index);
        // Heal
        if (Input.GetKey(KeyCode.W))
            gameObject.transform.position += Vector3.forward * 10 * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            gameObject.transform.position += Vector3.left * 10 * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            gameObject.transform.position += Vector3.back * 10 * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            gameObject.transform.position += Vector3.right * 10 * Time.deltaTime;
        

        // Should move the player depending on direction of gaze. Consider making it 
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) && (device.GetAxis().x != 0 || device.GetAxis().y != 0)) {
            Debug.Log("Casual racism. X: " + device.GetAxis().x + " Y: " + device.GetAxis().y);
            // Doesn't currently do anything. 
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

    }

    // Moving the player by use of a button. Gaze/orientation directed. 
    void Move()
    {
        // I need to find out how to apply movement to the player in a way that makes sense. 
        //cameraRig.transform.Translate(0.2f * Input.GetAxis("Horizontal"), 0, 0.2f * -Input.GetAxis("Vertical"), Space.Self);
        //device.GetComponent<Rigidbody>().AddForce(transform.forward * thrust);
    }

    // Detects collision, and performs attack/heal depending on friend or enemy
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
