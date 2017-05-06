using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    Rigidbody rb;
    Player sender;
    Character chara;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sender = GetComponentInParent<Player>();
        StartCoroutine(DestroyAfterTime(5));
        Physics.IgnoreCollision(gameObject.GetComponentInParent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(gameObject.transform.up * -8);


    }


    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision target)
    {

        switch (target.gameObject.tag.ToString())
        {
            case "Enemy":
                chara = target.gameObject.GetComponent<Character>();
                if (target.gameObject.tag.Equals("Enemy") == true)
                    sender.PerformAttack(chara);
                Debug.Log("Damage was applied ...");                                                        // Print
                Destroy(gameObject);
                break;
            default:
                Destroy(gameObject);
                break;


        }
    }

}
