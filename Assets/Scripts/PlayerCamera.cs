using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour {

    public Image img; 
    Player selfPlayer;
    sbyte HPa;
    sbyte HPb;

    void Start()
    {
        selfPlayer = transform.root.GetComponentInChildren<Player>();
    }

    void Update()
    {

        HPa = selfPlayer.GetHP();
        if (HPa != HPb) img.GetComponent<Image>().color = new Color32((byte)((selfPlayer.maxHP - selfPlayer.GetHP())*5), 0, 0, 100);
        HPb = HPa; 
    }
}
