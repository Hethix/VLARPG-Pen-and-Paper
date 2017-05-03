using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour {

    Player selfPlayer;
    Character chara; 

    // Use this for initialization
    void Start () {
        selfPlayer = gameObject.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision target)
    {

        switch (target.gameObject.tag.ToString())
        {
            case "Enemy":
                chara = target.gameObject.GetComponent<Character>();
                if (target.gameObject.tag.Equals("Enemy") == true)
                    if (selfPlayer.CheckCooldown() == true)
                    {
                        selfPlayer.PerformAttack(chara);
                        Debug.Log("Damage was applied ...");                                                        // Print
                    }
                break;
        }
    }
}
