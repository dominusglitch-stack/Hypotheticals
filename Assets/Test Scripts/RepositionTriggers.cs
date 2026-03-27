using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionTriggers : MonoBehaviour
{
    //public AudioClip respawnSFX;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.ToString() + " Triggered by " + other.ToString());
        if (other.gameObject.tag == "canPickup")
        {
            Collisions outside = other.GetComponent<Collisions>();
            //AudioSource playRespawn = other.GetComponent<AudioSource>();

            if (!outside.outside)
            {
                if (!outside.isPickedUp)
                {
                    //Debug.Log("Outside!");
                    outside.Invoke("RepositionObject", 2f);
                    outside.outside = true;
                }
            }
        }
    }
}
