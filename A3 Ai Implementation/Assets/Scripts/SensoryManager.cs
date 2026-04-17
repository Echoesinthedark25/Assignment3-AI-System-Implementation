using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryManager : MonoBehaviour
{

    public SimpleStateMachine npc;
    public GameObject soundTrigger;


    private void Awake()
    {
        foreach (Transform child in soundTrigger.transform)
        {
            SoundTrigger soundTrigger = child.GetComponent<SoundTrigger>();
            if (soundTrigger != null)
            {
                
            }
        }
    }


}
