using UnityEngine;
using System.Collections;

public class NetworkedPlayer : Photon.MonoBehaviour
{
    public GameObject avatar;

    public Transform playerGlobal;
    public Transform playerLocal;

    public bool isGameMaster;
    private GameObject spawnedCameraRig;

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
                spawnedCameraRig = Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
                playerGlobal = GameObject.Find("Player(Clone)").transform;
                playerLocal = playerGlobal.Find("Camera (head)/Camera (eye)");
                if (playerLocal == null)
                {
                    playerLocal = playerGlobal.Find("[CameraRig]/Camera (eye)");
                }
            }

            //parent camera to this
            this.transform.SetParent(playerLocal);
            this.transform.localPosition = Vector3.zero;

            //Disabling own avatar, so you can only see other's. Properly only relevant for players and not GM
            avatar.SetActive(false);
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
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}