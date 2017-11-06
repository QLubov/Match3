using System;
using System.Collections;
using UnityEngine;

//the same as counter
public class Timer : MonoBehaviour
{
  DateTime startTime;
  public Action OnGameEnded { get; set; }
  public bool onAction { get; set; }
  public TimeSpan Remains { get; private set; }
  public bool IsStarted { get; private set; }

  void Start()
  {
    IsStarted = false;
  }

  public void StartTimer(TimeSpan duration)
  {
    Remains = duration;
    IsStarted = true;
    startTime = DateTime.Now;
    StartCoroutine(StartTimerCoroutine(duration));
  }

  IEnumerator StartTimerCoroutine(TimeSpan duration)
  {
    while (DateTime.Now - startTime < duration)
    {
      Remains = duration - (DateTime.Now - startTime);
      yield return new WaitForEndOfFrame();
    }
    while (onAction == true)
      yield return new WaitForEndOfFrame();
    IsStarted = false;
    OnGameEnded();
  }


}
