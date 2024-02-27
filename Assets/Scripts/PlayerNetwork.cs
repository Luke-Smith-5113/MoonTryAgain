using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    public bool isGrounded;

    private float groundMax = 15f;
    private float airMax = 20f;
    private float jump = 5f;
    private float speed = 20f;

    private Rigidbody rb;
    private PlayerControls playerControls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        if (playerControls.Player.Move.IsInProgress())
        {
            Vector2 inputVector = playerControls.Player.Move.ReadValue<Vector2>();
            rb.AddRelativeForce(new Vector3(inputVector.x, 0, inputVector.y) * speed);
        }

        if (rb.velocity.magnitude > groundMax)
        {
            if (isGrounded) { rb.velocity = Vector3.ClampMagnitude(rb.velocity, groundMax); }
            else if (rb.velocity.magnitude > airMax) { rb.velocity = Vector3.ClampMagnitude(rb.velocity, airMax); }

        }

        if (playerControls.Player.Jump.IsInProgress())
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
