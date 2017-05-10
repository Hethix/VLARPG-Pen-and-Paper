using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public bool inStealth;
    //Attributes
    protected byte trapFinding;
    protected byte trailFollowing;
    public byte healingSkill;
    public byte sneakSkill; // amount of units the enemies can see you in sneak. They can see 25 normally. Suggest 20 for warriors/mages, 10-15 for rogues.   

    public bool dealDmg;
    public bool healPlayer;
    public NetworkedPlayer networkPlayer;


    //System - small black button that brings up Big Picture overlay
    //ApplicationMenu - red button
    //Grip - the bumper-like buttons on the left and right sides of the controller.Both buttons send the same input
    //Axis0, Axis1, Axis2, Axis3, Axis4 - "Trigger and touchpad are aliases of these. Each 'axis' actually have both an x and y (not sure why), though only x is used for trigger."
    //Touchpad - pushing in the large circular touchpad
    //Trigger - front button


    void Start()
    {
        dealDmg = false;
        healPlayer = false;

    }

    void Update()
    {
        // Stealth();
        
    }

    public void Stealth()
    {
        if (gameObject.transform.position.y < 2) // Using placeholder value
            inStealth = true;
        else
            inStealth = false; 
    }

    public void Search()
    {
        if (CheckCooldown() == true)
        {
            StartCoroutine(IESearch(1)); // 1 is the time used for duration of the particle "effect"
        }
    }

    public void Healing(Player defender)
    {
        SetCooldown(10);
        if (defender.GetHP() + healingSkill > defender.maxHP)
            Debug.Log("Max healed");
            defender.SetHP(defender.maxHP);
        else
            defender.SetHP((sbyte)(defender.GetHP() + healingSkill));
        Debug.Log(defender.GetHP() + " healed");
        healPlayer = true;
    }

    private IEnumerator IESearch(float time)
    {
        SetCooldown(30); // Set cooldown for SEARCH
        var partiSystem = Instantiate(Resources.Load("DetectTrapsEffect"), transform);
        yield return new WaitForSeconds(time);
        Destroy(partiSystem);
        byte searchRoll = RollRandom();
        Debug.Log(searchRoll);
        var objects = GameObject.FindGameObjectsWithTag("Trap");
        foreach (var obj in objects)
        {
            Traps trapComponent = obj.GetComponent<Traps>();
            if (trapComponent.GetDistanceToPlayer() < 25) // Magical number: How far can you detect traps. Also equals radius of circular particleSystem. 
                if (searchRoll + trapFinding > trapComponent.difficulty)
                    trapComponent.GetComponent<Renderer>().enabled = true;
        }

        var waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (var obj in waypoints)
        {
            Trail trailComponent = obj.GetComponent<Trail>();

            if (GetDistance(gameObject, obj) < 25 && searchRoll + trailFollowing > obj.GetComponent<Trail>().difficulty)
            {
                obj.GetComponent<ParticleSystem>().Play();
                trailComponent.CallDestroyAfterTime();
            }
        }
    }


}
