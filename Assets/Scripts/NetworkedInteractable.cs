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
        PhotonView photonView = PhotonView.Get(this);


        if (photonView.isMine)
        {
            ///Don't know whose this is if it is spawned by a RPC. if the GM owns it. then the parent thing should be in here
            ///People say it is owned by the scene, and therefore i should properly use the areGameMaster.
        }

        if (areGameMaster)
        {
            photonView.RPC("giveAvatar", PhotonTargets.AllBufferedViaServer, avatarObject);
        } else if (!areGameMaster)
        {
            //spawn the correct avatar here
        }

        avatar = Instantiate(avatarObject, Vector3.zero, Quaternion.identity); //Temporary test position


        this.transform.SetParent(avatar.transform);
        this.transform.localPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(objectGlobal.position);
            //stream.SendNext(objectGlobal.rotation);
            stream.SendNext(followingObject.transform.localPosition); //Need to test if the spawned object is childed to the controller, if it is,
            stream.SendNext(followingObject.transform.localRotation); //then i need to take into account child positions. Otherwise a double stream is fine
        }
        else
        {
            //this.transform.position = (Vector3)stream.ReceiveNext();
            //this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void giveAvatar(GameObject avatarPrefab)
    {
        if(avatarObject == null)
        {
            avatarObject = avatarPrefab;
        }
    }
}
