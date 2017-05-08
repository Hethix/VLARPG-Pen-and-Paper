using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : Photon.MonoBehaviour
{
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId padButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;

    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    private SteamVR_TrackedObject trackedObj;

    public InteractableItem interactingItem;

    private GameObject prefab;
    public GameObject playerSeenInteractable;
    private NetworkedInteractable networkedInteractableScriptRef;

    private GameObject menu;
    public GameObject[] menuItems;
    private bool isMenuActive = false;
    private bool changeMenu = false;

    public GameObject hitInteractable;
    public RaycastHit hit;

    private Transform cameraRig;
    public Camera mainCamera;
    private float moveSpeed;
    private Rigidbody rb;

    private int amountOfInteractablesSpawned; 

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        menu = GameObject.FindGameObjectWithTag("Menu");
        rb = GetComponentInParent<Rigidbody>();

        cameraRig = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }
        else
        {
            //Movement detection
            if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && (controller.GetAxis().x != 0 || controller.GetAxis().y != 0))
            {
                MoveCameraRig(controller.GetAxis().x, controller.GetAxis().y);
            }

            //Cast a ray, and use it to interact with.
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                //Debug.Log(hit.collider.name);
                //Does it hit an interactable item?
                if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Enemy"))
                {
                    //Debug.Log("I FOUND AN OBJECT TO INTERACT WITH WOOP WOOP!  " + hit.collider.name);
                    //If it is not the same object as last update, then change what we hit
                    if(hit.collider.gameObject != hitInteractable)
                    {
                        hitInteractable = hit.collider.gameObject;
                        //Debug.Log("Found a new object to interact with");
                    }
                    if (controller.GetPressDown(triggerButton))
                    {
                        DetectObjectHitLabel();
                    }

                }
            } else if(hitInteractable != null)
            {
                hitInteractable = null;
                //Debug.Log("Didn't hit an object");
            }

            //Stop holding the object
            if (controller.GetPressUp(triggerButton) && interactingItem != null)
            {
                interactingItem.EndInteraction(this, false); //Stops interaction with the item held
                interactingItem = null;
            }

            //Delete the object held in hand by pressing the menu button on the controller
            if (interactingItem != null)
            {
                if (controller.GetPressDown(menuButton) && !interactingItem.isMenuItem && !interactingItem.isArrow)
                {
                    //closestItem = null;
                    interactingItem.EndInteraction(this, true);
                    interactingItem = null;
                }
            }

            //If the controller has the menu attached then we can disable/enable the menu with that controller by pressing the Touch Pad
            if (controller.GetPressDown(padButton) && changeMenu == false && controller.index == 2)
            {
                isMenuActive = !isMenuActive;
                changeMenu = true;
                Menu(isMenuActive);
            }
        }
    }

    private void DetectObjectHitLabel()
    {
        InteractableItem currentHitObject = hitInteractable.GetComponent<InteractableItem>();
        
        //Detect what is hit with the ray, Can be a menu, arrows or an item.
        if(currentHitObject != null)
        {
            if (currentHitObject.isMenuItem && currentHitObject.worldPrefab != null)
            {
                prefab = (GameObject)Instantiate(currentHitObject.worldPrefab, hit.point, Quaternion.Euler(0, 0, 0)); //Spawn it at the controllers pos and with 0 rotation (facing upwards)
                interactingItem = prefab.GetComponent<InteractableItem>(); //Is only used for letting an object go again in this case
                interactingItem.BeginInteraction(this);
                SpawnNetworkedObject();
            }
            else if (currentHitObject.isArrow)
            {
                currentHitObject.GetComponent<Arrows>().pressed = true;
                interactingItem = null;
            }
            else
            {
                interactingItem = currentHitObject;
            }

            if (interactingItem) //Starts interacting with the chosen item
            {
                if (interactingItem.IsInteracting()) //this statement is used in order to grap an item in the other hand
                {
                    interactingItem.EndInteraction(this, false);
                }

                interactingItem.BeginInteraction(this);
            }
        }
    }


    private void MoveCameraRig(float x, float z)
    {

        //Debug.Log(mainCamera.transform.forward);
        moveSpeed = Time.deltaTime * 2.0f;
        //if (!moveVertical)
        //{
        //    cameraRig.transform.Translate(mainCamera.transform.forward.x * Input.GetAxis("Horizontal"), 0, mainCamera.transform.forward.z * Input.GetAxis("Vertical"));
        //} else 
            rb.AddRelativeForce(gameObject.transform.TransformDirection(Vector3.forward) * z * 50);
            rb.AddRelativeForce(gameObject.transform.TransformDirection(Vector3.right) * x * 50);

    }


    //Method used to disable the menu object and the arrows.
    private void Menu(bool isActive)
    {
        if (isActive)
        {
            menu.SetActive(true);
            for(int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].SetActive(true);
            }
            changeMenu = false;
        }
        else
        {
            menu.GetComponent<MenuController>().DisableMenu();
            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].SetActive(false);
            }

            menu.SetActive(false);
            changeMenu = false;
        }
    }


    [PunRPC]
    public void SpawnNetworkedObject()
    {
        if (PhotonNetwork.isMasterClient)
        {
            playerSeenInteractable = PhotonNetwork.InstantiateSceneObject("NetworkedInteractable", hit.point, Quaternion.identity, 0, null);
            networkedInteractableScriptRef = playerSeenInteractable.GetComponent<NetworkedInteractable>();
            networkedInteractableScriptRef.areGameMaster = true;
            networkedInteractableScriptRef.followingObject = interactingItem.gameObject;
            networkedInteractableScriptRef.avatarObject = hit.collider.GetComponent<InteractableItem>().worldPrefab;
            networkedInteractableScriptRef.number = ++amountOfInteractablesSpawned;
        }
    }
}