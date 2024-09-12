using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using System;
using System.Globalization;
using UnityEngine.Playables;
using Unity.VisualScripting;

public class BuyMenuLogic : MonoBehaviour
{
    [SerializeField] GameData gameData;
    [SerializeField] WeekdayHandler weekdayHandler;

    [SerializeField] TMPro.TextMeshProUGUI currentDayText;
    [SerializeField] TMPro.TextMeshProUGUI currentBankAmount;

    [Header("Inventory")]
    [SerializeField] TMPro.TextMeshProUGUI lemonsUnitsText;
    [SerializeField] TMPro.TextMeshProUGUI sugarUnitsText;

    [Header("Market")]
    [SerializeField] TMPro.TextMeshProUGUI lemonsBidAmountText;
    [SerializeField] TMPro.TMP_InputField lemonsInputField;
    [SerializeField] TMPro.TextMeshProUGUI lemonsAskAmountText;

    [SerializeField] TMPro.TextMeshProUGUI sugarBidAmountText;
    [SerializeField] TMPro.TMP_InputField sugarInputField;
    [SerializeField] TMPro.TextMeshProUGUI sugarAskAmountText;

    [Header("Expenses")]
    [SerializeField] TMPro.TextMeshProUGUI rentAmountText;
    [SerializeField] TMPro.TextMeshProUGUI waterBillAmountText;

    [Header("Buttons")]
    [SerializeField] Button startDayButton;
    [SerializeField] Button lemonsSellButton;
    [SerializeField] Button lemonsBuyButton;
    [SerializeField] Button sugarSellButton;
    [SerializeField] Button sugarBuyButton;
    [SerializeField] Button payRentButton;
    [SerializeField] Button payWaterBillButton;

    string currentDay;

    int startingBank = 4000;


    int lemonsBid;
    int lemonsAsk;

    int sugarBid;
    int sugarAsk;

    int rentAmount = 5000;

    int startingLemonsMarketPrice = 0;
    int currentLemonsMarketPrice;
    int startingSugarMarketPrice = 0;
    int currentSugarMarketPrice;

    //bool firstDay;

    void Start()
    {
        FirstDaySetup();

        RandomizeMarket();
    }

    void FirstDaySetup()
    {
        if (gameData.currentDayNumber < 1)
        {
            //firstDay = true;

            gameData.currentDayIndex = 0;
            gameData.playerMoney = startingBank;
            gameData.lemonsInventory = 0;
            gameData.sugarInventory = 0;
            gameData.rentDue = rentAmount;
            gameData.waterBillAmount = 0;
            gameData.lemonsMarketPrice = startingLemonsMarketPrice;
            gameData.sugarMarketPrice = startingSugarMarketPrice;
            gameData.playerPopularity = 0;
        }
        //else
        //{
        //    firstDay = false;
        //}
    }

    void RandomizeMarket()
    {
        currentLemonsMarketPrice = gameData.lemonsMarketPrice;

        float lemonsPriceChange = UnityEngine.Random.Range(-10f, 10f) / 100f + 1f;
        float lemonsMarketSpreadPercent = UnityEngine.Random.Range(2f, 5f) / 100f;

        float lemonsPriceCalculation = currentLemonsMarketPrice * lemonsPriceChange;
        float lemonsPriceSpread = lemonsPriceCalculation * lemonsMarketSpreadPercent;

        float lemonsPriceBid = lemonsPriceCalculation - lemonsPriceSpread;
        float lemonsPriceAsk = lemonsPriceCalculation + lemonsPriceSpread;

        lemonsBid = (int)Math.Round(lemonsPriceBid);
        lemonsAsk = (int)Math.Round(lemonsPriceAsk);

        if (lemonsBid < 0) { lemonsBid = 0; }

        gameData.lemonsMarketPrice = (int)Math.Round(lemonsPriceCalculation);

        currentSugarMarketPrice = gameData.sugarMarketPrice;

        float sugarPriceChange = UnityEngine.Random.Range(-10f, 10f) / 100f + 1f;
        float sugarMarketSpreadPercent = UnityEngine.Random.Range(2f, 5f) / 100f;

        float sugarPriceCalculation = currentSugarMarketPrice * sugarPriceChange;
        float sugarPriceSpread = sugarPriceCalculation * sugarMarketSpreadPercent;

        float sugarPriceBid = sugarPriceCalculation - sugarPriceSpread;
        float sugarPriceAsk = sugarPriceCalculation + sugarPriceSpread;

        sugarBid = (int)Math.Round(sugarPriceBid);
        sugarAsk = (int)Math.Round(sugarPriceAsk);

        if (sugarBid < 0) { sugarBid = 0; }

        gameData.sugarMarketPrice = (int)Math.Round(sugarPriceCalculation);

        InitializeDisplay();
    }

