using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedInteractable : Photon.MonoBehaviour {

    public GameObject avatarObject;
    public GameObject avatar;
    public GameObject followingObject;
    public bool areGameMaster;

    private Vector3 receivedAvatarPos;
    private Quaternion receivedAvatarRota;
    private Vector3 receivedAvatarScale;


    //public Transform objectGlobal;
    //public Transform objectLocal;

    // Use this for initialization
    void Start () {
        PhotonView photonView = PhotonView.Get(this);


        if (photonView.isMine)
        {
            
        }

        if (areGameMaster)
        {
            Debug.Log("Trying to give players the avatar");
            photonView.RPC("GiveAvatar", PhotonTargets.AllBufferedViaServer, avatarObject.name);

            //Destroying stuff so the GM isn't bothered with it. They have their own version
            Destroy(avatar.GetComponent<MeshRenderer>()); //Need to test this
            Destroy(avatar.GetComponent<Collider>());
            Destroy(avatar.GetComponent<InteractableItem>());
            Destroy(avatar.GetComponent<Rigidbody>());
            Destroy(avatar.GetComponent<MeshFilter>());
        } else if (!areGameMaster)
        {
           //Currently not doing anything here
        }
        if(avatarObject != null)
        {
            avatar = Instantiate(avatarObject, Vector3.zero, Quaternion.identity); //Temporary test position. Might be able to move this up in the statement above
        }


        this.transform.SetParent(avatar.transform);
        this.transform.localPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {

        }
        else
        {
            if(avatar != null)
            {
                avatar.transform.position = Vector3.Lerp(avatar.transform.position, receivedAvatarPos, Time.deltaTime * 10);
                avatar.transform.rotation = Quaternion.Lerp(avatar.transform.rotation, receivedAvatarRota, Time.deltaTime * 10);
                avatar.transform.localScale = Vector3.Lerp(avatar.transform.localScale, receivedAvatarScale, Time.deltaTime * 10);
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(objectGlobal.position);
            //stream.SendNext(objectGlobal.rotation);
            stream.SendNext(followingObject.transform.localPosition); //Need to test if the spawned object is childed to the controller, if it is,
            stream.SendNext(followingObject.transform.localRotation); //then i need to take into account child positions. Otherwise a double stream is fine
            stream.SendNext(followingObject.transform.localScale);
        }
        else
        {
            if(avatar != null)//Possible to solution that it tries to 
            {
                //this.transform.position = (Vector3)stream.ReceiveNext();
                //this.transform.rotation = (Quaternion)stream.ReceiveNext();
                receivedAvatarPos = (Vector3)stream.ReceiveNext();
                receivedAvatarRota = (Quaternion)stream.ReceiveNext();
                receivedAvatarScale = (Vector3)stream.ReceiveNext();
            }
        }
    }

    [PunRPC]
    void GiveAvatar(string avatarName)
    {
        Debug.Log("Trying to set new avatar");
        if(avatarObject == null)
        {
            avatarObject = Resources.Load<GameObject>("Prefabs/" + avatarName); //This works LOL
            Debug.Log("Received new avatar");
        }
    }
}
