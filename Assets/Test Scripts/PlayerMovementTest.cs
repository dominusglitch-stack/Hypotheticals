using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour
{
    private float playerSpeed = 5.0f;
    private float speedMultiplier = 1f;
    //change jump back to 1.5f
    private float jumpHeight = 1.2f;
    private float gravityValue = -9.81f;

    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public bool outside;
    private Transform cameraTransform;
    public AudioSource playerAudio;
    public AudioClip jumpSFX;
    public AudioClip jumpLandSFX;
    public bool firstLand = false;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;
        }

        if (groundedPlayer && !firstLand)
        {
            playerAudio.PlayOneShot(jumpLandSFX);
            firstLand = true;
        }

        // Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        move = Vector3.ClampMagnitude(move, 1f);

        //if (move != Vector3.zero)
        //transform.forward = move;

        if (sprintAction.action.IsPressed())
            speedMultiplier = Mathf.Lerp(speedMultiplier, 2f, Time.deltaTime / 0.2f);
        else
            speedMultiplier = Mathf.Lerp(speedMultiplier, 1f, Time.deltaTime / 0.2f);

        // Jump using WasPressedThisFrame()
        if (groundedPlayer && jumpAction.action.WasPressedThisFrame())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            playerAudio.PlayOneShot(jumpSFX);
            firstLand = false;
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Move
        Vector3 finalMove = move * playerSpeed * speedMultiplier + Vector3.up * playerVelocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player collides with " + other.ToString());
        //Debug.Log("Colliding with " + other.ToString());

        if (other.gameObject.tag == "Outside")
        {
            if (!outside)
            {
                //Debug.Log("Outside!");
                outside = true;
                transform.position = new Vector3(0, 1, 0);
                outside = false;
            }
        }
        else if (other.gameObject.tag == "Roof")
        {
            //Debug.Log("Ouch");
            playerVelocity.y = 0;
        }
    }

    //add a function to change velocity when hitting head on roof
}
