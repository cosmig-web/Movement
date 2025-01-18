using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Gunner : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform muzzle;
    public float fireRate = 1.25f;
    public Transform Player;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("Fire", 1f, fireRate);
    }


    void Update()
    {
        transform.LookAt(Player);
        rb.position = new Vector3(Player.position.x, rb.position.y, rb.position.z);
    }

    void Fire()
    {
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
    }

    
}
