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
    public float slideSpeed = 5f;
    private float desiredMoveSpeed ;

    public float GroundDrag = 0.2f;
    
    [Header("Jump")]
    public Transform Legs;
    public float radius = 0.2f;
    public LayerMask groundMask;
    public float jump = 25f;
    public float airResistance = 0.2f;
    public float jumpBuffer = 0.2f;
    public float coyotiTime = 0.2f;
    
    [Header("Slope")]
    public float slopeAngle = 45f;
    public float speedIncreaseMultiplier = 2f;
    public float slopeIncreaseMultiplier = 2f;
    
    
    private RaycastHit slopeHit;
    private Rigidbody rb;
    private float horizontal;
    private bool isGrounded;
    private RaycastHit hit;
    private Vector2 input;
    private float x;
    private float z;
    private float realBuffer;
    private float coyotiBuffer;
    private float lastDesiredMoveSpeed;
    private float angle;
    [SerializeField]public bool sliding;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        realBuffer = jumpBuffer;
        desiredMoveSpeed = movementSpeed;
      
    }

    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        isGrounded = Physics.Raycast(Legs.position, Legs.TransformDirection(Vector3.down), out hit, radius, groundMask);
        if (isGrounded)
        {
            coyotiBuffer = coyotiTime;
        }
        else
        {
            coyotiBuffer -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            realBuffer = jumpBuffer;
        }
        else
        {
            realBuffer -= Time.deltaTime;
        }
        if (realBuffer > 0 && coyotiBuffer > 0)
        {
            realBuffer = 0;
            Jump();
        }

        if (sliding)
        {
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = movementSpeed;
            }
            
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && movementSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(Smooth());
        }
        else
        {
            movementSpeed = desiredMoveSpeed;
        }
        
        lastDesiredMoveSpeed = desiredMoveSpeed;
        
    }

    private IEnumerator Smooth()
    {
        float time = 0;
        float difrence = Mathf.Abs(desiredMoveSpeed - movementSpeed);
        float startValue = movementSpeed;

        while (time < difrence)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difrence);
            if (OnSlope())
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrese = 1 + (slopeAngle / 90);
                
                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrese;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            
            yield return null;
        }
        movementSpeed = desiredMoveSpeed;
        //print(difrence + " " + time );
    }
    void FixedUpdate()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        isGrounded = Physics.Raycast(Legs.position, Legs.TransformDirection(Vector3.down), out hit, radius, groundMask);
        Move();
        SpeedControl();

        
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

        if (OnSlope())
        {
            rb.AddForce(GetSlopeDirection(movementDirection) * movementSpeed * 10f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (isGrounded)
        {
            rb.drag = GroundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
        
        rb.useGravity = !OnSlope();
        
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

    public bool OnSlope()
    {
        
        if (Physics.Raycast(Legs.position, Legs.TransformDirection(Vector3.down), out slopeHit, radius))
        {
            angle = Vector3.Angle(Vector3.up,slopeHit.normal);
            if (angle < slopeAngle && angle != 0)
            {
                return true;
            }
        }
        return false; 
        
        
    }

    public Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    /*private void OnDrawGizmos()
    {
        if(Legs !=  null)
        {
            Gizmos.DrawLine(Legs.position, Legs.position + Legs.TransformDirection(Vector3.down) * radius);
        }

    }*/
}
