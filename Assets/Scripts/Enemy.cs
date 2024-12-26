using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Player")] 
    public Transform player;
    public float distance = 10f;
    public Transform head;

    [Header("Movement")]
    public float speed = 4.5f;

    private Rigidbody rb;
    private RaycastHit hit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var isPlayerInRange = Physics.Raycast(head.position, head.forward, out hit, distance);
        if (isPlayerInRange)
        {
            transform.LookAt(player.position);
            rb.velocity = transform.forward * speed;
        }


    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(head.position, head.position + head.forward * distance);
    }

}
