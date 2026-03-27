using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    //[SerializeField] protected float _delayBeforeRespawn = 5f;
    //[SerializeField] protected float _despawnTime = 2.5f;
    //[SerializeField] protected float _respawnTime = 3f;
    [SerializeField] protected Transform _spawner;
    //[SerializeField] protected Material _fadeMaterial;
    //[SerializeField] protected Material _material;

    private Rigidbody rb;
    public bool outside;
    //private MeshRenderer meshRenderer;
    //private MeshRenderer[] meshRenderers = null;
    //public bool respawningObject = false;
    public bool isPickedUp = false;
    public bool[] inOtherCollider = new bool[3];
    //public bool isFloorObject = false;

    public AudioSource objectAudio;
    public AudioClip[] collisionSFX;
    public AudioClip respawnSFX;
    public bool hasStarted = true;
    public bool wasThrown = false;

    public float minSoundInterval = 0.10f;

    //Color fadeIn = Color.clear;
    //Color fadeOut = Color.grey;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectAudio = GetComponent<AudioSource>();

        minSoundInterval = 0.05f;
        /*
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            meshRenderers = new MeshRenderer[2];
            meshRenderers[0] = GameObject.Find("Witness 1").GetComponent<MeshRenderer>();
            meshRenderers[1] = GameObject.Find("Witness 2").GetComponent<MeshRenderer>();
            fadeOut = meshRenderers[0].material.color;
        }
        else
        {
            fadeOut = meshRenderer.material.color;
        }
        */

        _spawner = GameObject.Find("Spawner").transform;

        StartCoroutine(EardrumProtector());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasStarted)
        {
            objectAudio.clip = collisionSFX[Random.Range(0, 5)];

            if (isPickedUp)
            {
                if (!objectAudio.isPlaying || objectAudio.isPlaying && objectAudio.time > minSoundInterval)
                {
                    if (rb.constraints != RigidbodyConstraints.FreezeAll)
                    {
                        objectAudio.Play();
                    }
                }
            }
            else if (wasThrown)
            {
                //Debug.Log("Thrown");

                if (!objectAudio.isPlaying || objectAudio.isPlaying && objectAudio.time > minSoundInterval)
                {
                    //Debug.Log("Collide");
                    if (rb.constraints != RigidbodyConstraints.FreezeAll)
                    {
                        objectAudio.Play();
                    }
                }
            }
        }
    }

    IEnumerator EardrumProtector()
    {
        hasStarted = true;
        yield return new WaitForSeconds(0.5f);
        hasStarted = false;
    }

    public void StartThrowTimer()
    {
        StopCoroutine(ThrowTimer());
        wasThrown = true;
        StartCoroutine(ThrowTimer());
    }

    public IEnumerator ThrowTimer()
    {
        wasThrown = true;
        yield return new WaitForSeconds(3);
        wasThrown = false;
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(gameObject.ToString() + " Collides with " + collision.ToString());
        if (collision.collider.tag == "Board")
        {
            rb.isKinematic = true;
        }
        
        else if (collision.collider.tag == "Floor" && respawningObject == false && isPickedUp == false && isFloorObject == false)
        {
            StartCoroutine(RespawnObject());
            if (meshRenderer == null)
            {
                meshRenderers[0].material = _fadeMaterial;
                meshRenderers[1].material = _fadeMaterial;
            }
            else
            {
                meshRenderer.material = _fadeMaterial;
            }
            
            respawningObject = true;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.ToString() + " Triggered by " + other.ToString());
        if (other.gameObject.tag == "Outside")
        {
            if (!outside)
            {
                Debug.Log("Outside!");
                Invoke("RepositionObject", 2f);
                outside = true;
            }
        }
    }
    */

    private void RepositionObject()
    {
        transform.position = _spawner.position;
        objectAudio.PlayOneShot(respawnSFX);
        //play particle animation here
        outside = false;
    }

    /*
    public IEnumerator RespawnObject()
    {
        yield return new WaitForSeconds(_delayBeforeRespawn);

        float timer = 0;
        while (timer <= _despawnTime)
        {
            if (meshRenderer == null)
            {
                meshRenderers[0].material.color = Color.Lerp(fadeOut, fadeIn, timer / _despawnTime);
                meshRenderers[1].material.color = Color.Lerp(fadeOut, fadeIn, timer / _despawnTime);
            }
            else
            {
                meshRenderer.material.color = Color.Lerp(fadeOut, fadeIn, timer / _despawnTime);
            }

            timer += Time.deltaTime;

            yield return null;
        }

        if (meshRenderer == null)
        {
            meshRenderers[0].enabled = false;
            meshRenderers[1].enabled = false;
        }
        else
        {
            meshRenderer.enabled = false;
        }

        rb.isKinematic = true;
        transform.position = _spawner.position;

        yield return new WaitForSeconds(_respawnTime);

        if (meshRenderer == null)
        {
            meshRenderers[0].enabled = true;
            meshRenderers[1].enabled = true;
        }
        else
        {
            meshRenderer.enabled = true;
        }
        rb.isKinematic = false;

        timer = 0;
        while (timer <= _despawnTime)
        {
            if (meshRenderer == null)
            {
                meshRenderers[0].material.color = Color.Lerp(fadeIn, fadeOut, timer / _despawnTime);
                meshRenderers[1].material.color = Color.Lerp(fadeIn, fadeOut, timer / _despawnTime);
            }
            else
            {
                meshRenderer.material.color = Color.Lerp(fadeIn, fadeOut, timer / _despawnTime);
            }

            timer += Time.deltaTime;

            yield return null;
        }

        if (meshRenderer == null)
        {
            meshRenderers[0].material = _material;
            meshRenderers[1].material = _material;
        }
        else
        {
            meshRenderer.material = _material;
        }
        respawningObject = false;
    }
    
    public void PickedUp()
    {
        StopAllCoroutines();
        if (meshRenderer == null)
        {
            meshRenderers[0].material = _material;
            meshRenderers[1].material = _material;
        }
        else
        {
            meshRenderer.material = _material;
        }
        respawningObject = false;
        isPickedUp = true;
    }
    */

    public bool InAnotherCollider()
    {
        foreach (bool b in inOtherCollider)
        {
            if (b)
            {
                return true;
            }
        }

        return false;
    }
}
