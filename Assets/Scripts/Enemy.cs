using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character {

    public float DistToPlayer; //Distance to nearest player
    public bool combatReady; // Use this to activate combat mechanics. 

    public Vector3 start_pos;
    private Vector3 rnd_dir;

    private float roamRadius = 10;
    public NavMeshAgent agent;
    public Vector3 finalPosition;
    public float timer;
    private float wanderTimer = 5.0f;
    private GameObject[] players;
    private NavMeshHit hit;
    public Player player;
    private InteractableItem item;
    public Vector3 agentDestination;
    private bool beingMoved;
    public bool giveNewDestination;
    public bool giveNewStartPosition;
    public bool setPlayerHP;
    public sbyte setPlayerHpAmount;
    public Player lastHitPlayer;
    public bool performBumpAttack;

    public bool ownedByGM = false;



    /*Things to do:
    - Make enemies able to attack players by bumping into them
    - Fix GM picking up enemies - For some reason they are below the ground???
    - Syncronizing health and damage dealt?
    */

    void Start() {
        combatReady = false;
        agent = GetComponent<NavMeshAgent>();
        start_pos = transform.position;
        timer = 4.5f; //made so it starts wandering soon after being created
        item = GetComponent<InteractableItem>();
        agentDestination = agent.destination;
        giveNewDestination = false;
        setPlayerHP = false;
        performBumpAttack = false;
    }

	void Update () {
        if (ownedByGM)
        {
            if (finalPosition != agent.destination)
            {
                if (agent.enabled)
                {
                    agent.SetDestination(finalPosition);
                    giveNewDestination = true;
                }
            }
            if (item.currentlyInteracting)
            {
                if (agent.enabled)
                {
                    beingMoved = true;
                    agent.Stop();
                    agent.enabled = false;
                }
            }
            else if (!item.currentlyInteracting)
            {
                if (beingMoved)
                {
                    start_pos = transform.position;
                    beingMoved = false;
                    agent.enabled = true;
                    agent.Resume();
                    giveNewStartPosition = true;
                }
                if (agent.enabled)
                {
                    Timer();
                }
            }
        }
        if (performBumpAttack)
        {
            bool changeBumpDirection = false;
            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform child = gameObject.transform.GetChild(i);
                if (child.rotation.x > 99)
                {
                    changeBumpDirection = true;
                }
                if (!changeBumpDirection)
                {
                    child.localRotation = Quaternion.Euler(Mathf.Lerp(child.localRotation.x, 100, Time.fixedDeltaTime), child.rotation.y, child.rotation.z);
                } else
                {
                    child.localRotation = Quaternion.Euler(Mathf.Lerp(child.localRotation.x, 90, Time.fixedDeltaTime), child.rotation.y, child.rotation.z);
                    if(child.rotation.x < 91)
                    {
                        performBumpAttack = false;
                    }
                }
            }

        }
    }

    void DetectPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player"); //possible that this is VERY inefficient
        RandomizeArray(players); // Because of the randomization, the enemies should choose a random target from those nearby. 
        foreach (var obj in players)
        {
            player = obj.GetComponent<Player>();
            if (GetDistance(gameObject, obj) < 0.1f && player.inStealth==false)
            {
                combatReady = true;
                timer = 3.5f; 
                break;  
                
            }
            else if (GetDistance(gameObject, obj) < 25-player.sneakSkill && player.inStealth == true)
            {
                combatReady = true;
                timer = 3.5f;
                break;  
            }
            else
            {
                combatReady = false;
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
            DetectPlayers();
            if (combatReady)
            {
                NavMesh.SamplePosition(player.transform.position, out hit, 50, 1); //GO FUCK UP PLAYER IF HE IS COMBATREADY KILL KILL FAGGOTS
            }
            else
            {
                NewDestination();
                NavMesh.SamplePosition(rnd_dir, out hit, roamRadius, 1); //roam in nearby start area.
            }
            finalPosition = hit.position;
            timer = 0;
        }
    }

    void NewDestination()
    {
        rnd_dir = Random.insideUnitSphere * roamRadius;
        rnd_dir += start_pos;
    }




    void OnCollisionEnter(Collision target)
    {
        if (ownedByGM)
        {
            if (target.gameObject.tag == "Player") //If a player hits an enemy
            {
                Player player = target.gameObject.GetComponent<Player>();
                if (CheckCooldown() == true)
                {
                    PerformAttack(player);
                    setPlayerHpAmount = player.GetHP();
                    lastHitPlayer = player;
                    setPlayerHP = true;
                    performBumpAttack = true;
                }
            }
        }
    }

    
}
