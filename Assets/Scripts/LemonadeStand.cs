using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class LemonadeStand : MonoBehaviour
{
    [SerializeField] GameData gameData;

    [SerializeField] QueueManager queueManager;
    [SerializeField] UIHandler ui;
    [SerializeField] float serveTime = 4.0f;
    [SerializeField] float lemonadeMakingDelay = 1;
    public GameObject popularityIncreaseAnim;
    public GameObject popularityDecreaseAnim;
    
    public Transform checkpoint;
    public Customer currentCustomer;

    public int lemonadePrice = 100;
    public int pitcherSize = 10;
    public int leftInPitcher;

    SoundEffects soundEffects;

    public bool makingLemonade = false;

    float sweetness;
    float tartness;

    private void Awake()
    {
        soundEffects = FindObjectOfType<SoundEffects>();
    }

    private void Start()
    {
        leftInPitcher = 0;
        popularityIncreaseAnim.SetActive(false);
        popularityDecreaseAnim.SetActive(false);
    }

    public void MakeLemonade()
    {

        if (gameData.lemonsInventory >= ui.lemonsUnits && 
                gameData.sugarInventory >= ui.sugarUnits)
        {
            StartCoroutine(MakingLemonade());
        }
    }

    IEnumerator MakingLemonade()
    {
        if (!makingLemonade)
        {
            ui.ShowMakeLemonade(false);
            makingLemonade = true;
        }

        (sweetness, tartness) = ui.GetLemonadeTaste();
        ui.ReduceRecipeUnits();

        while (leftInPitcher < pitcherSize)
        {
            yield return new WaitForSeconds(lemonadeMakingDelay);
            leftInPitcher++;
            ui.pitcherUI.text = "Pitcher: " + leftInPitcher + " / " + pitcherSize;
        }

        makingLemonade = false;

        if (leftInPitcher == pitcherSize && currentCustomer != null)
        {
            StartCoroutine(ServeCustomer());
        }
    }

    public void SetCurrentCustomer(Customer customer)
    {
        if (currentCustomer != null)
        {
            return;
        }

        currentCustomer = customer;

        if (leftInPitcher > 0 && !makingLemonade)
        {
            StartCoroutine(ServeCustomer());
        }
        else
        {
            Debug.Log("The Lemonade Pitcher is empty.");
        }
    }

    IEnumerator ServeCustomer()
    {
        StartCoroutine(UpdateSliderOverTime(serveTime));

        yield return new WaitForSeconds(serveTime);
        leftInPitcher--;
        ui.pitcherUI.text = "Pitcher: " + leftInPitcher + " / " + pitcherSize;

        int tip;

        if (currentCustomer.IsTastePreferred(sweetness, tartness))
        {
            tip = currentCustomer.GiveTip();
            gameData.playerPopularity++;
            ui.popularityAmountText.text = gameData.playerPopularity.ToString();
        }
        else if (currentCustomer.IsTasteBad(sweetness, tartness))
        {
            tip = 0;
            gameData.playerPopularity--;
            currentCustomer.BadTaste();
            ui.popularityAmountText.text = gameData.playerPopularity.ToString();
        }
        else
        {
            tip = 0;
            
        }

        soundEffects.PlayClip(soundEffects.saleSound);
        ui.Sale(lemonadePrice, tip);
        currentCustomer.State = Customer.CustomerState.Exiting;
        queueManager.LeaveQueue(currentCustomer);
        currentCustomer = null;

        if (leftInPitcher < 1)
        {
            ui.ShowMakeLemonade(true);
        }

        yield break;
    }

    public void PriceUp()
    {
        lemonadePrice += 10;
        ui.lemonadePrice.text = "$" + (lemonadePrice / 100f).ToString("F2", CultureInfo.InvariantCulture);
    }
    
    public void PriceDown()
    {
        if (lemonadePrice > 0)
        {
            lemonadePrice -= 10;
            ui.lemonadePrice.text = "$" + (lemonadePrice / 100f).ToString("F2", CultureInfo.InvariantCulture);
        }
    }

    IEnumerator UpdateSliderOverTime(float duration)
    {
        float elapsed = 0;

        ui.progressSlider.value = 0;
        ui.serveTimeSliderObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ui.progressSlider.value = elapsed / duration;
            yield return null;
        }

        ui.progressSlider.value = 1;
        ui.serveTimeSliderObject.SetActive(false);
    }
}
