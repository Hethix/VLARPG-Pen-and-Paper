using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magus : Player {

	// Use this for initialization
	void Start () {
        //Attributes
        trapFinding = 0;
        trailFollowing = 0;
        healingSkill = 8;
        sneakSkill = 10;
        maxHP = 16;
        HP = 2; //Obvious. Note that since HP can go below 0, byte won't do. This puts a much lower cap on max health. 
        attack = 8; // attack is a modifier for the attack roll
        dmg = 2; // Modifier applied to damage rolls
        weaponDmg = 12; // This is the die size used for damage
        AC = 12; //Armour Class
}
	

}
