using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour {

    public TextMesh Text;

    Player self; 

    // Use this for initialization
    void Start () {
        self = gameObject.GetComponentInParent<Player>();
        Text.text = ("HP: [" + self.GetHP() + "]");

    }

    // Update is called once per frame
    void Update () {
		
	}
}
