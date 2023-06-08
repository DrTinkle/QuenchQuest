//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Events;

//public class Customer : MonoBehaviour
//{
//    [SerializeField] float despawnDistance = -10.0f;
//    [SerializeField] public float walkingSpeed = 1.0f;
//    [SerializeField] float targetPositionMargin = 0.05f;
//    int currentPathPointIndex = 0;

//    public Transform LemonadeServingPosition => PathManager.Instance.LemonadeServingSpot;
//    public Transform queueSpot;
//    public List<Transform> PathPoints => PathManager.Instance.PathPoints;
//    public Transform Checkpoint => PathManager.Instance.CheckPoint;

//    public bool isServed = false;
//    public bool isAtStand = false;

//    public CustomerSpawner customerSpawner;
//    CustomerPool customerPool;

//    Vector3 targetPosition;

//    public UnityEvent<Customer> OnServed;

//    bool debugged1;
//    bool debugged2;
//    bool debugged3;
//    bool debugged4;

//    public enum CustomerState
//    {
//        MovingThroughPath,
//        AtCheckpoint,
//        InQueue,
//        BeingServed,
//        Exiting
//    }

//    public CustomerState State { get; set; }

//    void OnEnable()
//    {
//        Debug.Log("Customer with ID " + GetInstanceID() + " spawned.");
//        customerPool = CustomerPool.Instance;
//        isServed = false;
//        isAtStand = false;
//        currentPathPointIndex = 0;
//        State = CustomerState.MovingThroughPath;
//        StartCoroutine(MoveThroughPath());

//        debugged1 = false;
//        debugged2 = false;
//        debugged3 = false;
//        debugged4 = false;
//    }

//    void Update()
//    {
//        CustomerStateSwitch();
//    }

//    void CustomerStateSwitch()
//    {
//        switch (State)
//        {
//            case CustomerState.InQueue:
//                if (isServed)
//                {
//                    Debug.Log("Customer with ID " + GetInstanceID() + " is served. Exiting.");
//                    State = CustomerState.Exiting;
//                }
//                else if (queueSpot == LemonadeServingPosition && !isAtStand)
//                {
//                    if (!debugged1)
//                    {
//                        Debug.Log("Customer with ID " + GetInstanceID() + " is next up.");
//                        debugged1 = true;
//                    }
//                    transform.position = Vector3.MoveTowards(transform.position, LemonadeServingPosition.position, walkingSpeed * Time.deltaTime);
//                    if (!debugged2)
//                    {
//                        Debug.Log("Customer with ID " + GetInstanceID() + " is moving to " + LemonadeServingPosition + ".");
//                        debugged2 = true;
//                    }
//                    if (Vector3.Distance(transform.position, LemonadeServingPosition.position) <= targetPositionMargin)
//                    {
//                        if (!debugged3)
//                        {
//                            Debug.Log("Customer with ID " + GetInstanceID() + " distance from " + LemonadeServingPosition + " is less than " + targetPositionMargin + " so is at stand.");
//                            debugged3 = true;
//                        }
//                        isAtStand = true;
//                    }
//                }
//                else
//                {
//                    if (!debugged4)
//                    {
//                        Debug.Log("Customer with ID " + GetInstanceID() + " is moving to queue spot " + queueSpot + ".");
//                        debugged4 = true;
//                    }
//                    transform.position = Vector3.MoveTowards(transform.position, queueSpot.position, walkingSpeed * Time.deltaTime);
//                }
//                break;
//            case CustomerState.Exiting:
//                transform.position = Vector3.MoveTowards(transform.position, new Vector3(despawnDistance, transform.position.y, transform.position.z), walkingSpeed * Time.deltaTime);
//                if (transform.position.x <= despawnDistance)
//                {
//                    Debug.Log("Customer with ID " + GetInstanceID() + " despawned.");
//                    CustomerPool.Instance.ReturnCustomer(this);
//                }
//                break;
//        }
//    }

//    public void Serve()
//    {
//        if (isServed) return;
//        Debug.Log("Customer with ID " + GetInstanceID() + " is being served.");
//        isServed = true;
//        isAtStand = false;
//        State = CustomerState.Exiting;
//        OnServed?.Invoke(this);
//    }

//    public IEnumerator MoveThroughPath()
//    {
//        Debug.Log("Customer with ID " + GetInstanceID() + " is moving through path.");
//        while (currentPathPointIndex < PathPoints.Count)
//        {
//            Vector3 targetPosition = PathPoints[currentPathPointIndex].position;

//            while (Vector3.Distance(transform.position, targetPosition) > targetPositionMargin)
//            {
//                transform.position = Vector3
//                    .MoveTowards(transform.position, targetPosition, walkingSpeed * Time.deltaTime);
//                yield return null;
//            }

//            currentPathPointIndex++;
//        }

//        Vector3 checkpointPosition = Checkpoint.position;
//        while (Vector3.Distance(transform.position, checkpointPosition) > targetPositionMargin)
//        {
//            transform.position = Vector3
//                .MoveTowards(transform.position, checkpointPosition, walkingSpeed * Time.deltaTime);
//            yield return null;
//        }

//        State = CustomerState.AtCheckpoint;
//        Debug.Log("Customer with ID " + GetInstanceID() + " is at checkpoint.");

//        AtCheckpointDecision();
//    }

//    void AtCheckpointDecision()
//    {
//        bool willJoinQueue = UnityEngine.Random.value > 0.1f;
//        if (willJoinQueue)
//        {
//            Debug.Log("Customer with ID " + GetInstanceID() + " decided to buy lemonade.");
//            State = CustomerState.InQueue;
//            Debug.Log("Customer with ID " + GetInstanceID() + " is joining queue.");
//            CustomerSpawner.Instance.JoinQueue(this);
//        }
//        else
//        {
//            State = CustomerState.Exiting;
//            Debug.Log("Customer with ID " + GetInstanceID() + " decided not to buy lemonade.");
//            Debug.Log("Customer with ID " + GetInstanceID() + " exiting.");
//        }
//    }

//    public void MoveToQueueSpot(Transform queueSpot)
//    {
//        Debug.Log("Customer with ID " + GetInstanceID() + " moving to queue spot " + queueSpot + ".");
//        this.queueSpot = queueSpot;
//        this.State = CustomerState.InQueue;
//        Debug.Log("Customer with ID " + GetInstanceID() + " waiting in queue.");
//    }
//}
