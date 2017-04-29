using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character {

    public float DistToPlayer; //Distance to nearest player
    bool AI = false; // Use this to deactivate all behaviour scripts
    public bool combatReady; // Use this to activate combat mechanics. 

    private Vector3 start_pos;
    private Vector3 rnd_dir;

    private float roamRadius = 10;
    NavMeshAgent agent;
    public Vector3 finalPosition;
    public float timer;
    private float wanderTimer = 5.0f;
    private GameObject[] players;
    private NavMeshHit hit;
    public Player player;
    private InteractableItem item;
    public Vector3 agentDestination;
    private bool beingMoved;

    void Start() {
        combatReady = false;
        agent = GetComponent<NavMeshAgent>();
        start_pos = transform.position;
        timer = 4.5f; //made so it starts wandering soon after being created
        item = GetComponent<InteractableItem>();
        agentDestination = agent.destination;
    }

	void Update () {
        if(finalPosition != agent.destination)
        {
            agent.SetDestination(finalPosition);
        }
        if (item.currentlyInteracting)
        {
            beingMoved = true;
            agent.Stop();
        } else if (!item.currentlyInteracting)
        {
            if (beingMoved)
            {
                start_pos = transform.position;
                beingMoved = false;
                agent.Resume();
            }
            Timer();
            
        }
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
            player = obj.GetComponent<Player>();
            if (GetDistance(gameObject, obj) < 25 && player.inStealth==false)
            {
                combatReady = true;
                timer = 3.5f; 
                break;  
                
            } else if (GetDistance(gameObject, obj) < 25-player.sneakSkill && player.inStealth == true)
            {
                combatReady = true;
                timer = 3.5f;
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
            if (combatReady)
            {
                NavMesh.SamplePosition(player.transform.position, out hit, 50, 1); //GO FUCK UP PLAYER IF HE IS COMBATREADY KILL KILL FAGGOTS
            }
            else
            {
                newDestination();
                NavMesh.SamplePosition(rnd_dir, out hit, roamRadius, 1); //roam in nearby start area.
            }
            finalPosition = hit.position;
            timer = 0;
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
