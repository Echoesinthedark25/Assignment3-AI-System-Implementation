using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRadius : MonoBehaviour
{
    public SimpleStateMachine npc;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (npc.soundHeard)
        {
            npc.alertRadius = this;
        }
    }
}
