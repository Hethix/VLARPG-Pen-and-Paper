using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour {

    Player selfPlayer;
    Character chara;
    public NetworkedPlayer networkPlayer; 

    // Use this for initialization
    void Start () {
        selfPlayer = transform.root.GetComponentInChildren<Player>();
        networkPlayer = transform.root.GetComponentInChildren<NetworkedPlayer>();

    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter(Collider target)
    {
        Debug.Log("Touched!");
        switch (target.gameObject.tag.ToString())
        {
            case "Enemy":
                Debug.Log("Hit!");
                chara = target.gameObject.GetComponent<Character>();
                if (target.gameObject.tag.Equals("Enemy") == true)
                    if (selfPlayer.CheckCooldown() == true)
                    {
                        selfPlayer.PerformAttack(chara);
                        networkPlayer.lastHitEnemy = target.GetComponent<Enemy>();
                         
                        selfPlayer.dealDmg = true; 
                        Debug.Log("Damage was applied ...");                                                        // Print
                    }
                break;
        }
    }
}
