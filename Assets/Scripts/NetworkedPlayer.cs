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
    private Quaternion rotationQuaternion;
    private Player myPlayerScript;

    public Enemy lastHitEnemy; 
    private int lastHitEnemyNumber;
    public Player lastHitPlayer; 
    PhotonView photonView;

    public Player localPlayersInArray;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
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
                spawnedCameraRig = (GameObject)Instantiate(Resources.Load("Player" + "_" + avatar.name), new Vector3(102.0f, 16.0f, 60.0f), Quaternion.identity);
                //spawnedCameraRig = Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
                playerGlobal = GameObject.Find("Player_" + avatar.name + "(Clone)").transform;
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
            if(!isGameMaster)
                myPlayerScript = GetComponent<Player>();
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            
            if(!isGameMaster)
            {
                if (myPlayerScript.dealDmg)
                {
                    lastHitEnemyNumber = lastHitEnemy.number;
                    photonView.RPC("DealDmgToEnemy", PhotonTargets.OthersBuffered, lastHitEnemyNumber, (int)lastHitEnemy.GetHP());
                    myPlayerScript.dealDmg = false; 
                }
                if (myPlayerScript.healPlayer)
                {
                    Debug.Log(myPlayerScript.healPlayer);
                    photonView.RPC("Heal", PhotonTargets.OthersBuffered, lastHitPlayer, (int)lastHitPlayer.GetHP());
                    myPlayerScript.healPlayer = false; 
                }
                if (myPlayerScript.HP <= 0)
                {
                    Application.Quit();
                }
            }
        }else
        {
            //Making smooth movement here
            this.transform.localPosition = Vector3.Lerp(this.transform.position, receivedBodyPos, Time.deltaTime * 10);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, receivedBodyRota, Time.deltaTime * 10);
            avatar.transform.localPosition = Vector3.Lerp(avatar.transform.localPosition, receivedHeadPos, Time.deltaTime * 10);
            rotationQuaternion = Quaternion.Lerp(avatar.transform.rotation, receivedHeadRota, Time.deltaTime * 10);
            avatar.transform.rotation = Quaternion.Euler(new Vector3(90f, rotationQuaternion.y, 0f)); //needs testing.
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(playerGlobal.localPosition);
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

    [PunRPC]
    void DealDmgToEnemy(int enemyHit, int currentHP)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemiesInArray in allEnemies)
        {
            Enemy localEnemyInArray = enemiesInArray.GetComponent<Enemy>();

            if (localEnemyInArray.number == enemyHit)
            {
                localEnemyInArray.SetHP((sbyte)currentHP);
            }
        }
    }

    [PunRPC]
    void Heal(string name, int healValue)
    {
        Debug.Log("PunRPC heal");
        GameObject[] defenders = GameObject.FindGameObjectsWithTag("Player");
        foreach (var players in defenders)
        {
            localPlayersInArray = players.GetComponentInChildren<Player>();
            Debug.Log("For each");
            if (localPlayersInArray.name == name)
            {
                localPlayersInArray.SetHP((sbyte)(healValue));
                Debug.Log("HP was set");
            }
        }
    }
}