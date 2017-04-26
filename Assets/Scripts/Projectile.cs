using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(DestroyAfterTime(5));
    }

    // Update is called once per frame
    void Update () {
        rb.AddForce(gameObject.transform.forward * 8);


    }


    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
