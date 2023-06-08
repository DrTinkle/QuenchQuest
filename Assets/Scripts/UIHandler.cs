using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using System;
using System.Globalization;

public class UIHandler : MonoBehaviour
{
    [SerializeField] GameData gameData;
    [SerializeField] SceneHandler sceneHandler;

    [Header("Text")]
    [SerializeField] TMPro.TextMeshProUGUI unitsSoldUI;
    [SerializeField] TMPro.TextMeshProUGUI cashUI;
    public TMPro.TextMeshProUGUI pitcherUI;
    public TMPro.TextMeshProUGUI lemonadePrice;
    public TMPro.TextMeshProUGUI lemonsInventoryUnits;
    public TMPro.TextMeshProUGUI sugarInventoryUnits;
    [SerializeField] TMPro.TextMeshProUGUI lemonsRecipeUnits;
    [SerializeField] TMPro.TextMeshProUGUI sugarRecipeUnits;
    [SerializeField] TMPro.TextMeshProUGUI waterRecipeUnits;

    [Header("Sliders")]
    [SerializeField] GameObject makeLemonadeWindow;
    [SerializeField] List<Slider> recipeSliders;
    [SerializeField] Slider sugarSlider;
    [SerializeField] Slider waterSlider;
    [SerializeField] Slider lemonsSlider;
    public GameObject serveTimeSliderObject;

    [Header("Buttons")]
    [SerializeField] GameObject lemonadeButton;

    [Header("Time")]
    [SerializeField] TimeManager timeManager;
    [SerializeField] TMPro.TextMeshProUGUI timeText;
    [SerializeField] GameObject endDayPanel;
    [SerializeField] Button endDayButton;

    [Header("Popularity")]
    public TMPro.TextMeshProUGUI popularityAmountText;

    public Slider progressSlider;
    [SerializeField] int waterPrice = 2;

    Slider activeSlider;

    public int sugarUnits;
    public int lemonsUnits;
    int waterUnits;
    public float maxPossibleSweetness;
    public float maxPossibleTartness;

    LemonadeStand lemonadeStand;

    bool isUpdating = false;

    int unitsSold = 0;

    void Awake()
    {
        lemonadeStand = FindObjectOfType<LemonadeStand>();
        progressSlider = serveTimeSliderObject.GetComponent<Slider>();
    }

    void Start()
    {
        SlidersStart();
        InitializeUI();
    }

    void Update()
    {
        Clock();

        if (Input.GetKeyDown(KeyCode.S))
        {
            EndDay();
        }
    }

    void InitializeUI()
    {
        ShowMakeLemonade(true);
        serveTimeSliderObject.SetActive(false);

        unitsSoldUI.text = "Units Sold: " + 0.ToString();
        lemonsInventoryUnits.text = gameData.lemonsInventory.ToString();
        sugarInventoryUnits.text = gameData.sugarInventory.ToString();
        pitcherUI.text = "Pitcher: " + lemonadeStand.leftInPitcher + " / " + lemonadeStand.pitcherSize;
        cashUI.text = "Cash " + "$" + (gameData.playerMoney / 100f).ToString(
            "F2", CultureInfo.InvariantCulture);
        popularityAmountText.text = gameData.playerPopularity.ToString();
        lemonadePrice.text = "$" + (lemonadeStand.lemonadePrice / 100f).ToString(
            "F2", CultureInfo.InvariantCulture);

        sugarSlider.onValueChanged.AddListener(value => { activeSlider = sugarSlider; OnSliderChanged(); });
        waterSlider.onValueChanged.AddListener(value => { activeSlider = waterSlider; OnSliderChanged(); });
        lemonsSlider.onValueChanged.AddListener(value => { activeSlider = lemonsSlider; OnSliderChanged(); });

        DisplayEndDayPanel(false);
    }

    public void DisplayEndDayPanel(bool b)
    {
        endDayPanel.SetActive(b);
    }

    void SlidersStart()
    {
        lemonsSlider.value = 33;
        sugarSlider.value = 33;
        waterSlider.value = 34;

        sugarUnits = PercentageToUnits(sugarSlider.value);
        waterUnits = PercentageToUnits(waterSlider.value);
        lemonsUnits = PercentageToUnits(lemonsSlider.value);

        lemonsRecipeUnits.text = lemonsUnits.ToString();
        sugarRecipeUnits.text = sugarUnits.ToString();
        waterRecipeUnits.text = waterUnits.ToString();
    }

