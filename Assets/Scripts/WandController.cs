using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : MonoBehaviour
{
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId padButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    private InteractableItem closestItem;
    private InteractableItem interactingItem;

    private GameObject prefab;

    private GameObject menu;
    public GameObject[] menuItems;
    private bool isMenuActive = false;
    private bool changeMenu = false;

    private GameObject hitInteractable;
    public RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        menu = GameObject.FindGameObjectWithTag("Menu");
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
            //Cast a ray, and use it to choose what to interact with.
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                Debug.Log(hit.collider.name);
                DrawRay();
                if (hit.collider.CompareTag("Interactable"))
                {
                    //Debug.Log("I FOUND AN OBJECT TO INTERACT WITH WOOP WOOP!  " + hit.collider.name);
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
                Debug.Log("Didn't hit an object");
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
                    objectsHoveringOver.Clear();
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
                prefab = (GameObject)Instantiate(currentHitObject.worldPrefab, transform.position, Quaternion.Euler(0, 0, 0)); //Spawn it at the controllers pos and with 0 rotation (facing upwards)
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

    //Need a better representation, which shows the correct length of ray from controller to interactable object.
    private void DrawRay()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
    }


    
    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
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
}