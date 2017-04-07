using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedInteractable : Photon.MonoBehaviour {

    public GameObject avatarObject;
    private GameObject avatar;
    public GameObject followingObject;
    public bool areGameMaster;

    public Transform objectGlobal;
    public Transform objectLocal;

    // Use this for initialization
    void Start () {
        if (areGameMaster)
        {
            //Disable render and collider
        } else if (!areGameMaster)
        {
            //spawn the correct avatar here
            avatar = Instantiate(avatarObject, Vector3.zero, Quaternion.identity); //Temporary test position
        }

        this.transform.SetParent(followingObject.transform);
        this.transform.localPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(objectGlobal.position);
            stream.SendNext(objectGlobal.rotation);
            stream.SendNext(followingObject.transform.localPosition); //Need to test if the spawned object is childed to the controller, if it is,
            stream.SendNext(followingObject.transform.localRotation); //then i need to take into account child positions. Otherwise a double stream is fine
        }
        else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
