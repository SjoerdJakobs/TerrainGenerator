using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

    public bool IsRunning { get; set; }

    private float timerTimer;
    public float TimerTime
    {
        get { return timerTimer;  }
    }

    public void StartTimer(float startTime = 0)
    {
        StopCoroutine(RunTimer());
        StartCoroutine(RunTimer(startTime));
        IsRunning = true;
    }
   
    public void StopTimer()
    {
        StopCoroutine(RunTimer());
        IsRunning = false;
    }

    public void ResetTimer()
    {
        StopTimer();
        StartTimer();
    }

    private IEnumerator RunTimer(float startTime = 0)
    {
        timerTimer = startTime;
        while(true)
        {
            timerTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
