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

    private InteractableItem interactingItem;

    private GameObject prefab;
    private GameObject playerSeenInteractable;

    private GameObject menu;
    public GameObject[] menuItems;
    private bool isMenuActive = false;
    private bool changeMenu = false;

    private GameObject hitInteractable;
    public RaycastHit hit;

    private Transform cameraRig;
    public Camera mainCamera;
    private float moveSpeed;


    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        menu = GameObject.FindGameObjectWithTag("Menu");

        cameraRig = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //cameraRig.transform.localEulerAngles = new Vector3(cameraRig.transform.localEulerAngles.x, mainCamera.transform.localEulerAngles.y, cameraRig.transform.localEulerAngles.z);

        if (controller.GetPress(padButton))
        {
            MoveCameraRig(false);
        } else if (controller.GetPress(gripButton))
        {
            MoveCameraRig(true);
        }


        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        else
        {
            //Cast a ray, and use it to interact with.
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                //Debug.Log(hit.collider.name);
                //Does it hit an interactable item?
                if (hit.collider.CompareTag("Interactable"))
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


            /*
            if (controller.GetPressDown(triggerButton))
            {
                float minDistance = float.MaxValue;
                float distance;
                foreach (InteractableItem item in objectsHoveringOver) //Goes through all the objects and detects which is closest to the controller
                {
                    distance = (item.transform.position - transform.position).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItem = item;
                    }
                }

                if (closestItem != null && closestItem.isMenuItem) //If the item pressed is something on the menu, then instantiate an object corresponding to the one on the menu
                {
                    prefab = (GameObject)Instantiate(closestItem.worldPrefab, transform.position, Quaternion.Euler(0,0,0)); //Spawn it at the controllers pos and with 0 rotation (facing upwards)
                    interactingItem = prefab.GetComponent<InteractableItem>(); //Is only used for letting an object go again in this case
                    closestItem = null;
                    interactingItem.BeginInteraction(this);
                }
                else if (closestItem != null && closestItem.isArrow) //If an arrow is pressed. simply tell the arrow and make it change menu page
                {
                    closestItem.GetComponent<Arrows>().pressed = true;
                    interactingItem = null;
                    closestItem = null;
                }
                else
                {
                    interactingItem = closestItem;
                    closestItem = null;
                }

                if (interactingItem) //Starts interacting with the chosen item
                {
                    if (interactingItem.IsInteracting()) //this statement is used in order to grap an item in the other hand
                    {
                        interactingItem.EndInteraction(this, false);
                    }

                    interactingItem.BeginInteraction(this);
                }
            } */
        }
    }

    private void DetectObjectHitLabel()
    {
        InteractableItem currentHitObject = hitInteractable.GetComponent<InteractableItem>();
        
        //Detect what is hit with the ray, Can be a menu, arrows or an item.
        if(currentHitObject != null)
        {
            if (currentHitObject.isMenuItem)
            {
                prefab = (GameObject)Instantiate(currentHitObject.worldPrefab, hit.point, Quaternion.Euler(0, 0, 0)); //Spawn it at the controllers pos and with 0 rotation (facing upwards)
                /* playerSeenInteractable = PhotonNetwork.Instantiate("NetworkedInteractable", hit.point, Quaternion.identity, 0);
                playerSeenInteractable.GetComponent<NetworkedInteractable>().areGameMaster = true;
                playerSeenInteractable.GetComponent<NetworkedInteractable>().followingObject = interactingItem.gameObject; */
                interactingItem = prefab.GetComponent<InteractableItem>(); //Is only used for letting an object go again in this case
                interactingItem.BeginInteraction(this);
                //Debug.Log(interactingItem);
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


    private void MoveCameraRig(bool moveVertical)
    {

        //Debug.Log(mainCamera.transform.forward);
        moveSpeed = Time.deltaTime * 2.0f;
        if (!moveVertical)
        {
            //cameraRig.transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * 5.0f, 0, moveSpeed * -Input.GetAxis("Vertical") * 5.0f, Space.Self);
            cameraRig.transform.Translate(mainCamera.transform.forward.x * Input.GetAxis("Horizontal"), 0, mainCamera.transform.forward.z * Input.GetAxis("Vertical"));
        } else if (moveVertical)
        {
            if(mainCamera.transform.rotation.x > 0)
            {
                cameraRig.transform.Translate(0, moveSpeed, 0);

            } else if (mainCamera.transform.rotation.x < 0)
            {
                cameraRig.transform.Translate(0, -moveSpeed, 0);

            }

        }
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

    }
}