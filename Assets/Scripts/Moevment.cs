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
    public float distance = 5;
    private float desiredMoveSpeed;

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

    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpped = 1f;
    public LayerMask attackLayer;

    [Header("Animations")]
    public Animator animator;
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";
    

    string currentAnimationState;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    private Camera cam;
    private AudioSource audios;
    private bool attacking = false;
    private bool readyToAttack = true;
    private int attackCount;
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
    private float speed;
    [SerializeField]public bool sliding;
    
    void Start()
    {
        audios = GetComponent<AudioSource>();
        speed = movementSpeed;
        rb = GetComponent<Rigidbody>();
        realBuffer = jumpBuffer;
        desiredMoveSpeed = movementSpeed;
        cam = Camera.main;



    }

    void Update()
    {
        Jumping();
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

        if (isGrounded)
        {
            StopAllCoroutines();
            StartCoroutine(Smooth());
        }
        else
        {
            movementSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;

        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        SetAnimations();
        
    }

    private IEnumerator Smooth()
    {
        float time = 0;
        float difrence = Mathf.Abs(desiredMoveSpeed - movementSpeed);
        float startValue = speed;

        while (time < difrence)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difrence);
            if (OnSlope())
            {
                float angle = Vector3.Angle(Vector3.down, slopeHit.normal);
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

    private void Jumping()
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
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out slopeHit, distance))
        {
            angle = Vector3.Angle(Vector3.down,slopeHit.normal);
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

    void SetAnimations()
    {
        if(!attacking)
        {
            if(rb.velocity.x == 0 && rb.velocity.z == 0)
            {
                ChangeAnimationState(IDLE);
            }
            else
            {
                ChangeAnimationState(WALK);
            }
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if(currentAnimationState == newState) return;

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
        
    }

    void HitTarget(Vector3 pos)
    {
        audios.pitch = 1;
        audios.PlayOneShot(hitSound);

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO);
    }

    void AttckRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit.point);
        }
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    public void Attack()
    {
        if (!readyToAttack || attacking) return;
        
        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpped);
        Invoke(nameof(AttckRaycast), attackDelay);

        audios.pitch = Random.Range(0.9f, 1.1f);
        audios.PlayOneShot(swordSwing);

        if(attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.down) * distance);
    }*/ 
}
