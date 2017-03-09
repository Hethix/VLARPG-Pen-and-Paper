using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour {


    //Range();
    Transform trapTrans;
    GameObject go;
    Transform playerTrans;
    float max = 10f;
    //float distancePlayer;

    //Hiddeness();
    int factor = 1;

    //Damage();
    byte dmg;
    Rigidbody playerRig;

    //Timer
    private IEnumerator coroutine;

    //Difficulty

    public int difficulty = 10;



   

    // Use this for initialization
    void Start () {

        //Range();
        go = GameObject.Find("Player");
        trapTrans = GetComponent<Transform>();
        playerTrans = go.GetComponent<Transform>();

        //Hiddeness(); 
        gameObject.GetComponent<Renderer>().enabled = false;

        //Damage();
        playerRig = go.GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void Update () {
        

        //Den skal have en sværhedsgrad 
        //roll 20 på damage
        //Den skal destroy sig selv efter den aktivere. Eksempel: først give skade (activate) og så ødelægge sig selv (destroy)
        Range();
        Hiddeness();
        Damage();

    }
    void Damage() {
        if (Input.GetKeyDown(KeyCode.R)) {
            dmg = RollRandom(20);
            Debug.Log(dmg);

        }
    }
    void Range() {
        if ((Vector3.Distance(trapTrans.position, playerTrans.position)) < max ) {

            //distancePlayer = Vector3.Distance(trapTrans.position, playerTrans.position);
            if(factor > 0 && factor < 2) { 
                factor += 1;
            }
           // Debug.Log(distancePlayer + " " + factor);
                
        }

    }
    void Hiddeness() {
        if (factor == 2)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }
    }

    byte RollRandom(byte max) //Generate a random number between 1 and max, including both.
    {
            byte temp = (byte)Random.Range(1, max + 1);  //Assumes the minimum value to be 1
            Debug.Log("Roll " + "d" + max + " = " + temp);
            return temp;
        }

    void OnCollisionEnter(Collision col)
    {
        
        if (col.gameObject.name == "Player")
        {
            playerRig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            RollRandom(20);
            coroutine = Wait();
            StartCoroutine(coroutine);
            
           
        }

    }
    IEnumerator Wait() {

        Debug.Log(Time.time);
        yield return new WaitForSeconds(3);
        Debug.Log(Time.time);
        playerRig.constraints = RigidbodyConstraints.None;
        Destroy(gameObject);
    }
}
