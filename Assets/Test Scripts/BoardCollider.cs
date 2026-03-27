using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCollider : MonoBehaviour
{
    public List<bool> collidingObjStatements;
    public List<GameObject> collidingObjs;
    public AudioClip collideSFX;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject.ToString() + " Collides with " + collision.gameObject.ToString());
        if (collision.collider.tag == "canPickup")
        {
            if(collision.collider.gameObject.GetComponent<Collisions>().isPickedUp == false)
            {
                AudioSource playCollide = collision.gameObject.GetComponent<AudioSource>();

                playCollide.Play();

                //Debug.Log("Colliding and not pickedup");

                collision.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                //collision.rigidbody.linearVelocity = Vector3.zero;
                //collision.rigidbody.angularVelocity = Vector3.zero;
                //collision.rigidbody.freezeRotation = true;
                collision.rigidbody.useGravity = false;
                //collision.rigidbody.mass = 10000;

                collidingObjs.Add(collision.collider.gameObject);
                //collidingObjStatements.Add(true);
            }
            //collision.gameObject.layer = 9;
            //collision.rigidbody.isKinematic = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "canPickup")
        {
            if (collision.collider.gameObject.GetComponent<Collisions>().isPickedUp == false)
            {
                int check = collidingObjs.IndexOf(collision.collider.gameObject);

                if (check == -1)
                {
                    //Debug.Log("Colliding now and not pickedup");

                    collision.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    //collision.rigidbody.linearVelocity = Vector3.zero;
                    //collision.rigidbody.angularVelocity = Vector3.zero;
                    //collision.rigidbody.freezeRotation = true;
                    collision.rigidbody.useGravity = false;
                    //collision.rigidbody.mass = 10000;

                    collidingObjs.Add(collision.collider.gameObject);
                    //collidingObjStatements.Add(true);
                }

            }
            else if (collision.collider.gameObject.GetComponent<Collisions>().isPickedUp == true)
            {
                int check = collidingObjs.IndexOf(collision.collider.gameObject);

                if (check != -1)
                {
                    //check = collidingObjs.IndexOf(collision.collider.gameObject);
                    collidingObjs.Remove(collision.collider.gameObject);
                    //collidingObjStatements.RemoveAt(check);
                }
            }
            //collision.gameObject.layer = 9;
            //collision.rigidbody.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //int check = collidingObjs.IndexOf(collision.collider.gameObject);
        collidingObjs.Remove(collision.collider.gameObject);
        //collidingObjStatements.RemoveAt(check);
    }
}
