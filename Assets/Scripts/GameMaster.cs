using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    NetworkedInteractable[] allInteractables;

	// Use this for initialization
	void Start () {
        allInteractables = FindObjectsOfType<NetworkedInteractable>();
        foreach(var interactableInArray in allInteractables)
        {
            interactableInArray.areGameMaster = true;
            interactableInArray.avatar = null;
            interactableInArray.transform.parent = null;
        }
	}
	
	// Update is called once per frame
	void Update () {

	}
}
