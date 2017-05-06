using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{

    private GameObject left;
    private GameObject right;
    private int count = 0;


    // Use this for initialization
    void Start()
    {


        left = transform.FindChild("Lefty").gameObject;
        right = transform.FindChild("Righty").gameObject;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Lars" && count == 0)
        {
            Debug.Log("Lars with da great DICK");
            left.transform.Rotate(-50f, 0, 0);
            right.transform.Rotate(50f, 0, 0);
            count += 1;
        }
    }
}
