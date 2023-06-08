//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CustomerSpawner : MonoBehaviour
//{
//    public static CustomerSpawner Instance { get; set; }
//    public List<Transform> queueSpots => PathManager.Instance.QueueSpots;
//    List<Customer> currentCustomers = new();

//    public float customerSpawnRate = 5f;
//    private float nextSpawnTime;

//    public CustomerPool customerPool;

//    public IReadOnlyList<Customer> CurrentCustomers => currentCustomers;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else if (Instance != this)
//        {
//            Debug.LogError("Another instance of CustomerSpawner already exists!");
//        }
//    }

//    void Start()
//    {
//        nextSpawnTime = Time.time;
//    }

//    void Update()
//    {
//        if (customerPool.CustomerCount() > 0 && Time.time >= nextSpawnTime)
//        {
//            SpawnCustomer();
//        }

//        if (Input.GetKeyDown(KeyCode.L))
//        {
//            Debug.Log("Listing all customers in currentCustomers:");
//            foreach (Customer c in currentCustomers)
//            {
//                Debug.Log("Customer " + c.name + " with Instance ID: " + c.GetInstanceID());
//            }
//        }
//    }

//    void SpawnCustomer()
//    {
//        Customer customer = customerPool.GetCustomer();

//        if (customer == null)
//        {
//            Debug.LogError("No Customer's in pool!");
//            return;
//        }

//        customer.transform.position = transform.position;
//        customer.OnServed.AddListener(CustomerServed);

//        currentCustomers.Add(customer);

//        nextSpawnTime = Time.time + customerSpawnRate;
//    }

//    public void CustomerServed(Customer customer)
//    {
//        customer.OnServed.RemoveListener(CustomerServed);

//        if (currentCustomers.Contains(customer))
//        {
//            currentCustomers.Remove(customer);
//            ShiftQueueForward(customer);
//        }

//        else
//        {
//            foreach (Customer c in currentCustomers)
//            {
//                Debug.Log("Customer " + c.name + " with Instance ID: " + c.GetInstanceID() + " is in the list");
//            }
//        }
//    }

//    public void JoinQueue(Customer customer)
//    {
//        Transform queueSpot = GetNextQueueSpot();
//        Debug.Log("Customer with ID " + customer.GetInstanceID() + " is assigned queue spot " + servedIndex + ".");
//        if (queueSpot != null)
//        {
//            Debug.Log("Queue spot not null");
//            customer.MoveToQueueSpot(queueSpot);
//            Debug.Log("Customer with ID " + customer.GetInstanceID() + " joins queue with queue spot " + servedIndex + ".");
//        }
//        else
//        {
//            customer.State = Customer.CustomerState.Exiting;
//            Debug.Log("Queue spots null. Customer with ID " + customer.GetInstanceID() + " exits.");
//        }
//    }

//    public Transform GetNextQueueSpot()
//    {
//        for (int i = 0; i < queueSpots.Count; i++)
//        {
//            if (queueSpots[i].childCount == 0)
//            {
//                return queueSpots[i];
//            }
//        }

//        return null;
//    }

//    int servedIndex;

//    public void ShiftQueueForward(Customer servedCustomer)
//    {
//        servedIndex = queueSpots.FindIndex(t => t == servedCustomer.queueSpot);

//        for (int i = servedIndex + 1; i < queueSpots.Count; i++)
//        {
//            Customer customer = currentCustomers.Find(c => c.queueSpot == queueSpots[i]);

//            if (customer != null && customer.State == Customer.CustomerState.InQueue)
//            {
//                customer.queueSpot = queueSpots[i - 1];
//                customer.MoveToQueueSpot(customer.queueSpot);
//            }
//        }
//    }
//}
