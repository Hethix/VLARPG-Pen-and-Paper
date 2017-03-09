using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    
    public sbyte HP; //Obvious
    public byte attack; // attack is a modifier for the attack roll
    public byte dmg; // Modifier applied to damage rolls
    public byte weaponDmg; // This is the die size used for damage
    public byte AC; //Armour Class
    public float cooldown; 
    Random random = new Random();


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    // HP getter
    public sbyte GetHP() 
    {
        return HP;
    }

    // HP setter
    public void SetHP(sbyte _HP) 
    {
        HP = _HP;
    }

    //Generate a random number between 1 and max, including both.
    byte RollRandom(byte max) 
    {
        byte temp = (byte)Random.Range(1, max+1);  //Assumes the minimum value to be 1
        // Debug.Log("Roll " + "d" + max + " = " + temp);                                                                   Print
        return temp;
    }

    //Standard attack
    void PerformAttack(Character defender) 
    {
        if (CheckCooldown() == true)
        {
            byte tempHit = RollRandom(20);
            if (tempHit == 20)
            {
                //Debug.Log("Critical!");                                                                                       Print
                DealDamage(defender, true);
            }
            else if (tempHit + attack >= defender.AC)
            {
                //Debug.Log("Hit!");                                                                                            Print
                DealDamage(defender, false);

            }
            else
            {
                //Debug.Log("miss!");                                                                                           Print
            }
            SetCooldown(2);
        }
    }

    //Apply damage from standard attack
    void DealDamage(Character defender, bool crit)
    {
        byte tempDmg; 
        if (crit == true) {
            tempDmg = (byte)(RollRandom(weaponDmg) + RollRandom(weaponDmg) + dmg);
            // Debug.Log(tempDmg);                                                                                         Print
        }
        else
        {
            tempDmg = (byte)(RollRandom(weaponDmg) + dmg);
            // Debug.Log("dmg: " + tempDmg);                                                                              Print
        }
        
        if (defender.GetHP() - tempDmg <= 0)
        {
            defender.Die();
        }
        else
        {
            defender.SetHP((sbyte)(defender.GetHP() - tempDmg));
        }
    }

    // Remove slain object
    void Die() 
    {
        Destroy(gameObject);
    }

    //Takes a float of time which is added to the general cooldown (time until net ability can be used)
    void SetCooldown(float timeInSecs) { 
        cooldown = Time.time + timeInSecs; 
    }

    // Returns true/false whether or not the cooldown has run out. 
    bool CheckCooldown() {  
        if (cooldown < Time.time)
            return true;
        else
            return false;
    }

    // Detects collision, and performs attack if other object is labelled Enemy
    void OnCollisionEnter(Collision target)
    {
        Character chara = target.gameObject.GetComponent<Character>();
        if (target.gameObject.tag.Equals("Enemy") == true) 
            if (CheckCooldown() == true)
                PerformAttack(chara);
    }

}
