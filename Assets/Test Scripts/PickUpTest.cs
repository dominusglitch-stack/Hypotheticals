using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using TMPro;

public class PickUpTest : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickupForce = 150f;
    public float pickUpRange = 5f;

    private float rotationSensitivity = 1f;
    public GameObject heldObj;
    private Rigidbody heldObjRb;
    private Collisions heldObjCollisions;
    private bool canDrop = true;
    private int LayerNumber;
    public CinemachineInputAxisController playerCamera;
    [SerializeField] protected float placementOffset = 0.1f;

    public AudioSource objectAudio;
    public AudioClip[] throwSFX;
    public AudioClip pickupSFX;

    public CaseGenerator caseGen;
    public GameObject grabbedText;

    InputAction rotateAction;
    InputAction throwAction;
    InputAction interactAction;
    InputAction lookAction;

    private void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        throwAction = InputSystem.actions.FindAction("Attack");
        interactAction = InputSystem.actions.FindAction("Interact");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    private void Update()
    {
        if (interactAction.WasPressedThisFrame())
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickup")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                    else if (hit.transform.gameObject.tag == "Box")
                    {
                        caseGen.CheckCase();
                    }
                    else if (hit.transform.gameObject.tag == "Quit")
                    {
                        caseGen.GiveUp(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    //StopClipping();
                    DropObject();
                }
            }
        }
        if (heldObj != null)
        {
            heldObjRb.isKinematic = false;
            RotateObject();
            if (throwAction.WasPressedThisFrame() && canDrop == true)
            {
                //StopClipping();
                ThrowObject();
            }
        }
    }

    private void FixedUpdate()
    {
        if (heldObj != null)
        {
            MoveObject();
        }
    }

    private void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjCollisions = pickUpObj.GetComponent<Collisions>();
            //heldObjCollisions.PickedUp();
            heldObjCollisions.isPickedUp = true;
            heldObjRb.isKinematic = false;
            heldObjRb.useGravity = false;
            heldObjRb.linearDamping = 10;
            heldObjRb.mass = 1;
            //heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;
            heldObjRb.freezeRotation = false;
            heldObjRb.constraints = RigidbodyConstraints.None;
            objectAudio = pickUpObj.GetComponent<AudioSource>();
            objectAudio.PlayOneShot(pickupSFX);
            //heldObjRb.constraints = RigidbodyConstraints.FreezeRotation;

            grabbedText.SetActive(true);

            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    private void DropObject()
    {
        heldObjCollisions.isPickedUp = false;
        heldObjCollisions.StartThrowTimer();
        //Debug.Log("IspickedUp = false");
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.useGravity = true;
        heldObjRb.linearDamping = 1;
        heldObj.transform.parent = null;
        heldObjRb.constraints = RigidbodyConstraints.None;
        objectAudio.PlayOneShot(throwSFX[Random.Range(0,5)]);
        objectAudio = null;
        heldObj = null;

        grabbedText.SetActive(false);
    }

    private void MoveObject()
    {
        //heldObj.transform.position = holdPos.transform.position;

        if (Vector3.Distance(heldObj.transform.position, holdPos.transform.position) > 0.1f)
        {
            Vector3 direction = (holdPos.transform.position - heldObj.transform.position);
            heldObjRb.AddForce(direction * pickupForce);
        }

        if (Vector3.Distance(heldObj.transform.position, holdPos.transform.position) > 4f)
        {
            DropObject();
        }

        //heldObjRb.MovePosition(holdPos.transform.position * (Time.fixedDeltaTime * 2));
    }

    private void RotateObject()
    {
        if (rotateAction.IsPressed())
        {
            canDrop = false;

            playerCamera.enabled = false;
            heldObjRb.constraints = RigidbodyConstraints.FreezeRotation;

            Vector2 lookInput = lookAction.ReadValue<Vector2>();
            heldObj.transform.Rotate(Vector3.down, lookInput.x * rotationSensitivity);
            heldObj.transform.Rotate(Vector3.right, lookInput.y * rotationSensitivity);
        }
        else
        {
            playerCamera.enabled = true;
            heldObjRb.constraints = RigidbodyConstraints.None;

            canDrop = true;
        }
    }

    private void ThrowObject()
    {
        heldObjCollisions.isPickedUp = false;
        heldObjCollisions.StartThrowTimer();
        //Debug.Log("IspickedUp = false");
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.useGravity = true;
        heldObjRb.linearDamping = 1;
        heldObj.transform.parent = null;
        heldObjRb.constraints = RigidbodyConstraints.None;
        heldObjRb.AddForce(transform.forward * throwForce);
        objectAudio.PlayOneShot(throwSFX[Random.Range(0, 5)]);
        objectAudio = null;
        heldObj = null;

        grabbedText.SetActive(false);
    }

    //private void StopClipping()
    //{
        //var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);

        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        //if (hits.Length > 1)
        //{
            //if (hits[0].collider.gameObject.tag == "Board")
            //{
                //heldObj.transform.position = hits[0].point + hits[0].normal * placementOffset;
            //}
            //else
                //heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        //}
    //}
}
