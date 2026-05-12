using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    private float playerSpeed = 4.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public bool needJumop = false;
    public float stepOffset;
    public float skin;
    CollisionFlags flags;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            needJumop = true;
        }
        if (flags == CollisionFlags.Above)
        {
            if (playerVelocity.y != 0)
            {
                playerVelocity.y = 0;
            }
        }
    }
    void FixedUpdate()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        controller.Move(move * Time.deltaTime * playerSpeed);
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Makes the player jump
        if (needJumop)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            needJumop = false;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        flags = controller.Move(playerVelocity * Time.deltaTime);
    }
    public float pushPwoer;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) 
        {
            return;
        }
        else
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * pushPwoer;
        }
        
        
    }

    
}
