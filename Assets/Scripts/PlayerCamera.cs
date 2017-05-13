using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour {

    public Image img; 
    Player selfPlayer;
    sbyte HPa;
    sbyte HPb;
    sbyte mHP;


    void Start()
    {
        selfPlayer = transform.root.GetComponentInChildren<Player>();
        mHP = selfPlayer.maxHP;
        img = GetComponent<Image>();
    }

    void Update()
    {

        HPa = selfPlayer.GetHP();
        if (HPa != HPb) img.color = new Color32((byte)((100-((HPa/mHP)*100))*2), 0, 0, 100);
        HPb = HPa; 
    }
}
