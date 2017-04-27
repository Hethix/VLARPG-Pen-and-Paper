using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Behaviour : MonoBehaviour {



   private Vector3 start_pos;
    private Vector3 rnd_dir;

    //Påvirker radius hvor den kan roame selve den større cirkel
    private float roamRadius = 10;
   NavMeshAgent agent;
   private Vector3 finalPosition;
    private float timer;
    private float wanderTimer = 5.0f;
    private int playerAmount;


    // Use this for initialization
    void Start () {
       agent = GetComponent<NavMeshAgent>();
        start_pos = transform.position;
        
	}
	
	// Update is called once per frame
	void Update () {

       
       // for(int i = 0, i < )
        Debug.Log(start_pos);

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            agent.SetDestination(finalPosition);
            timer = 0;
            newDestination();
        }
        NavMeshHit hit;
        NavMesh.SamplePosition(rnd_dir, out hit, roamRadius, 1);
        finalPosition = hit.position;
    }

    public void newDestination()
    {
        rnd_dir = Random.insideUnitSphere * roamRadius;
        rnd_dir += start_pos;
    }
}


