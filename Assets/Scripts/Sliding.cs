using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Sliding : MonoBehaviour
{
    [Header("References")] 
    public Transform orentation;
    public Transform Playerobj;
    
    
    [Header("Sliding")]
    public float slidingForce;
    public float YSliding;
   
    private float startYSliding;
    private float horizontal;
    private float vertical;
    private Rigidbody rb;
    private Moevment moevment;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moevment = GetComponent<Moevment>();
        startYSliding = Playerobj.localScale.y;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Slide") && (horizontal != 0 || vertical != 0) && !moevment.sliding)
        {
            startSlide();
        }
        if (Input.GetButtonUp("Slide") && moevment.sliding)
        {
            stopSlide();
        }
        //print(moevment.OnSlope());
    }

    void FixedUpdate()
    {
        if (moevment.OnSlope())
        {
            OnSlopeSlide();
        }
    }
    public void startSlide()
    {
        Vector3 slideDirection = orentation.forward * vertical + orentation.right * horizontal;
        moevment.sliding = true;
        
        Playerobj.localScale = new Vector3(Playerobj.localScale.x, YSliding, Playerobj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Force);
        rb.AddForce(slideDirection * slidingForce, ForceMode.Force);


    }

    public void OnSlopeSlide()
    {
        Vector3 slideDirection = orentation.forward * vertical + orentation.right * horizontal;
        rb.AddForce(moevment.GetSlopeDirection(slideDirection) * slidingForce, ForceMode.Force);
    }
    public void stopSlide()
    {
        moevment.sliding = false;
        
        Playerobj.localScale = new Vector3(Playerobj.localScale.x, startYSliding, Playerobj.localScale.z);
    }
}