    void OnSliderChanged()
    {
        if (isUpdating) return;

        isUpdating = true;

        int remainingPercent = 100 - (int)activeSlider.value;

        if (activeSlider == sugarSlider)
        {
            float total = waterSlider.value + lemonsSlider.value;

            if (Mathf.Approximately(total, 0f))
            {
                waterSlider.value = remainingPercent / 2f;
                lemonsSlider.value = remainingPercent / 2f;
            }
            else
            {
                waterSlider.value = (waterSlider.value / total) * remainingPercent;
                lemonsSlider.value = (lemonsSlider.value / total) * remainingPercent;
            }
        }
        else if (activeSlider == waterSlider)
        {
            float total = sugarSlider.value + lemonsSlider.value;

            if (Mathf.Approximately(total, 0f))
            {
                sugarSlider.value = remainingPercent / 2f;
                lemonsSlider.value = remainingPercent / 2f;
            }
            else
            {
                sugarSlider.value = (sugarSlider.value / total) * remainingPercent;
                lemonsSlider.value = (lemonsSlider.value / total) * remainingPercent;
            }
        }
        else
        {
            float total = sugarSlider.value + waterSlider.value;

            if (Mathf.Approximately(total, 0f))
            {
                sugarSlider.value = remainingPercent / 2f;
                waterSlider.value = remainingPercent / 2f;
            }
            else
            {
                sugarSlider.value = (sugarSlider.value / total) * remainingPercent;
                waterSlider.value = (waterSlider.value / total) * remainingPercent;
            }
        }

        sugarUnits = PercentageToUnits(sugarSlider.value);
        waterUnits = PercentageToUnits(waterSlider.value);
        lemonsUnits = PercentageToUnits(lemonsSlider.value);

        lemonsRecipeUnits.text = lemonsUnits.ToString();
        sugarRecipeUnits.text = sugarUnits.ToString();
        waterRecipeUnits.text = waterUnits.ToString();

        int remainingUnits = lemonadeStand.pitcherSize - (sugarUnits + waterUnits + lemonsUnits);

        if (remainingUnits > 0)
        {
            if (activeSlider == sugarSlider)
            {
                sugarUnits += remainingUnits;
            }
            else if (activeSlider == waterSlider)
            {
                waterUnits += remainingUnits;
            }
            else
            {
                lemonsUnits += remainingUnits;
            }
        }

        isUpdating = false;
    }

    public int PercentageToUnits(float percentage)
    {
        return Mathf.RoundToInt(lemonadeStand.pitcherSize * percentage / 100);
    }

    public void Sale(int price, int tip)
    {
        unitsSold++;
        unitsSoldUI.text = "Units Sold " + unitsSold;
        gameData.playerMoney += price + tip;
        cashUI.text = "Cash " + "$" + (gameData.playerMoney / 100f).ToString("F2", CultureInfo.InvariantCulture);
    }

    void Clock()
    {
        float hours = timeManager.Hours;
        float minutes = Mathf.Floor(timeManager.Minutes / 10f) * 10f;
        string amPm = timeManager.Hours < 12 ? " AM" : " PM";

        if (hours > 12)
        {

            hours -= 12;

            if (hours < 1)
            {
                timeText.text = 12 + ":" + Mathf.Floor(minutes).ToString("00") + amPm;
            }
            else
            {
                timeText.text = Mathf.Floor(hours).ToString("00") + ":" +
                    Mathf.Floor(minutes).ToString("00") + amPm;
            }
        }
        else
        {
            timeText.text = Mathf.Floor(hours).ToString("00") + ":" +
                Mathf.Floor(minutes).ToString("00") + amPm;
        }
    }

    public void ShowMakeLemonade(bool b)
    {
        makeLemonadeWindow.SetActive(b);
    }

    //10 units total should be ideal of 1(10%) lemons, 7(70%) water, 2(20%) sugar
    //ideal ratio gives value of sweetness 1, tartness 1

    public (float Sweetness, float Tartness) GetLemonadeTaste()
    {
        int pitcherSize = lemonadeStand.pitcherSize; // pitcherSize is 10 for now

        float idealSugarPercent = 0.2f;
        float idealLemonsPercent = 0.1f;
        float idealSugarUnits = idealSugarPercent * pitcherSize;
        float idealLemonsUnits = idealLemonsPercent * pitcherSize;
        float sweetness, tartness;

        float sugarDeviation = Math.Abs(sugarUnits - idealSugarUnits);
        float lemonsDeviation = Math.Abs(lemonsUnits - idealLemonsUnits);

        sweetness = 1 + (sugarDeviation * 0.1f);
        tartness = 1 + (lemonsDeviation * 0.1f);

        float maxPossibleDeviation = Math.Max(idealSugarUnits, pitcherSize - idealSugarUnits);
        maxPossibleSweetness = 1 + (maxPossibleDeviation * 0.1f);
        float maxPossibleTartnessDeviation = Math.Max(idealLemonsUnits, pitcherSize - idealLemonsUnits);
        maxPossibleTartness = 1 + (maxPossibleTartnessDeviation * 0.1f);

        if (sugarUnits < 1)
        {
            sweetness = 0;
        }
        if (lemonsUnits < 1)
        {
            tartness = 0;
        }

        Debug.Log((sweetness, tartness));
        return (sweetness, tartness);
    }

    public void ReduceRecipeUnits()
    {
        gameData.waterBillAmount += waterUnits * waterPrice;
        gameData.lemonsInventory -= lemonsUnits;
        gameData.sugarInventory -= sugarUnits;

        lemonsInventoryUnits.text = gameData.lemonsInventory.ToString();
        sugarInventoryUnits.text = gameData.sugarInventory.ToString();
    }

    public void EndDay()
    {
        WeekdayHandler weekdayHandler = FindObjectOfType<WeekdayHandler>();
        weekdayHandler.SetNextDay();
        gameData.currentDayNumber++;
        sceneHandler.LoadScene(1);
    }

}
