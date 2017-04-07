using UnityEngine;
using System.Collections;

public class NetworkedPlayer : Photon.MonoBehaviour
{
    public GameObject avatar;

    public Transform playerGlobal;
    public Transform playerLocal;

    public bool isGameMaster;
    private GameObject spawnedCameraRig;

    private Vector3 receivedBodyPos;
    private Quaternion receivedBodyRota;
    private Vector3 receivedHeadPos;
    private Quaternion receivedHeadRota;

    void Start()
    {
        Debug.Log("i'm instantiated");
        //Since i check if this is mine, there should allways only be one camera rig in the scene, and that should be your own
        if (photonView.isMine)
        {
            Debug.Log("player is mine");
            if (isGameMaster) //If the player is GameMaster, then spawn him with that prefab
            {
                spawnedCameraRig = (GameObject)Instantiate(Resources.Load("GameMaster"), new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity);
                //spawnedCameraRig = Instantiate(Resources.Load("GameMaster", typeof(GameObject))) as GameObject;
                playerGlobal = GameObject.Find("GameMaster(Clone)").transform;
                playerLocal = playerGlobal.Find("[CameraRig]/Camera (head)/Camera (eye)");
                if(playerLocal == null)
                {
                    playerLocal = playerGlobal.Find("[CameraRig]/Camera (eye)");
                }
            }
            else //Spawn a player with the designated prefab
            {
                spawnedCameraRig = (GameObject)Instantiate(Resources.Load("Player"), new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity);
                //spawnedCameraRig = Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
                playerGlobal = GameObject.Find("Player(Clone)").transform;
                playerLocal = playerGlobal.Find("Camera (head)/Camera (eye)");
                if (playerLocal == null)
                {
                    playerLocal = playerGlobal.Find("Camera (eye)");
                }
            }

            //parent camera to this
            this.transform.SetParent(playerLocal);
            this.transform.localPosition = Vector3.zero;

            //Disabling own avatar, so you can only see other's. Properly only relevant for players and not GM
            avatar.SetActive(false);
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            //The local movement is handled by setting as child up in start
        }else
        {
            //Making smooth movement here
            this.transform.position = Vector3.Lerp(this.transform.position, receivedBodyPos, Time.deltaTime * 10);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, receivedBodyRota, Time.deltaTime * 10);
            avatar.transform.position = Vector3.Lerp(avatar.transform.position, receivedHeadPos, Time.deltaTime * 10);
            avatar.transform.rotation = Quaternion.Lerp(avatar.transform.rotation, receivedHeadRota, Time.deltaTime * 10);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(playerGlobal.position);
            stream.SendNext(playerGlobal.rotation);
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation); //properly need to change it so it only affects the y-axis
        }
        else
        {
            receivedBodyPos = (Vector3)stream.ReceiveNext();
            receivedBodyRota = (Quaternion)stream.ReceiveNext();
            receivedHeadPos = (Vector3)stream.ReceiveNext();
            receivedHeadRota = (Quaternion)stream.ReceiveNext();
        }
    }
}