using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTrigger : MonoBehaviour
{
    public CaseGenerator caseGen;
    public GameObject correctWeapon = null;
    public GameObject correctLocation = null;
    public GameObject correctWitness = null;
    public GameObject tempWeapon = null;
    public bool isWeaponTrigger = false;
    public bool isLocationTrigger = false;
    public bool isWitnessTrigger = false;
    //public LineRenderer redString;
    //public List<LineRenderer> redStrings;
    public Transform[] boardObjs; 
    //public Transform weaponNotes;
    //public Transform locationNotes;
    //public Transform witnessNotes;

    [Header("Rope")]
    public int segments = 12;
    public float droop = 0.6f;
    public float maxSagPerMeter = 0.25f;
    public float ropeClearance = 0.04f;

    private void Start()
    {
        if (boardObjs != null)
        {
            foreach (Transform t in boardObjs)
            {
                ManageLocations(t, true);
                GameSceneManager.instance.ManageStrings(t);
                GameSceneManager.instance.DrawRope(t);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        //Debug.Log("Colliding " + other.ToString());

        if (other.gameObject.tag == "canPickup")
        {
            //Debug.Log("Step 1");

            //redString.enabled = true;

            if (isWeaponTrigger)
            {
                if (other.gameObject.GetComponent<ObjectAttributes>())
                {
                    Collisions colliderBool = other.gameObject.GetComponent<Collisions>();
                    colliderBool.inOtherCollider[0] = true;

                    if (correctWeapon == null)
                    {
                        tempWeapon = other.gameObject;
                        caseGen.UpdateCaseWeapon(other.gameObject);

                        ManageLocations(other.transform, true);
                        GameSceneManager.instance.ManageStrings(other.transform);

                        GameSceneManager.instance.DrawRope(other.transform);
                    }
                    else
                    {
                        ManageLocations(other.transform, true);
                        GameSceneManager.instance.ManageStrings(other.transform);

                        GameSceneManager.instance.DrawRope(other.transform);
                    }
                }
                else
                {
                    Collisions colliderBool = other.gameObject.GetComponent<Collisions>();
                    colliderBool.inOtherCollider[0] = true;
                }
            }
            else if (isLocationTrigger)
            {
                if (other.gameObject.GetComponent<CaseSets>())
                {
                    if (correctLocation == null)
                    {
                        caseGen.UpdateCaseLocation(other.gameObject);

                        ManageLocations(other.transform, true);
                        GameSceneManager.instance.ManageStrings(other.transform);

                        GameSceneManager.instance.DrawRope(other.transform);
                    }
                    else
                    {
                        ManageLocations(other.transform, true);
                        GameSceneManager.instance.ManageStrings(other.transform);

                        GameSceneManager.instance.DrawRope(other.transform);
                    }

                    Collisions colliderBool = other.gameObject.GetComponent<Collisions>();
                    colliderBool.inOtherCollider[1] = true;
                }
                else
                {
                    Collisions colliderBool = other.gameObject.GetComponent<Collisions>();
                    colliderBool.inOtherCollider[1] = true;
                    //ManageLocations(other.transform, true);
                    //GameSceneManager.instance.ManageStrings(other.transform);
                }
            }
            else
            {
                //Debug.Log("Step 2");

                if (other.gameObject.GetComponent<CaseSets>())
                {
                    //Debug.Log("Step 3");

                    if (correctWitness == null)
                    {
                        //Debug.Log("Step 4");

                        caseGen.UpdateCaseWitness(other.gameObject);

                        //Debug.Log(other.gameObject + " has entered");

                        ManageLocations(other.transform, true);
                        GameSceneManager.instance.ManageStrings(other.transform);

                        GameSceneManager.instance.DrawRope(other.transform);
                    }
                    else
                    {
                        ManageLocations(other.transform, true);
                        GameSceneManager.instance.ManageStrings(other.transform);

                        GameSceneManager.instance.DrawRope(other.transform);
                    }

                    Collisions colliderBool = other.gameObject.GetComponent<Collisions>();
                    colliderBool.inOtherCollider[2] = true;
                }
                else
                {

                    //Debug.Log(other.gameObject + " has entered");

                    Collisions colliderBool = other.gameObject.GetComponent<Collisions>();
                    colliderBool.inOtherCollider[2] = true;
                    //ManageLocations(other.transform, true);
                    //GameSceneManager.instance.ManageStrings(other.transform);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "canPickup")
        {
            if(isWeaponTrigger)
            {
                GameSceneManager.instance.DrawRope(other.transform);
            }
            else if(isLocationTrigger)
            {
                if (other.gameObject.GetComponent<CaseSets>())
                {
                    GameSceneManager.instance.DrawRope(other.transform);
                }
                
            }
            else
            {
                if (other.gameObject.GetComponent<CaseSets>())
                {
                    GameSceneManager.instance.DrawRope(other.transform);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exiting " + other.ToString());

        if (other.gameObject.tag == "canPickup")
        {
            if (isWeaponTrigger)
            {
                Collisions colliderBool = other.gameObject.GetComponent<Collisions>();

                if (correctWeapon == other.gameObject)
                {
                    caseGen.UpdateCaseWeapon(null);

                    colliderBool.inOtherCollider[0] = false;

                    if (!colliderBool.InAnotherCollider())
                    {
                        ManageLocations(other.transform, false);
                        GameSceneManager.instance.ManageStrings(other.transform);
                        //redString.enabled = false;
                    }

                }

                tempWeapon = null;

                colliderBool.inOtherCollider[0] = false;

                if (!colliderBool.InAnotherCollider())
                {
                    ManageLocations(other.transform, false);
                    GameSceneManager.instance.ManageStrings(other.transform);
                    //redString.enabled = false;
                }
                //redString.enabled = false;
            }
            else if (isLocationTrigger)
            {
                Collisions colliderBool = other.gameObject.GetComponent<Collisions>();

                if (correctLocation == other.gameObject)
                {
                    caseGen.UpdateCaseLocation(null);

                    colliderBool.inOtherCollider[1] = false;

                    if (!colliderBool.InAnotherCollider())
                    {
                        ManageLocations(other.transform, false);
                        GameSceneManager.instance.ManageStrings(other.transform);
                        //redString.enabled = false;
                    }
                }

                colliderBool.inOtherCollider[1] = false;

                if (!colliderBool.InAnotherCollider())
                {
                    ManageLocations(other.transform, false);
                    GameSceneManager.instance.ManageStrings(other.transform);
                    //redString.enabled = false;
                }
            }
            else
            {
                Collisions colliderBool = other.gameObject.GetComponent<Collisions>();

                if (correctWitness == other.gameObject)
                {
                    caseGen.UpdateCaseWitness(null);

                    colliderBool.inOtherCollider[2] = false;

                    if (!colliderBool.InAnotherCollider())
                    {
                        ManageLocations(other.transform, false);
                        GameSceneManager.instance.ManageStrings(other.transform);
                        //redString.enabled = false;
                    }
                }

                colliderBool.inOtherCollider[2] = false;

                if (!colliderBool.InAnotherCollider())
                {
                    ManageLocations(other.transform, false);
                    GameSceneManager.instance.ManageStrings(other.transform);
                    //redString.enabled = false;
                }
            }

            //redString.enabled = false;
        }
    }

    /*
    private void DrawRope(Collider other, Transform notesPos)
    {
        Vector3 a = notesPos.position;
        Vector3 b = other.gameObject.transform.position;

        if (redString.positionCount != segments + 1)
            redString.positionCount = segments + 1;

        float dist = Vector3.Distance(a, b);
        float effectiveDroop = Mathf.Min(droop, Mathf.Max(-2f, maxSagPerMeter * dist));

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;

            Vector3 p = Vector3.Lerp(a, b, t);

            float sag = Mathf.Sin(t * Mathf.PI) * effectiveDroop;
            p.y -= sag;

            float minY = 0f + ropeClearance;
            if (p.y < minY) p.y = minY;

            redString.SetPosition(i, p);
        }

        //redString.SetPosition(0, weaponNotes.position);
        //redString.SetPosition(1, other.gameObject.transform.position);
    }
    */

    private void ManageLocations(Transform location, bool adding)
    {
        bool addEntry = true;

        if (adding)
        {
            //Debug.Log("Adding");
            if (GameSceneManager.instance.stringLocations == null)
            {
                GameSceneManager.instance.stringLocations = new List<Transform>();
                GameSceneManager.instance.stringLocations.Add(location);
                GameSceneManager.instance.connectionCount = new List<int>();
                GameSceneManager.instance.connectionCount.Add(0);
            }
            else
            {
                foreach (Transform t in GameSceneManager.instance.stringLocations)
                {
                    if (location == t)
                    {
                        addEntry = false;
                        //Debug.Log("Match");
                        //stringLocations.Add(other.transform);
                    }
                    else
                    {
                        //Debug.Log("Not a Match");
                    }
                }

                if (addEntry)
                {
                    GameSceneManager.instance.stringLocations.Add(location);
                    GameSceneManager.instance.connectionCount.Add(0);
                }
            }
            
        }
        else if (!adding)
        {
            //Debug.Log("Not Adding");

            int counter = 0;
            //Debug.Log("Removing Start Start");
            foreach (Transform t in GameSceneManager.instance.stringLocations)
            {
                //Debug.Log("Removing Start");
                if (t == location)
                {
                    //Debug.Log("Removing");
                    if (GameSceneManager.instance.stringLocations.Count == 1)
                    {
                        GameSceneManager.instance.stringLocations = null;
                        GameSceneManager.instance.connectionCount = null;
                    }
                    else
                    {
                        GameSceneManager.instance.stringLocations.Remove(t);
                        int connectionToRemove = GameSceneManager.instance.connectionCount[counter];
                        GameSceneManager.instance.connectionCount.Remove(connectionToRemove);
                        break;
                    }
                }

                counter++;
            }
        }
    }

}
