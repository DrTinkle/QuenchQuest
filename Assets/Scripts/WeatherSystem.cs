using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [SerializeField] GameData gameData;

    int forecastDays = 2; // pelaajalle n‰kyv‰t s‰‰ennustep‰iv‰t
    const int daysInWeek = 7;
    const int initialWeeksForecast = 2; // montako viikkoo generoidaan alkuun

    [SerializeField] AnimationCurve temperatureCurve;
    float curveValue;
    [SerializeField] TimeManager timeManager;

    public float temperatureToDisplay;

    public enum WeatherCondition
    {
        Sunny,
        Cloudy,
        Rainy
    }

    public struct Weather
    {
        public WeatherCondition Condition;
        public float Temperature;

        public Weather(WeatherCondition condition, float temperature)
        {
            Condition = condition;
            Temperature = temperature;
        }
    }

    public Weather CurrentWeather { get; private set; }
    public List<Weather> WeatherForecast { get; private set; } = new List<Weather>();

    void Start()
    {
        GenerateInitialForecast();
    }

    void GenerateInitialForecast()
    {
        for (int i = 0; i < initialWeeksForecast; i++)
        {
            GenerateWeekForecast();
        }
    }

    void GenerateWeekForecast()
    {
        for (int i = 0; i < daysInWeek; i++)
        {
            WeatherCondition randomCondition = (WeatherCondition)UnityEngine.Random.Range(0, 3);
            float temperature = GenerateTemperature(randomCondition);
            WeatherForecast.Add(new Weather(randomCondition, temperature));
        }
    }

    float GenerateTemperature(WeatherCondition condition)
    {
        switch (condition)
        {
            case WeatherCondition.Sunny:
                return UnityEngine.Random.Range(20, 30);
            case WeatherCondition.Cloudy:
                return UnityEngine.Random.Range(15, 25);
            case WeatherCondition.Rainy:
                return UnityEngine.Random.Range(10, 20);
            default:
                return 20;
        }
    }














    //private void Update()
    //{
    //    curveValue = temperatureCurve.Evaluate(timeManager.Hours / 24f);
    //    //temperatureToDisplay = GetTemperatureForWeather(CurrentWeather.Condition) + curveValue;
    //}

    //public void UpdateWeather()
    //{
    //    this.CurrentWeather = this.WeatherForecast[0];
    //    this.WeatherForecast.RemoveAt(0);

    //    if (daysPassed % daysInWeek == 0)
    //    {
    //        GenerateWeekForecast();
    //    }

    //    daysPassed++;
    //}

    //void GenerateWeekForecast()
    //{
    //    for (int i = 0; i < daysInWeek; i++)
    //    {
    //        WeatherCondition randomCondition = (WeatherCondition)UnityEngine.Random.Range(0, 3);
    //        (float maxTemp, float minTemp) = GenerateTemperature(randomCondition);
    //        this.WeatherForecast.Add(new Weather(randomCondition, maxTemp, minTemp));
    //    }
    //}

    ////private (float maxTemp, float minTemp) GenerateTemperature(WeatherCondition condition)
    ////{
    ////    float maxTemp = UnityEngine.Random.Range(0, 40);
    ////    float minTemp = Mathf.Max(0, maxTemp + UnityEngine.Random.Range(minTempChange, 0));
    ////    return (maxTemp, minTemp);
    ////}



    //public List<Weather> GetForecast()
    //{
    //    return this.WeatherForecast.Take(forecastDays).ToList();
    //}

    //public void UpgradeForecastSkill() // t‰m‰ sitten kun on skillej‰
    //{
    //    forecastDays++;
    //}
}

