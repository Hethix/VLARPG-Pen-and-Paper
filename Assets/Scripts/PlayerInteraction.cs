using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    Character chara;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // TEMP for movement on laptop                                                                              Delete after testing
        //if (Input.GetKey(KeyCode.W))
        //    gameObject.transform.position += Vector3.forward * 10 * Time.deltaTime;
        //if (Input.GetKey(KeyCode.A))
        //    gameObject.transform.position += Vector3.left * 10 * Time.deltaTime;
        //if (Input.GetKey(KeyCode.S))
        //    gameObject.transform.position += Vector3.back * 10 * Time.deltaTime;
        //if (Input.GetKey(KeyCode.D))
        //    gameObject.transform.position += Vector3.right * 10 * Time.deltaTime;

    }

    // Detects collision, and performs attack/heal depending on friend or enemy
    void OnCollisionEnter(Collision target)
    {
        Player self = GetComponentInParent<Player>();

        switch (target.gameObject.tag.ToString())
        {
            case "Enemy":
                chara = target.gameObject.GetComponent<Character>();
                if (target.gameObject.tag.Equals("Enemy") == true)
                    if (self.CheckCooldown() == true)
                    {
                        self.PerformAttack(chara);
                        Debug.Log("Damage was applied ...");                                                        // Print
                    }
                break;

            case "Ally":
                chara = target.gameObject.GetComponent<Character>();
                if (target.gameObject.tag.Equals("Ally") == true) //Consider if a button-press should be added, or the interaction could be improved. 
                    if (self.CheckCooldown() == true)
                    {
                        self.Heal(chara);
                        Debug.Log("Healing was applied...");                                                        // Print
                    }
                break;
            default:
                Debug.Log("The target does not have any of the tags defined in this switch.");
                break;
        }

            
    }
}
