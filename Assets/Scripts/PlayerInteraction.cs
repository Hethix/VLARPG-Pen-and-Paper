﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    protected Player selfPlayer;
    public Player chara;
    protected SteamVR_TrackedObject trackedObject;
    protected SteamVR_Controller.Device device;
    protected Transform cameraRig;
    protected Rigidbody rb;
    public NetworkedPlayer networkPlayer;

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

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))  //The player has to press before collision. 
        {
            if (selfPlayer.CheckCooldown() == true)
            {
                if (chara != null)
                {
                    selfPlayer.Healing(chara);
                    networkPlayer.lastHitPlayer = chara;
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
    void OnTriggerEnter(Collider target)
    {
        switch (target.gameObject.tag.ToString())
        {

            case "Player":
                chara = target.gameObject.GetComponent<Player>();
                break;
            default:
                break;
        }
    }

    void OnTriggerExit(Collision target)
    {
        chara = null; 
        Debug.Log("Target exited");
    }


    IEnumerator WaitFor() //This is needed to getComponent after parenting action. 
    {
        yield return new WaitForSeconds(0.1f);


    }
}
