using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public enum WeatherType
    {
        Sunny,         // 昼・晴れ
        Evening,       // 夕暮れ・晴れ
        RainyNight     // 夜・雨
    }

    [Header("このシーンの天気")]
    [SerializeField] private WeatherType weatherType;


    [Header("Skybox")]
    [SerializeField] private Material sunnySkybox;
    [SerializeField] private Material eveningSkybox;
    [SerializeField] private Material rainyNightSkybox;


    [Header("Directional Light")]
    [SerializeField] private Light directionalLight;

    [SerializeField] private Color sunnyLightColor = Color.white;
    [SerializeField] private Color eveningLightColor = new Color(1f, 0.55f, 0.3f);
    [SerializeField] private Color rainyNightLightColor = new Color(0.25f, 0.35f, 0.5f);

    [SerializeField] private float sunnyLightIntensity = 1.2f;
    [SerializeField] private float eveningLightIntensity = 0.7f;
    [SerializeField] private float rainyNightLightIntensity = 0.25f;


    [Header("雨")]
    [SerializeField] private GameObject rainObject;


    private void Start()
    {
        ApplyWeather();
    }


    private void ApplyWeather()
    {
        switch (weatherType)
        {
            case WeatherType.Sunny:

                RenderSettings.skybox = sunnySkybox;

                directionalLight.color = sunnyLightColor;
                directionalLight.intensity = sunnyLightIntensity;

                rainObject.SetActive(false);

                break;


            case WeatherType.Evening:

                RenderSettings.skybox = eveningSkybox;

                directionalLight.color = eveningLightColor;
                directionalLight.intensity = eveningLightIntensity;

                rainObject.SetActive(false);

                break;


            case WeatherType.RainyNight:

                RenderSettings.skybox = rainyNightSkybox;

                directionalLight.color = rainyNightLightColor;
                directionalLight.intensity = rainyNightLightIntensity;

                rainObject.SetActive(true);

                break;
        }

        DynamicGI.UpdateEnvironment();
    }
}