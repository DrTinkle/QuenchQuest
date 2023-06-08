using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance { get; set; }

    public List<Transform> QueueSpots;
    public List<Customer> customersInQueue = new();
    public Transform LemonadeServingSpot;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void JoinQueue(Customer customer)
    {
        customersInQueue.Add(customer);
    }

    public void LeaveQueue(Customer customer)
    {
        customersInQueue.Remove(customer);
        ShiftQueue();
    }

    void ShiftQueue()
    {
        foreach (Customer customer in customersInQueue)
        {
            customer.queueIndex--;
        }
    }
}

