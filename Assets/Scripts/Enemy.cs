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
    public bool beingMoved; //Change to private when done
    public bool giveNewDestination;
    public bool giveNewStartPosition;
    public bool setPlayerHP;
    public sbyte setPlayerHpAmount;
    public Player lastHitPlayer;
    public bool performBumpAttack;

    public bool ownedByGM = false;
    public int number; 


    /*Things to do/test:
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
        giveNewDestination = false;
        setPlayerHP = false;
        performBumpAttack = false;
    }

	void Update () {
        if (ownedByGM)
        {
            if (item.currentlyInteracting)
            {
                if (agent.enabled)
                {
                    beingMoved = true;
                    agent.Stop(true); //Fuck visual studio and their recommendations!
                }
            }
            else if (!item.currentlyInteracting)
            {
                if (beingMoved)
                {
                    start_pos = transform.position;
                    agent.Resume();
                    giveNewStartPosition = true;
                    beingMoved = false;
                }
                if (agent.enabled)
                {
                    Timer();
                }
            }
            if (finalPosition != agent.destination)
            {
                if (!item.currentlyInteracting && !beingMoved)
                {
                    agent.SetDestination(finalPosition);
                    giveNewDestination = true;
                }
            }
        }
        /* if (performBumpAttack)
        {
            Debug.Log("Attack performed");
            bool changeBumpDirection = false;
            for(int i = 0; i < 2; i++)
            {
                Debug.Log("Entered forloop");
                Transform child = gameObject.transform.GetChild(i);
                if (child.rotation.x > 99)
                {
                    changeBumpDirection = true;
                }
                if (!changeBumpDirection)
                {
                    Debug.Log("Reset animation??");
                    child.localRotation = Quaternion.Euler(Mathf.Lerp(child.localRotation.x, 100, Time.fixedDeltaTime), child.rotation.y, child.rotation.z);
                } else
                {
                    child.localRotation = Quaternion.Euler(Mathf.Lerp(child.localRotation.x, 90, Time.fixedDeltaTime), child.rotation.y, child.rotation.z);
                    if(child.rotation.x < 91)
                    {
                        child.localRotation = Quaternion.Euler(90, child.rotation.y, child.rotation.z);
                        changeBumpDirection = true;
                        performBumpAttack = false;
                    }
                }
            }

        } */
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

    // Cannot serialize gameObject. Find another solution with strings / or other method
    void OnTriggerStay(Collider target)
    {
        if (ownedByGM)
        {
            if (CheckCooldown() == true)
            { 
                if (target.gameObject.tag.Equals("Player")) //If a player hits an enemy
                {
                    Debug.Log("Attack performed"); 
                    Player player = target.gameObject.GetComponent<Player>();
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
