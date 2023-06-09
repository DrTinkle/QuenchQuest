using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct TastePreference
{
    public float MinSweetness;
    public float MaxSweetness;
    public float MinTartness;
    public float MaxTartness;
    public float TartnessTolerance;
    public float SweetnessTolerance;

    public TastePreference(
        float minSweetness, 
        float maxSweetness, 
        float minTartness, 
        float maxTartness,
        float tartnessTolerance,
        float sweetnessTolerance)
    {
        MinSweetness = minSweetness;
        MaxSweetness = maxSweetness;
        MinTartness = minTartness;
        MaxTartness = maxTartness;
        TartnessTolerance = tartnessTolerance;
        SweetnessTolerance = sweetnessTolerance;
    }
}

public class Customer : MonoBehaviour
{
    [SerializeField] GameData gameData;
    [SerializeField] GameObject floatingTipText;
    Animator animator;

    [SerializeField] float targetPositionMargin = 0.05f;
    [SerializeField] float checkpointPositionMargin = 0.5f;
    [SerializeField] float areaWealth = 10;
    [SerializeField] private float animatorSpeedChangeRate = 5f;

    [Header("Customer Stats")]
    [SerializeField] int thirst;
    [SerializeField] float wealth;
    [SerializeField] int moneyToSpend;
    [SerializeField] int age;
    [SerializeField] float walkingSpeed;
    [SerializeField] float thinkingTime;
    [SerializeField] float maxSweetnessTaste = 0.1f;
    [SerializeField] float maxTartnessTaste = 0.1f;
    [SerializeField] float maxSweetnessTolerance;
    [SerializeField] float maxTartnessTolerance;

   

    public TastePreference tastePreference;
    public int queueIndex;

    const int minAge = 5;
    const int maxAge = 100;
    const float minSpeed = 0.2f;
    const float maxSpeed = 2.0f;    
    const float minTime = 0.2f;
    const float maxTime = 3.0f;

    LemonadeStand lemonadeStand;
    QueueManager queueManager;
    SoundEffects soundEffects;
    CustomerSpawner customerSpawner;
    UIHandler ui;

    NavMeshAgent navMeshAgent;

    Vector3 targetPosition;
    Vector3 despawnPosition;
    bool despawnSet;

    float willingnessToBuy;
    bool willBuy;

    bool enteredQueue;

    private float currentAnimatorSpeed = 0f;

    public CustomerState State { get; set; }

    public enum CustomerState
    {
        MovingToCheckpoint,
        AtCheckpoint,
        InQueue,
        Exiting
    }

    void Awake()
    {
        lemonadeStand = FindObjectOfType<LemonadeStand>();
        queueManager = FindObjectOfType<QueueManager>();
        soundEffects = FindObjectOfType<SoundEffects>();
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        ui = FindObjectOfType<UIHandler>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        floatingTipText.SetActive(false);

        served = false;
        enteredQueue = false;

        RandomizeStats();
        CreateTasteProfile();

        State = CustomerState.MovingToCheckpoint;
    }


    void Update()
    {
        CustomerStateSwitch();

        if (navMeshAgent.velocity.magnitude > 0.2f)
        {
            animator.SetBool("isWalking", true);
            float targetAnimatorSpeed = navMeshAgent.velocity.magnitude * walkingSpeed;
            currentAnimatorSpeed = Mathf.Lerp(
                currentAnimatorSpeed, targetAnimatorSpeed, animatorSpeedChangeRate * Time.deltaTime);
            animator.speed = currentAnimatorSpeed;
        }
        else
        {
            animator.SetBool("isWalking", false);
            currentAnimatorSpeed = Mathf.Lerp(
                currentAnimatorSpeed, 1f, animatorSpeedChangeRate * Time.deltaTime);
            animator.speed = currentAnimatorSpeed;
        }

    }

    void CustomerStateSwitch()
    {
        switch (State)
        {
            case CustomerState.MovingToCheckpoint:
                MoveToCheckpoint();
                break;

            case CustomerState.AtCheckpoint:
                break;

            case CustomerState.InQueue:
                MoveThroughQueue();
                break;
            case CustomerState.Exiting:
                Exit();
                break;

        }
    }

