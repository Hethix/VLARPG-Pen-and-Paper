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

    private IEnumerator coroutine;
    public bool startNewRoutine;


    //public Transform objectGlobal;
    //public Transform objectLocal;

    // Use this for initialization
    void Start () {
        PhotonView photonView = PhotonView.Get(this);
        coroutine = DetectDestroy(10.0f);
        startNewRoutine = true;

        if (photonView.isMine)
        {
            //The owner is the GM(also known as master client in terms of Photon
        }

        if (areGameMaster)
        {
            photonView.RPC("GiveAvatar", PhotonTargets.AllBufferedViaServer, avatarObject.name);
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
        if (areGameMaster)
        {
            if (startNewRoutine)
            {
                StopCoroutine("DetectDestroy");
                StartCoroutine("DetectDestroy", 4.0f); //Runs the destroy detection every 4 seconds. Faster would properly impact performance
                startNewRoutine = false;
            }
        }
        if (photonView.isMine)
        {
            //There is a chance that isMine isn't allways the GM. and therefore I am not certain to use it.
        }
        else
        {
            if(avatar != null) //Fix for trying to move an object that might not yet be spawned by the clients, before receiving data.
            { //Smothing of item movement
                avatar.transform.position = Vector3.Lerp(avatar.transform.position, receivedAvatarPos, Time.deltaTime * 10);
                avatar.transform.rotation = Quaternion.Lerp(avatar.transform.rotation, receivedAvatarRota, Time.deltaTime * 10);
                avatar.transform.localScale = Vector3.Lerp(avatar.transform.localScale, receivedAvatarScale, Time.deltaTime * 10);
            }
        }
    }


    private IEnumerator DetectDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(followingObject == null)
        {
            photonView.RPC("DestroyAvatar", PhotonTargets.AllBufferedViaServer); //Calls the method for all clients to destroy the avatar since the GM destroyed it
        }
        startNewRoutine = true;
    }


    //Sending and receiving data
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(followingObject.transform.localPosition); 
            stream.SendNext(followingObject.transform.localRotation);
            stream.SendNext(followingObject.transform.localScale);
        }
        else
        {
            receivedAvatarPos = (Vector3)stream.ReceiveNext();
            receivedAvatarRota = (Quaternion)stream.ReceiveNext();
            receivedAvatarScale = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void GiveAvatar(string avatarName)
    {
        if(avatarObject == null)
        {
            avatarObject = Resources.Load<GameObject>("Prefabs/" + avatarName); //This works LOL
        }
    }

    [PunRPC]
    void DestroyAvatar()
    {
        Destroy(avatar);
        Destroy(gameObject);
    }

}
