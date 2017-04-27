using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public float DistToPlayer; //Distance to nearest player
    bool AI = false; // Use this to deactivate all behaviour scripts
    bool combatReady; // Use this to activate combat mechanics. 

    void Start() {
        combatReady = false; 


    }

	void Update () {

    }

    void GoTo()
    {

    }

    void GoTo(GameObject player) //Overload of the GoTo() method, meant for going to a location near a player. 
    {

    }

    void Perception()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
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

    //void Roam() //Use the GoTo() Method to move to a random nearby location. 
    //{

    //}
}
