using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] Light directionalLight;
    [SerializeField] LightsManager lightsManager;
    [SerializeField] UIHandler ui;

    public float time { get; private set; }
    float startHour = 6.0f;
    [SerializeField] float dayLength = 60.0f;

    float maxIntensity = 1.0f;
    float dawn = 6.0f;
    float dusk = 18.0f;
    float noon = 12.0f;

    float dayTimeAmbientIntensity = 1.0f;
    float nightTimeAmbientIntensity = 0.1f;

    float dayTimeReflectionIntensity = 1.0f;
    float nightTimeReflectionIntensity = 0.1f;

    Vector3 nightTimeDirection = new(60f, -30f, 0f);
    Vector3 dayTimeDirection = new(60f, 150f, 0f);

    Color sunriseColor = new(1f, 0.64f, 0.37f);
    Color noonColor = Color.white;
    Color sunsetColor = new(0.69f, 0.13f, 0.13f);

    public float Hours { get; private set; }
    public float Minutes { get; private set; }


    float lastUpdateTime;
    float minutesPerDay;
    float updateInterval;

    bool isEndOfDayPanelShown;

    void Awake()
    {
        minutesPerDay = 24.0f * 60.0f;
        updateInterval = 1f / minutesPerDay;

        Time.timeScale = 1;


        time = startHour / 24.0f;
        lastUpdateTime = -updateInterval;
    }

    private void Start()
    {
        lightsManager.SwitchLights(true);
    }

    void Update()
    {
        CalculateTime();
        AdjustLighting();
        AdjustLightAndAmbientColor();
        ControlSpotLights();
        ControlGameTimeScale();
    }

    void CalculateTime()
    {
        time += Time.deltaTime / dayLength;

        if (time > 1f)
        {
            time -= 1f;
        }

        Hours = time * 24.0f;
        Minutes = (Hours - Mathf.Floor(Hours)) * 60.0f;
        Minutes = Mathf.Floor(Minutes);
    }

    void AdjustLighting()
    {
        if (time - lastUpdateTime > updateInterval)
        {
            float rotationAngleY = Mathf.Lerp(180f, 0f, InterpolateRotationValue(Hours));
            float rotationAngleX = Mathf.Lerp(0f, 90f, InterpolateRotationValue(Hours));

            Vector3 eulerRotation = new(rotationAngleX, rotationAngleY, 0f);
            directionalLight.transform.rotation = Quaternion.Euler(eulerRotation);

            directionalLight.intensity = InterpolateIntensityValue(Hours) * maxIntensity;
            UnityEngine.RenderSettings.ambientIntensity = Mathf.Lerp(
                nightTimeAmbientIntensity, dayTimeAmbientIntensity, InterpolateIntensityValue(Hours));
            UnityEngine.RenderSettings.reflectionIntensity = Mathf.Lerp(
                nightTimeReflectionIntensity, dayTimeReflectionIntensity, InterpolateIntensityValue(Hours));

            lastUpdateTime = time;
        }
    }

    void AdjustLightAndAmbientColor()
    {
        if (Hours >= dawn && Hours < noon)
        {
            directionalLight.color = Color.Lerp(noonColor, sunriseColor, 1f - InterpolateIntensityValue(Hours));
            UnityEngine.RenderSettings.ambientLight = Color.Lerp(noonColor, sunriseColor, 1f - InterpolateIntensityValue(Hours));
        }
        else if (Hours >= noon && Hours < dusk)
        {
            directionalLight.color = Color.Lerp(sunsetColor, noonColor, InterpolateIntensityValue(Hours));
            UnityEngine.RenderSettings.ambientLight = Color.Lerp(sunsetColor, noonColor, InterpolateIntensityValue(Hours));
        }
        else
        {
            directionalLight.color = sunsetColor;
            UnityEngine.RenderSettings.ambientLight = sunsetColor;
        }
    }

    void ControlSpotLights()
    {
        if (Hours > 9f)
        {
            lightsManager.SwitchLights(false);
        }
    }

    float InterpolateIntensityValue(float hours)
    {
        float normalizedValue;

        if (hours >= noon && hours <= dusk)
        {
            normalizedValue = 1f - (hours - noon) / (dusk - noon);
        }
        else if (hours >= dawn && hours < noon)
        {
            normalizedValue = (hours - dawn) / (noon - dawn);
        }
        else
        {
            normalizedValue = 1f;
        }

        float minimumValue = 0.1f;
        normalizedValue = Mathf.Clamp(normalizedValue, minimumValue, 1.0f);

        return normalizedValue;
    }

    float InterpolateRotationValue(float hours)
    {
        float normalizedValue;
        float minValue = dusk;
        float maxValue = dawn;

        normalizedValue = (hours - minValue) / (maxValue - minValue);

        return normalizedValue;
    }

    void ControlGameTimeScale()
    {
        if (!isEndOfDayPanelShown && Hours >= 18f)
        {
            ui.DisplayEndDayPanel(true);
            Time.timeScale = 0f;
            isEndOfDayPanelShown = true;
        }
    }





































































    //    [SerializeField] Light directionalLight;
    //    [SerializeField] LightsManager lightsManager;

    //    public float time;
    //    [SerializeField] float startHour = 6.0f;
    //    [SerializeField] float dayLength = 480.0f;

    //    [SerializeField] private float maxIntensity = 1.0f;
    //    [SerializeField] private float dawn = 6.0f;
    //    [SerializeField] private float dusk = 18.0f;
    //    [SerializeField] private float noon = 12.0f;

    //    [SerializeField] float dayTimeAmbientIntensity = 1.0f;
    //    [SerializeField] float nightTimeAmbientIntensity = 0.1f;

    //    [SerializeField] float dayTimeReflectionIntensity = 1.0f;
    //    [SerializeField] float nightTimeReflectionIntensity = 0.1f;

    //    [SerializeField] private Vector3 nightTimeDirection = new(60f, -30f, 0f);
    //    [SerializeField] private Vector3 dayTimeDirection = new(60f, 150f, 0f);

    //    [SerializeField] private Color sunriseColor = new(1f, 0.64f, 0.37f);
    //    [SerializeField] private Color noonColor = Color.white;
    //    [SerializeField] private Color sunsetColor = new(0.69f, 0.13f, 0.13f);

    //    public float Hours { get; private set; }
    //    public float Minutes { get; private set; }

    //    UIHandler ui;

    //    float lastUpdateTime;
    //    float minutesPerDay;
    //    float updateInterval;

    //void Awake()
    //{
    //    ui = FindObjectOfType<UIHandler>();
    //    minutesPerDay = 24.0f * 60.0f;
    //    updateInterval = 1f / minutesPerDay;

    //    Time.timeScale = 1;


    //    time = startHour / 24.0f;
    //    lastUpdateTime = -updateInterval;
    //}

    //private void Start()
    //{
    //    lightsManager.SwitchLights(true);
    //}

    //void Update()
    //{
    //    CalculateTime();
    //    AdjustLighting();
    //AdjustLightAndAmbientColor();
    //ControlSpotLights();
    //ControlGameTimeScale();
    //}

    //void CalculateTime()
    //{
    //    time += Time.deltaTime / dayLength;

    //    if (time > 1f)
    //    {
    //        time -= 1f;
    //    }

    //    Hours = time * 24.0f;
    //    Minutes = (Hours - Mathf.Floor(Hours)) * 60.0f;
    //    Minutes = Mathf.Floor(Minutes);
    //}

    //void AdjustLighting()
    //{
    //if (time - lastUpdateTime > updateInterval)
    //{
    //directionalLight.transform.rotation = Quaternion.Euler(Vector3.Lerp(
    //    nightTimeDirection, dayTimeDirection, InterpolateIntensityValue(Hours)));

    //directionalLight.intensity = InterpolateIntensityValue(Hours) * maxIntensity;
    //        //RenderSettings.ambientIntensity = Mathf.Lerp(
    //        //    nightTimeAmbientIntensity, dayTimeAmbientIntensity, InterpolateIntensityValue(Hours));
    //        //RenderSettings.reflectionIntensity = Mathf.Lerp(
    //        //    nightTimeReflectionIntensity, dayTimeReflectionIntensity, InterpolateIntensityValue(Hours));

    //        //lastUpdateTime = time;
    //    //}
    //}

    ////void AdjustLightAndAmbientColor()
    ////{
    ////    if (Hours >= dawn && Hours < noon)
    ////    {
    ////        directionalLight.color = Color.Lerp(sunriseColor, noonColor, InterpolateIntensityValue(Hours));
    ////        RenderSettings.ambientLight = Color.Lerp(sunriseColor, noonColor, InterpolateIntensityValue(Hours));
    ////    }
    ////    else if (Hours >= noon && Hours < dusk)
    ////    {
    ////        directionalLight.color = Color.Lerp(noonColor, sunsetColor, InterpolateIntensityValue(Hours));
    ////        RenderSettings.ambientLight = Color.Lerp(noonColor, sunsetColor, InterpolateIntensityValue(Hours));
    ////    }
    ////    else
    ////    {
    ////        directionalLight.color = sunsetColor;
    ////        RenderSettings.ambientLight = sunsetColor;
    ////    }
    ////}

    ////void ControlSpotLights()
    ////{
    ////    if (Hours > 9f)
    ////    {
    ////        lightsManager.SwitchLights(false);
    ////    }
    ////}



    //float InterpolateIntensityValue(float hours)
    //{
    //    float normalizedValue;
    //    float minValue = dawn;
    //    float maxValue = noon;
    //    if(hours >= noon) 
    //    {
    //        minValue = noon;
    //        maxValue = dusk;
    //        hours = Mathf.Clamp(hours, minValue, maxValue);
    //        normalizedValue = 1f - (hours - minValue) / (maxValue - minValue);
    //    }

    //    hours = Mathf.Clamp(hours, minValue, maxValue);
    //    normalizedValue = (hours - minValue) / (maxValue - minValue);

    //    return normalizedValue;
    //}
}
