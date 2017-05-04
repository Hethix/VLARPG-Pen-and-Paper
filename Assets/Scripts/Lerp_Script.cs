using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp_Script : MonoBehaviour {

    // animate the game object from -1 to +1 and back
    public float minimum = 0;
    public float maximum = 0.15F;
    public Vector3 pos;
    // starting value for the Lerp
    static float t = 0.0f;
    static float tx = 0.0f;

    // Use this for initialization
    void Start () {
        pos = transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
       

        // animate the position of the game object...
        transform.position = new Vector3(0, Mathf.Lerp(minimum, maximum, 0.25f * tx), 0) + pos;

        // .. and increate the t interpolater
        t += 0.25f * Time.deltaTime;
        tx += 5.0f * Time.deltaTime;

        // now check if the interpolator has reached 1.0
        // and swap maximum and minimum so game object moves
        // in the opposite direction.
        if (t > 1.0f)
        {
            float temp = maximum;
            maximum = minimum;
            minimum = temp;
            t = 0.0f;
            tx = 0.0f;
        }

    }
}
