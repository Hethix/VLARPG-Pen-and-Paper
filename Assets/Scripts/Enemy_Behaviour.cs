using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Behaviour : MonoBehaviour {



   private Vector3 start_pos;
    private Vector3 rnd_dir;
    private float roamRadius = 5;
   

    // Use this for initialization
    void Start () {
        start_pos = transform.position;
        
	}
	
	// Update is called once per frame
	void Update () {
        
       
       

        Debug.Log(rnd_dir);
	}

   public void FreeRoam()
    {
        rnd_dir = Random.insideUnitSphere * roamRadius;
        rnd_dir += start_pos;
        
        //NavMeshHit hit;
        //NavMesh.SamplePosition(rnd_dir, out hit, roamRadius, 1);
        //Vector3 finalPosition = hit.position;
       //_nav.destination = finalPosition;
    }
}


