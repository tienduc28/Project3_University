using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayNightManager : MonoBehaviour
{
    [SerializeField] 
    private Light directionalLight;
    [SerializeField]
    private LightingPreset lightingPreset;

    [SerializeField, Range(0, 24)]
    private float timeOfDay;
    public TimeOfDay timePeriod;
    private float startOfDayTime = 6.0f;
    private float endOfDayTime = 18.0f;
    private float timeMultiplier = 1f;

    #region Singleton
    public static DayNightManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        timePeriod = TimeOfDay.DayTime;
    }

    private void Update()
    {
        if (directionalLight == null)
            return;

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * timeMultiplier;
            timeOfDay %= 24;
            SetDayNight();
            UpdateLightng(timeOfDay);
        }else
        {
            UpdateLightng(timeOfDay);
            SetDayNight();
        }
    }

    private void SetDayNight()
    {
        if (timeOfDay >= startOfDayTime && timeOfDay <= endOfDayTime)
            timePeriod = TimeOfDay.DayTime;
        else
            timePeriod = TimeOfDay.NightTime;
    }
    private void UpdateLightng(float timeOfDay)
    {
        float timePercentage = timeOfDay / 24;

        if (directionalLight != null)
        {
            directionalLight.color = lightingPreset.directionalLightColor.Evaluate(timePercentage);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercentage * 360f) - 90f, 170f, 0));
        }

        RenderSettings.ambientLight = lightingPreset.ambientColor.Evaluate(timePercentage);
        RenderSettings.fogColor = lightingPreset.fogColor.Evaluate(timePercentage);
    }

    public enum TimeOfDay{ DayTime, NightTime};
}
