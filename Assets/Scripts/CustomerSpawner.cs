using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance { get; set; }

    [SerializeField] CustomerPool customerPool;
    [SerializeField] TimeManager timeManager;

    public List<Transform> spawnPoints;
    public AnimationCurve spawnRateOverDay;

    readonly List<Customer> currentCustomers = new();

    float nextSpawnTime;
    float startTimeDelay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Another instance of CustomerSpawner already exists!");
        }
    }

    void Start()
    {
        startTimeDelay = Time.time + 2.0f;
        nextSpawnTime = Time.time + GetSpawnDelay();
    }

    void Update()
    {
        RandomizeSpawn();
        GetCustomerList();
    }

    void RandomizeSpawn()
    {
        if (customerPool.CustomerCount() > 0 && Time.time >= nextSpawnTime && Time.time >= startTimeDelay)
        {
            SpawnCustomer();
        }
    }

    void SpawnCustomer()
    {
        nextSpawnTime = Time.time + GetSpawnDelay();
        Customer customer = customerPool.GetCustomer();

        if (customer == null)
        {
            Debug.LogError("No Customer's in pool!");
            return;
        }

        int i = UnityEngine.Random.Range(0, spawnPoints.Count);
        customer.transform.position = spawnPoints[i].position;

        currentCustomers.Add(customer);
    }

    float GetSpawnDelay()
    {
        float customerSpawnDelay = spawnRateOverDay.Evaluate(timeManager.time) * UnityEngine.Random.Range(1.0f, 5.0f);
        Debug.Log(customerSpawnDelay);
        return customerSpawnDelay;
        
    }

    void GetCustomerList()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Listing all customers in currentCustomers:");
            foreach (Customer c in currentCustomers)
            {
                Debug.Log("Customer " + c.name + " with Instance ID: " + c.GetInstanceID());
            }
        }
    }

    public Vector3 GetDespawnLocation()
    {
        int i = UnityEngine.Random.Range(0, spawnPoints.Count);
        return spawnPoints[i].position;
    }

    public void DespawnCustomer(Customer customer)
    {
        currentCustomers.Remove(customer);
        customerPool.ReturnCustomer(customer);
    }
}



