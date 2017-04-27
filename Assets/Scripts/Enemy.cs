using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character {

    public float DistToPlayer; //Distance to nearest player
    bool AI = false; // Use this to deactivate all behaviour scripts
    bool combatReady; // Use this to activate combat mechanics. 

    private Vector3 start_pos;
    private Vector3 rnd_dir;

    private float roamRadius = 10;
    NavMeshAgent agent;
    private Vector3 finalPosition;
    private float timer;
    private float wanderTimer = 5.0f;
    private GameObject[] players;

    void Start() {
        combatReady = false;
        agent = GetComponent<NavMeshAgent>();
        start_pos = transform.position;
        timer = 4.5f; //made so it starts wandering soon after being created

    }

	void Update () {
        NavMeshHit hit;
        NavMesh.SamplePosition(rnd_dir, out hit, roamRadius, 1);
        finalPosition = hit.position;
        if(finalPosition != agent.destination)
        {
            agent.destination = finalPosition;
        }
        Timer();
    }

    void GoTo()
    {

    }

    void GoTo(GameObject player) //Overload of the GoTo() method, meant for going to a location near a player. 
    {

    }

    void DetectPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player"); //possible that this is VERY inefficient
        RandomizeArray(players); // Because of the randomization, the enemies should choose a random target from those nearby. 
        foreach (var obj in players)
        {
            Player player = obj.GetComponent<Player>();
            if (GetDistance(gameObject, obj) < 25 && player.inStealth==false)
            {
                combatReady = true; 
                break;  
                
            } else if (GetDistance(gameObject, obj) < 25-player.sneakSkill && player.inStealth == true)
            {
                combatReady = true; 
                break;  
            }
        }
    }

    void RandomizeArray(GameObject[] playersArr) // Fisher Yates Shuffle
    {
        for (var i = playersArr.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = playersArr[i];
            playersArr[i] = playersArr[r];
            playersArr[r] = tmp;
        }
    }

    void Timer()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            agent.SetDestination(finalPosition);
            timer = 0;
            newDestination();
        }
    }

    void newDestination()
    {
        rnd_dir = Random.insideUnitSphere * roamRadius;
        rnd_dir += start_pos;
    }
    //void Roam() //Use the GoTo() Method to move to a random nearby location. 
    //{

    //}
}
