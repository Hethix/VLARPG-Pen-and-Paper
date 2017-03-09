using UnityEngine;
using System.Collections;

public class RayCastTest : MonoBehaviour {

    private RaycastHit hit;
    private GameObject hitInteractable;


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.DrawRay(transform.position, transform.forward, Color.blue);
            Debug.Log("Hit an object, " + hit.distance);
            if (hit.collider.CompareTag("Interactable"))
            {
                Debug.Log("I FOUND AN OBJECT TO INTERACT WITH WOOP WOOP!");
                hitInteractable = hit.collider.gameObject;
            }
        }
        else
        {
            hitInteractable = null;
            Debug.Log("Didn't hit an object");
        }
    }
}
