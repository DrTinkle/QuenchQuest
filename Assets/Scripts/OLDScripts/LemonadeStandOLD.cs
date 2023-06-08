//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LemonadeStand : MonoBehaviour
//{
//    public Customer currentCustomer;

//    public void SetCurrentCustomer(Customer customer)
//    {
//        if (currentCustomer != null)
//        {
//            currentCustomer.OnServed.RemoveListener(CustomerServed);
//        }

//        currentCustomer = customer;

//        if (currentCustomer != null)
//        {
//            currentCustomer.OnServed.AddListener(CustomerServed);
//        }
//    }

//    void CustomerServed(Customer customer)
//    {
//        currentCustomer = null;
//    }

//    public void ServeCurrentCustomer()
//    {
//        if (currentCustomer != null)
//        {
//            currentCustomer.Serve();
//        }
//    }
//}
