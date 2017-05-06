using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Player {

    void Start()
    {
        //Attributes
        trapFinding = 5;
        trailFollowing = 5;
        healingSkill = 6;
        sneakSkill = 20;
        maxHP = 20;
        HP = 20; //Obvious. Note that since HP can go below 0, byte won't do. This puts a much lower cap on max health. 
        attack = 6; // attack is a modifier for the attack roll
        dmg = 4; // Modifier applied to damage rolls
        weaponDmg = 8; // This is the die size used for damage
        AC = 11; //Armour Class
    }

    void Update()
    {

    }
}
