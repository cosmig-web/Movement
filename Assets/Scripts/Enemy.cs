using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Player")] 
    public Transform player;
    public float distance = 10f;
    public float radius = 10f;
    public Transform head;

    [Header("Movement")]
    public float speed = 4.5f;
    public Transform legs;
    public List<Vector3> path;
    public int currentPathIndex = 0;
    public bool loop = false;
    

    private Rigidbody rb;
    private RaycastHit hit;
    private int indexer;
    private Vector3 target;
    private bool repeate = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = path[currentPathIndex];
        
    }

    void Update()
    {
        var isPlayerInRange = Physics.OverlapSphere(head.position, radius, LayerMask.GetMask("Player"));
        var isGround = Physics.Raycast(legs.position, Vector3.down, out hit, distance, LayerMask.GetMask("Ground"));
        
        if (isPlayerInRange.Length != 0)
        {
            transform.LookAt(player.position);
            rb.position += transform.forward * speed * Time.deltaTime;
        }

        if (isPlayerInRange.Length <= 0)
        {
            if (Vector3.Distance(transform.position, target) < 0.3f)
            {
                if (!repeate)
                {
                    currentPathIndex++;
                    indexer++;
                }

                if (repeate)
                {
                    currentPathIndex--;
                    indexer++;
                }
                target = path[currentPathIndex];
                if (1+(indexer) >= path.Count)
                {
                    
                    indexer = 0;
                    if (loop)
                    {
                        repeate = true;
                    }
                    
                }

                if (currentPathIndex == 0)
                {
                    repeate = false;
                }
            }
            transform.LookAt(target);
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
            
        }
    }

    /*void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(head.position, head.TransformDirection(distance, distance , distance).normalized);
    }*/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }

}
