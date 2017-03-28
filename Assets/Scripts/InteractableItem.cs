using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private WandController attachedWand;
    private Transform interactionPoint;
    private bool currentlyInteracting;
    private Coroutine stopRoutine;
    private Coroutine newItemRoutine;

    private float velocityFactor = 2000f;
    private Vector3 posDelta;

    private float rotationFactor = 400f;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;

    //Variables for lerping size of instantiated objects
    private Vector3 startScale;
    private Vector3 endScale;
    private float distance;
    private GameObject menuPosition;


    public bool isMenuItem;
    public bool isArrow;
    public bool isNewItem;
    //Bliver sat inde i MenuController
    public GameObject worldPrefab;

    

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        velocityFactor /= _rigidbody.mass;
        rotationFactor /= _rigidbody.mass;
        startScale = transform.localScale;
        endScale = new Vector3(1.0f, 1.0f, 1.0f);
        menuPosition = GameObject.FindGameObjectWithTag("Menu");
        interactionPoint = new GameObject().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedWand && currentlyInteracting)
        {
            //Debug.Log("Trying to move object with hand");
            posDelta = attachedWand.hit.point - interactionPoint.position;
            this._rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            //Apply rotation force
            this._rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
            //Extra adjustment to keep the object oriented in place (standing up)
            this.transform.localRotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);

            //Size adjustment based on distance to the menu
            if (menuPosition.activeInHierarchy) //Do so if the menu is active
            {
                distance = Vector3.Distance(attachedWand.transform.position, menuPosition.transform.position);
                this.transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.Clamp((distance * 2), 0.0f, 1));
            }
        }
    }

    public void BeginInteraction(WandController wand)
    {
        if (isNewItem)
        {
            newItemRoutine = StartCoroutine(InstantiateFix(wand));
        }
        else
        {
            attachedWand = wand;
            interactionPoint.position = attachedWand.hit.point;
            //interactionPoint.position = attachedWand.transform.position;
            interactionPoint.rotation = attachedWand.transform.rotation;

            interactionPoint.SetParent(transform, true);
            currentlyInteracting = true;

            StopCoroutine(stopRoutine);
            this._rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            gameObject.layer = 2;
        }
        
    }

    public void EndInteraction(WandController wand, bool detectDestroy)
    {
        if (wand == attachedWand)
        {
            attachedWand = null;
            currentlyInteracting = false;
            stopRoutine = StartCoroutine(LockPosition(2.0f));
        }
        isNewItem = false;
		this.transform.localScale = endScale;
        gameObject.layer = 0;

        if (detectDestroy)
        {
            Destroy();
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }

    IEnumerator LockPosition(float waitTime)
    {
        this._rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
                                     RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(waitTime);
        this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    IEnumerator InstantiateFix(WandController wand) //This is needed since BeginInteraction() is ran before Start() is
    {
        yield return new WaitForSeconds(0.001f);
        attachedWand = wand;
        interactionPoint.position = attachedWand.hit.point;
        //interactionPoint.position = attachedWand.transform.position;
        interactionPoint.rotation = attachedWand.transform.rotation;

        interactionPoint.SetParent(transform, true);
        currentlyInteracting = true;
        this._rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        gameObject.layer = 2;

    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}