using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    // [SerializeField]private AnimationController animationController;

    private float speed = 5f;
    private float gravity = -9.81f;
    private float jumpHeight = 3f;

    [SerializeField] private float crouchingMultiplier;
    private float crouchinHeight = 1.25f;
    [SerializeField] private float standingHeight = 1.8f;

    private float groundDistance = 0.4f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;


    void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            // animationController.SetWalk(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        Walk();
        //Jump();       

        velocity.y += gravity * Time.deltaTime;
    }

    void Walk()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        /*
        if(move.magnitude > 0.1f)
        {
            animationController.SetWalk(true);
        }
        else
        {
            animationController.SetWalk(false);
        }         
            */

        if (Input.GetKey(KeyCode.LeftControl))
        {

            controller.height = crouchinHeight;
            speed *= crouchingMultiplier;
            move *= crouchingMultiplier;

        }
        else
        {
            speed = 12f;
            controller.height = standingHeight;
        }

    }


    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

}
