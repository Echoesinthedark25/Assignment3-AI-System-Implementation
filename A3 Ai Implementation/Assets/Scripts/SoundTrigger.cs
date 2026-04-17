using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public Collider soundTrigger;
    public GameObject soundRadius;
    


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActivateRadius();
        }
    }

    private void ActivateRadius()
    {
        soundRadius.SetActive(true);

        Invoke("DeactivateRadius", 1f);
    }

    private void DeactivateRadius()
    {
        soundRadius.SetActive(false);
    }

    public void Update()
    {

    }


}
