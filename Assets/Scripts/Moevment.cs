using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Moevment : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 5f;
    public Transform orientation;
    public Vector3 movementDirection;

    public float GroundDrag = 0.2f;
    
    [Header("Jump")]
    public Transform Legs;
    public float radius = 0.2f;
    public LayerMask groundMask;
    public float jump = 25f;
    public float airResistance = 0.2f;
    
    private Rigidbody rb;
    private float horizontal;
    private bool isGrounded;
    private RaycastHit hit;
    private Vector2 input;
    private float x;
    private float z;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
      
    }
    void FixedUpdate()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        isGrounded = Physics.Raycast(Legs.position, Legs.TransformDirection(Vector3.down), out hit, radius, groundMask);
        Move();
        SpeedControl();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void Move()
    {
       
        movementDirection = orientation.forward * z + orientation.right * x;
        if (isGrounded)
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        if (!isGrounded)
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * 10f * airResistance, ForceMode.Force);
        }

        if (isGrounded)
        {
            rb.drag = GroundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
        
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.AddForce(transform.up * jump, ForceMode.Impulse);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    /*private void OnDrawGizmos()
    {
        if(Legs !=  null)
        {
            Gizmos.DrawLine(Legs.position, Legs.position + Legs.TransformDirection(Vector3.down) * radius);
        }

    }*/
}
