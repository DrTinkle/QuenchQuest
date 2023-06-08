using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    public static CustomerPool Instance { get; set; }

    [SerializeField] CustomerSpawner customerSpawner;
    [SerializeField] int poolSize = 5;

    public GameObject customerPrefab;

    Queue<Customer> customerPool;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Another instance of CustomerPool already exists!");
        }
    }

    void Start()
    {
        customerPool = new Queue<Customer>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject customer = Instantiate(customerPrefab, transform);
            customer.SetActive(false);
            customerPool.Enqueue(customer.GetComponent<Customer>());
        }
    }

    public Customer GetCustomer()
    {
        if (customerPool.Count > 0)
        {
            Customer customer = customerPool.Dequeue();

            int i = UnityEngine.Random.Range(0, customerSpawner.spawnPoints.Count);
            customer.transform.position = customerSpawner.spawnPoints[i].position;

            customer.gameObject.SetActive(true);

            return customer;
        }
        else
        {
            //GameObject customer = Instantiate(customerPrefab);
            //return customer;
            return null;
        }
    }

    public void ReturnCustomer(Customer customer)
    {
        customer.gameObject.SetActive(false);
        customerPool.Enqueue(customer);
    }

    public int CustomerCount()
    {
        return customerPool.Count;
    }
}
