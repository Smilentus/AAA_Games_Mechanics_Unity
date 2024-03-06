using System;
using System.Collections;
using UnityEngine;


public class GameTimeFlowController : MonoBehaviour
{
    public event Action<WorldTimeData> onWorldTimeChanged;


    private WorldTimeData currentWorldTimeData = new WorldTimeData();
    public WorldTimeData CurrentWorldTimeData => currentWorldTimeData;


    private void Awake()
    {
        currentWorldTimeData.onWorldTimeChanged += OnWorldTimeChanged;

        // Пока что в этом моменте просто устанавливаем стандартную дату и время
        currentWorldTimeData.Sync(new WorldTimeData() 
        {
            TimeSeconds = 0,
            TimeMinutes = 0,
            TimeHours = 6,
            TimeDays = 1,
            TimeMonths = 1,
            TimeYears = 2065
        });
    }

    private void Start()
    {
        StartCoroutine(DelayedWorldTimer());
    }


    private void OnWorldTimeChanged()
    {
        onWorldTimeChanged?.Invoke(currentWorldTimeData);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    private IEnumerator DelayedWorldTimer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);

            currentWorldTimeData.TimeMinutes++;
        }
    }

    public void SyncWorldTime(WorldTimeData worldTimeData)
    {
        currentWorldTimeData.Sync(worldTimeData);
    }
}

[System.Serializable]
public struct WorldTimeData
{
    public event Action onWorldTimeChanged;


    private uint timeSeconds;
    public uint TimeSeconds
    {
        get => timeSeconds;
        set
        {
            timeSeconds = value;

            CycleTime();
        }
    }

    private uint timeMinutes;
    public uint TimeMinutes
    {
        get => timeMinutes;
        set
        {
            timeMinutes = value;

            CycleTime();
        }
    }

    private uint timeHours;
    public uint TimeHours
    {
        get => timeHours;
        set
        {
            timeHours = value;

            CycleTime();
        }
    }

    private uint timeDays;
    public uint TimeDays
    {
        get => timeDays;
        set
        {
            timeDays = value;

            CycleTime();
        }
    }

    private uint timeMonths;
    public uint TimeMonths
    {
        get => timeMonths;
        set
        {
            timeMonths = value;

            CycleTime();
        }
    }

    private uint timeYears;
    public uint TimeYears
    {
        get => timeYears;
        set
        {
            timeYears = value;

            CycleTime();
        }
    }


    public void Sync(WorldTimeData worldTimeData)
    {
        TimeSeconds = worldTimeData.TimeSeconds;
        TimeMinutes = worldTimeData.TimeMinutes;    
        TimeHours = worldTimeData.TimeHours;
        TimeDays = worldTimeData.TimeDays;
        TimeMonths = worldTimeData.TimeMonths;
        TimeYears = worldTimeData.TimeYears;

        CycleTime();
    }

    public void CycleTime()
    {
        if (timeSeconds >= 60)
        {
            timeMinutes += timeSeconds / 60;
            timeSeconds = timeSeconds % 60;
        }

        if (timeMinutes >= 60)
        {
            timeHours += timeMinutes / 60;
            timeMinutes = timeMinutes % 60;
        }

        if (timeHours >= 24)
        {
            timeDays += timeHours / 24;
            timeHours = timeHours % 24;
        }

        if (timeDays > 31)
        {
            timeMonths += timeDays / 31;
            timeDays = timeDays % 31;
        }

        if (timeMonths > 12)
        {
            timeYears += timeMonths / 12;
            timeMonths = timeMonths % 12;
        }

        onWorldTimeChanged?.Invoke();
    }
}