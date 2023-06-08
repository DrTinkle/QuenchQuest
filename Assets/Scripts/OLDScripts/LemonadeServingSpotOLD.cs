//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LemonadeServingSpot : MonoBehaviour
//{
//    [SerializeField] float serveTime = 1.00f;

//    public LemonadeStand lemonadeStand;
//    public Customer customerBeingServed;
//    public CustomerSpawner customerSpawner;

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        customerBeingServed = other.GetComponent<Customer>();

//        if (customerBeingServed != null)
//        {
//            lemonadeStand.currentCustomer = customerBeingServed;
//            StartCoroutine(ServeCustomer());
//        }

//    }

//    //void Update()
//    //{
//    //    if (Input.GetKeyDown(KeyCode.Space))
//    //    {
//    //        Debug.Log("customerBeingServed Instance ID: " + customerBeingServed.GetInstanceID());
//    //        lemonadeStand.ServeCurrentCustomer();
//    //    }

//    //    if (Input.GetKeyDown(KeyCode.P))
//    //    {
//    //        customerBeingServed = null;
//    //    }
//    //}

//    IEnumerator ServeCustomer()
//    {
//        Debug.Log("ServeCustomer() called in LemonadeServingSpot");
//        yield return new WaitForSeconds(serveTime);
//        lemonadeStand.ServeCurrentCustomer();
//        Debug.Log("Serving Customer with ID " + customerBeingServed.GetInstanceID() + ".");
//        customerBeingServed = null;
//        yield break;
//    }
//}