    public void MoveToCheckpoint()
    {
        if (State != CustomerState.AtCheckpoint)
        {
            targetPosition = lemonadeStand.checkpoint.position;
            navMeshAgent.SetDestination(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) <= checkpointPositionMargin)
            {
                if (State != CustomerState.AtCheckpoint)
                {
                    State = CustomerState.AtCheckpoint;
                    StartCoroutine(CheckPointDecision());
                }
            }
        }
    }

    IEnumerator CheckPointDecision() //tässä asiakas päättää meneekö jonoon vai ei ja käyttää aikaa päättämiseen
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(thinkingTime);
        navMeshAgent.isStopped = false;

        if (WillBuyCheck() && queueManager.customersInQueue.Count < queueManager.QueueSpots.Count)
        {
            queueManager.JoinQueue(this);
            State = CustomerState.InQueue;
        }
        else if(queueManager.customersInQueue.Count >= queueManager.QueueSpots.Count + 1)
        {
            State = CustomerState.Exiting;
        }        
        else
        {
            State = CustomerState.Exiting;
        }

        yield break;
    }

    public bool WillBuyCheck() //tarkistaa tykkääkö hinnasta josta vähennetään popularity
    {
        return willingnessToBuy >= lemonadeStand.lemonadePrice - gameData.playerPopularity;
    }

    public void MoveThroughQueue()
    {
        if (!enteredQueue)
        {
            queueIndex = queueManager.customersInQueue.Count - 2;
            enteredQueue = true;
        }

        if (queueIndex > -1)
        {
            targetPosition = queueManager.QueueSpots[queueIndex].position;
            navMeshAgent.SetDestination(targetPosition);
        }
        else
        {
            targetPosition = queueManager.LemonadeServingSpot.position;
            navMeshAgent.SetDestination(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) <= targetPositionMargin)
            {
                if (WillBuyCheck()) //jos tykkää hinnasta, menee tarjoiltavaksi
                {
                    GetServed();
                }
                else //jos ei, lähtee menee ja poistuu jonosta
                {
                    queueManager.LeaveQueue(this);
                    State = CustomerState.Exiting;
                }
            }
        }
    }

    public void GetServed()
    {
        if (WillBuyCheck()) //jos edelleen tykkää hinnasta, tarjoillaan
        {
            lemonadeStand.SetCurrentCustomer(this);
        }
        else //jos ei, lähtee menee ja poistuu jonosta
        {
            queueManager.LeaveQueue(this);
            State = CustomerState.Exiting;
        }
    }

    public bool IsTastePreferred(float sweetness, float tartness)
    {
        return sweetness >= tastePreference.MinSweetness &&
               sweetness <= tastePreference.MaxSweetness &&
               tartness >= tastePreference.MinTartness &&
               tartness <= tastePreference.MaxTartness;
    }

    public bool IsTasteBad(float sweetness, float tartness)
    {
        if (sweetness == 0 || tartness == 0)
        {
            return true;
        }

        return sweetness >= tastePreference.MaxSweetness + tastePreference.SweetnessTolerance &&
               tartness >= tastePreference.MaxTartness + tastePreference.TartnessTolerance;
    }


    public int GiveTip()
    {
        int randomTipPercent = UnityEngine.Random.Range(1, 51);

        int tip = (int)((lemonadeStand.lemonadePrice * randomTipPercent) / 100f);

        if (floatingTipText)
        {
            StartCoroutine(TipPopUp(tip));
            soundEffects.PlayClip(soundEffects.tipSound);
        }

        return tip;
    }

   public void BadTaste()
    {
        StartCoroutine(LosePopularityPopUp());
        soundEffects.PlayClip(soundEffects.ewSound);
    }

    IEnumerator TipPopUp(int tip)
    {
        lemonadeStand.popularityIncreaseAnim.SetActive(true);
        lemonadeStand.popularityIncreaseAnim.GetComponent<Animator>().Play("StarAnimation");
        floatingTipText.SetActive(true);
        floatingTipText.GetComponentInChildren<TextMesh>().text = "$" + (tip / 100f).ToString(
            "F2", CultureInfo.InvariantCulture) + " TIP";
        floatingTipText.GetComponentInChildren<Animator>().Play("Tip Animation");
        yield return new WaitForSeconds(1);
        lemonadeStand.popularityIncreaseAnim.SetActive(false);
        floatingTipText.SetActive(false);
    }

    IEnumerator LosePopularityPopUp()
    {
        lemonadeStand.popularityDecreaseAnim.SetActive(true);
        lemonadeStand.popularityDecreaseAnim.GetComponent<Animator>().Play("StarSplitAnimation");
        yield return new WaitForSeconds(1);
        lemonadeStand.popularityDecreaseAnim.SetActive(false);
    }

    public void Exit()
    {
        if (!despawnSet)
        {
            despawnPosition = customerSpawner.GetDespawnLocation();
            despawnSet = true;
        }

        navMeshAgent.SetDestination(despawnPosition);

        if (Vector3.Distance(transform.position, despawnPosition) <= targetPositionMargin)
        {
            customerSpawner.DespawnCustomer(this);
        }
    }

    void RandomizeStats()
    {
        thirst = UnityEngine.Random.Range(1, 11);
        int youngerPeople = UnityEngine.Random.Range(0, 20);
        age = UnityEngine.Random.Range(minAge, maxAge - youngerPeople + 1);

        wealth = age / UnityEngine.Random.Range(0.0f, 100.0f) * areaWealth;
        moneyToSpend = (int)(wealth * UnityEngine.Random.Range(0.01f, 1.0f)) * 100;
        walkingSpeed = MapValue(age, minAge, maxAge, maxSpeed, minSpeed);
        thinkingTime = MapValue(age, minAge, maxAge, minTime, maxTime);

        willingnessToBuy = moneyToSpend * thirst / 100;

        navMeshAgent.speed = walkingSpeed;
    }

    float MapValue(float value, float minValue1, float maxValue1, float minValue2, float maxValue2)
    {
        value = Mathf.Clamp(value, minValue1, maxValue1);

        float normalizedPosition = (value - minValue1) / (maxValue1 - minValue1);

        float interpolatedValue = Mathf.Lerp(minValue2, maxValue2, normalizedPosition);

        return interpolatedValue;
    }

    private void CreateTasteProfile()
    {
        float sweetnessTaste = UnityEngine.Random.Range(0, maxSweetnessTaste);
        float tartnessTaste = UnityEngine.Random.Range(0, maxTartnessTaste);

        float minSweetness = 1;
        float maxSweetness = 1 + sweetnessTaste;
        float minTartness = 1;
        float maxTartness = 1 + tartnessTaste;
        maxSweetnessTolerance = ui.maxPossibleSweetness - maxSweetnessTaste;
        maxTartnessTolerance = ui.maxPossibleTartness - maxTartnessTaste;

        float sweetnessTolerance = UnityEngine.Random.Range(0, maxSweetnessTolerance);
        float tartnessTolerance = UnityEngine.Random.Range(0, maxTartnessTolerance);

        this.tastePreference = new TastePreference(
            minSweetness,
            maxSweetness,
            minTartness,
            maxTartness,
            sweetnessTolerance,
            tartnessTolerance);
    }
}
