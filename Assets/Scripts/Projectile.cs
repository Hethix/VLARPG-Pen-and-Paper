﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    Rigidbody rb;
    Player selfPlayer;
    Enemy chara;
    public NetworkedPlayer networkPlayer;


    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        selfPlayer = GetComponentInParent<Player>();
        StartCoroutine(DestroyAfterTime(4));
        Physics.IgnoreCollision(gameObject.GetComponentInParent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(gameObject.transform.forward * 12);
    }


    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider target)
    {
        Debug.Log("I hit: " + target);
        switch (target.gameObject.tag.ToString())
        {
            case "Enemy":
                chara = target.gameObject.GetComponent<Enemy>();
                if (target.gameObject.tag.Equals("Enemy") == true)
                {
                    selfPlayer.PerformAttack(chara);
                    networkPlayer.lastHitEnemy = chara;
                    selfPlayer.dealDmg = true;
                }
                Destroy(gameObject);
                break;
            default:
                //Destroy(gameObject);
                break;
        }
    }

}
