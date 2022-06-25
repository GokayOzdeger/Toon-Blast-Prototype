using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.Events;
using System;
#if UNITY_IOS || UNITY_EDITOR
using UnityEngine.iOS;
#endif
[CreateAssetMenu(menuName = "ScriptableObjects/Singletons/DeviceManager")]
public class DeviceManager : ScriptableSingleton<DeviceManager>
{
#if UNITY_EDITOR
    [Header("Editor Only")]
    public DeviceGeneration testGen;
    public DeviceOrientation testOrientation;
#endif
    private DeviceOrientation _lastOrientation;

    public DeviceOrientation DeviceOrientate { get; private set; }
    public string DeviceModel { get; private set; }
    public float DeviceAspectRatio { get; private set; }
    public float DeviceInches { get; private set; }
    public bool IsTablet { get; private set; }
    public bool HasNotch { get; private set; }

    public event Action DeviceOrientationChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void FirstInitialize()
    {
        Instance.GetInches();
        Instance.GetRatio();
        Instance.GetIfTablet();
        Instance.GetDeviceModel();
        Instance.GetIfHasNotch();
        Instance.InstantiateUpdater();
        Instance.SetMaxFramerate();
    }

    private void InstantiateUpdater()
    {
        GameObject updater = new GameObject("Device Manager Updater");
        updater.AddComponent<DeviceManagerUpdate>();
        DontDestroyOnLoad(updater);
    }

    public void Update()
    {
        CheckOrientation();
    }

    private void CheckOrientation()
    {
        DeviceOrientation myOrientation;
#if UNITY_EDITOR
        myOrientation = testOrientation;
#else
            myOrientation = Input.deviceOrientation;
#endif
        if (myOrientation == DeviceOrientation.LandscapeLeft || myOrientation == DeviceOrientation.LandscapeRight)
        {
            if (_lastOrientation != myOrientation)
            {
                DeviceOrientate = myOrientation;
                DeviceOrientationChanged?.Invoke();
            }
            _lastOrientation = myOrientation;
        }
    }

    private void GetInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        DeviceInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
    }

    private void SetMaxFramerate()
    {
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        Application.targetFrameRate = int.MaxValue;
#endif
    }

    public void SetMaxFramerate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
#if UNITY_EDITOR
        Application.targetFrameRate = int.MaxValue;
#endif
    }
    private void GetRatio()
    {
        DeviceAspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
    }

    private void GetIfTablet()
    {
        IsTablet = (DeviceInches > 6.5f) && (DeviceAspectRatio < 2);
    }

    private void GetIfHasNotch()
    {
#if UNITY_IOS || UNITY_EDITOR
        DeviceGeneration[] notchGens =
            {
                DeviceGeneration.iPhoneX,
                DeviceGeneration.iPhoneXR,
                DeviceGeneration.iPhoneXS,
                DeviceGeneration.iPhoneXSMax,

                DeviceGeneration.iPhone11,
                DeviceGeneration.iPhone11Pro,
                DeviceGeneration.iPhone11ProMax,

                DeviceGeneration.iPhone12,
                DeviceGeneration.iPhone12Mini,
                DeviceGeneration.iPhone12Pro,
                DeviceGeneration.iPhone12ProMax,
            };
        if (new List<DeviceGeneration>(notchGens).Contains(Device.generation)) HasNotch = true;
#elif UNITY_ANDROID
        HasNotch = false;
#endif
    }
    private void GetDeviceModel()
    {
        DeviceModel = SystemInfo.deviceModel;
    }


}

public class DeviceManagerUpdate: MonoBehaviour
{
    private DeviceManager cachedInstance;

    private void Awake()
    {
        cachedInstance = DeviceManager.Instance;
#if !UNITY_EDITOR && UNITY_IOS 
        DeviceGeneration[] lowSettingGens =
            {
                DeviceGeneration.iPhone6,
                DeviceGeneration.iPhone6S,
                DeviceGeneration.iPhone6Plus,
                DeviceGeneration.iPhone6SPlus,
                DeviceGeneration.iPhone7,
                DeviceGeneration.iPhone7Plus
    };
        if (new List<DeviceGeneration>(lowSettingGens).Contains(Device.generation))
        {
            Debug.Log($"Last Quality Settings is index {QualitySettings.GetQualityLevel()}");
            QualitySettings.SetQualityLevel(0);
            Debug.Log($"Next Quality Settings is index {QualitySettings.GetQualityLevel()}");

        }
#endif
    }
    private void Update()
    {
        cachedInstance.Update();
    }
}
