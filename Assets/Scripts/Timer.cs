using System;
using System.Collections;
using UnityEngine;

//TODO:the same as counter or not
public class Timer : MonoBehaviour
{
  DateTime startTime;
  public Action OnTimerExpired { get; set; }
  public TimeSpan Remains { get; private set; }
  //TODO: print this field in GameMgr editor config
  public float duration;

  void Start()
  {
    Remains = TimeSpan.FromSeconds(duration);
  }

  public void StartTimer()
  {
    startTime = DateTime.Now;
    StartCoroutine(StartTimerCoroutine());
  }

  public void AddTime(float addition)
  {
    Remains += TimeSpan.FromSeconds(addition);
  }

  IEnumerator StartTimerCoroutine()
  {
    while (Remains > TimeSpan.Zero)
    {
      Remains -= TimeSpan.FromSeconds(Time.deltaTime);
      yield return new WaitForFixedUpdate();
    }
    OnTimerExpired();
  }
}
