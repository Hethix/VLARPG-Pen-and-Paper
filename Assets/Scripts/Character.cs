using UnityEngine;
using System.Collections;

public class Character : Photon.MonoBehaviour {

    public sbyte maxHP; // Used to cap healing
    public sbyte HP; //Obvious. Note that since HP can go below 0, byte won't do. This puts a much lower cap on max health. 
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
    [PunRPC]
    public sbyte GetHP() 
    {
        return HP;
    }

    // HP setter
    [PunRPC]
    public void SetHP(sbyte _HP) 
    {
        HP = _HP;
    }

    public float GetDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    //Generate a random number between 1 and max, including both.
    public byte RollRandom()
    {
        byte temp = (byte)Random.Range(1, 21);  //Assumes the minimum value to be 1
        // Debug.Log("Roll " + "d" + max + " = " + temp);                                                                   Print
        return temp;
    }


    public byte RollRandom(byte max) 
    {
        byte temp = (byte)Random.Range(1, max+1);  //Assumes the minimum value to be 1
        // Debug.Log("Roll " + "d" + max + " = " + temp);                                                                   Print
        return temp;
    }

    //Standard attack
    public void PerformAttack(Character defender) 
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
    public void DealDamage(Character defender, bool crit)
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

    //Takes a float of time which is added to the general cooldown (time until next ability can be used)
    public void SetCooldown(float timeInSecs) { 
        cooldown = Time.time + timeInSecs;
        Debug.Log("Cooldown set to .. " + timeInSecs);
    }

    // Returns true/false whether or not the cooldown has run out. 
    public bool CheckCooldown() {  
        if (cooldown < Time.time)
            return true;
        else
            return false;
    }

    // Detects collision, and performs attack if other object is labelled Enemy
    /* Will properly move this into "Enemy" and "Player"
    void OnCollisionEnter(Collision target)
    {
        Character chara = target.gameObject.GetComponent<Character>();
        if (target.gameObject.tag == "Enemy" && this.tag != "Enemy") //If a player hits an enemy
        {
            if (CheckCooldown() == true)
            {
                PerformAttack(chara);

            }
        } else if(target.gameObject.tag == "Player" && this.tag != "Player") //If an enemy hits a player
        {
            if (CheckCooldown() == true)
            {
                PerformAttack(chara);
            }
        }
    } */

}
