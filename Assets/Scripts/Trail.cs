using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour {

    public byte difficulty = 10; // Difficulty set to a standard of 10 
    float timeAlive = 300; // Default time alive set to 5 minutes

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<ParticleSystem>().Stop(); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void CallDestroyAfterTime()
    {
        StartCoroutine(DestroyAfterTime(timeAlive));
    }


    private IEnumerator DestroyAfterTime(float time) { 
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
            

    }
}