    void InitializeDisplay()
    {
        UpdateBankText();
        currentDay = weekdayHandler.GetWeekDay();

        currentDayText.text = currentDay;
        lemonsUnitsText.text = gameData.lemonsInventory.ToString();
        sugarUnitsText.text = gameData.sugarInventory.ToString();
        lemonsBidAmountText.text = "$" + (lemonsBid / 100f).ToString("F2", CultureInfo.InvariantCulture);
        lemonsAskAmountText.text = "$" + (lemonsAsk / 100f).ToString("F2", CultureInfo.InvariantCulture);
        sugarBidAmountText.text = "$" + (sugarBid / 100f).ToString("F2", CultureInfo.InvariantCulture);
        sugarAskAmountText.text = "$" + (sugarAsk / 100f).ToString("F2", CultureInfo.InvariantCulture);
        rentAmountText.text = "$" + (gameData.rentDue / 100f).ToString("F0", CultureInfo.InvariantCulture);
        waterBillAmountText.text = "$" + (gameData.waterBillAmount / 100f).ToString(
            "F2", CultureInfo.InvariantCulture);
    }

    void UpdateBankText()
    {
        currentBankAmount.text = "$" + (gameData.playerMoney / 100f).ToString(
            "F2", CultureInfo.InvariantCulture);
    }

    public void BuyLemons()
    {
        string input = lemonsInputField.text;
        int inputValue = int.Parse(input);

        int buyPrice = inputValue * lemonsAsk;

        if (gameData.playerMoney >= buyPrice)
        {
            gameData.playerMoney -= buyPrice;
            gameData.lemonsInventory += inputValue;
        }

        lemonsUnitsText.text = gameData.lemonsInventory.ToString();
        UpdateBankText();
    }

    public void SellLemons()
    {
        string input = lemonsInputField.text;
        int inputValue = int.Parse(input);

        int sellPrice = inputValue * lemonsBid;

        if (gameData.lemonsInventory >= inputValue)
        {
            gameData.playerMoney += sellPrice;
            gameData.lemonsInventory -= inputValue;
        }

        lemonsUnitsText.text = gameData.lemonsInventory.ToString();
        UpdateBankText();
    }

    public void BuySugar()
    {
        string input = sugarInputField.text;
        int inputValue = int.Parse(input);

        int buyPrice = inputValue * sugarAsk;

        if (gameData.playerMoney >= buyPrice)
        {
            gameData.playerMoney -= buyPrice;
            gameData.sugarInventory += inputValue;
        }

        sugarUnitsText.text = gameData.sugarInventory.ToString();
        UpdateBankText();
    }

    public void SellSugar()
    {
        string input = sugarInputField.text;
        int inputValue = int.Parse(input);

        int sellPrice = inputValue * sugarBid;

        if (gameData.sugarInventory >= inputValue)
        {
            gameData.playerMoney += sellPrice;
            gameData.sugarInventory -= inputValue;
        }

        sugarUnitsText.text = gameData.sugarInventory.ToString();
        UpdateBankText();
    }

    public void StartDay()
    {
        gameData.currentSceneIndex = 2;
        SceneHandler sceneHandler = FindObjectOfType<SceneHandler>();
        sceneHandler.LoadScene(gameData.currentSceneIndex);
    }

    public void PayRent()
    {
        if (gameData.playerMoney >= gameData.rentDue)
        {
            gameData.playerMoney -= gameData.rentDue;
            gameData.rentDue = 0;
        }

        rentAmountText.text = "$" + (gameData.rentDue / 100f).ToString("F0", CultureInfo.InvariantCulture);
        UpdateBankText();
    }

    public void PayWaterBill()
    {
        if (gameData.playerMoney >= gameData.waterBillAmount)
        {
            gameData.playerMoney -= gameData.waterBillAmount;
            gameData.waterBillAmount = 0;
        }

        waterBillAmountText.text = "$" + (gameData.waterBillAmount / 100f).ToString(
            "F2", CultureInfo.InvariantCulture);
        UpdateBankText();
    }
}
