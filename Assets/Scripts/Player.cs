using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public float thrust; // Use this to find a fitting speed. 
    public bool inStealth;


    //Attributes
    byte trapFinding;
    byte trailFollowing;
    byte healingSkill;
    public byte sneakSkill; // amount of units the enemies can see you in sneak. They can see 25 normally. Suggest 20 for warriors/mages, 10-15 for rogues.   

    //Sluk / tænd ild
    //Klatre
    //Find traps / investigate / Search
    //Åbne døre(Pick lock / brute force)
    //Følge et spor(maybe...)
    //Snige sig / liste
    //Heal
    //Remove trap
    //Sleight of hand

    // Attributes: 
    // Actions


    //System - small black button that brings up Big Picture overlay
    //ApplicationMenu - red button
    //Grip - the bumper-like buttons on the left and right sides of the controller.Both buttons send the same input
    //Axis0, Axis1, Axis2, Axis3, Axis4 - "Trigger and touchpad are aliases of these. Each 'axis' actually have both an x and y (not sure why), though only x is used for trigger."
    //Touchpad - pushing in the large circular touchpad
    //Trigger - front button


    void Start()
    {

    }

    void Update()
    {



        Stealth();
    }


    public void Heal(Character defender)
    {
        if (CheckCooldown() == true)
        {
            defender.SetHP((sbyte)(GetHP() + healingSkill)); 
            SetCooldown(10);
        }
    }

    public void Climb()
    {
        // Not written. Consider VRTK
    }

    public void Stealth()
    {
        if (gameObject.transform.position.y > 2) // Using placeholder value
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
