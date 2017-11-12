using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
  static Item clicked;
  public GameManager gameMgr;
  bool stepInProgress = false;
  bool IsGameEnded = false;
  public static bool IsBombUsed = false;
  public static bool IsRecolorUsed = false;
  public static Action OnGameEnded { get; set; }

  void Start()
  {
    gameMgr.timer.OnTimerExpired += OnTimerExpired;
  }

  public void OnClick(BaseEventData data)
  {
    if (stepInProgress || IsGameEnded)
      return;
    var item = ((PointerEventData)data).pointerCurrentRaycast.gameObject.GetComponent<Item>();
    //TODO: do not forget to set timer is On Action for these two cases
    //StartCoroutine(DoAdditional(clicked, item));
    if (clicked == null)
    {
      clicked = item;
      clicked.Animator.SetBool("Focused", true);
    }
    else
    {
      clicked.Animator.SetBool("Focused", false);
      //It was a very big mistake to start coroutine for object which was about to destroy
      //StartCoroutine(gameMgr.DoGameStep(clicked, gameObject));
      StartCoroutine(DoGameStep(clicked, item));
      clicked = null;
    }
  }

  IEnumerator DoGameStep(Item first, Item second)
  {
    stepInProgress = true;
    yield return gameMgr.StartCoroutine(gameMgr.DoGameStep(first, second));
    stepInProgress = false;
  }

  IEnumerator DoAdditional(Item first, Item second)
  {
    stepInProgress = true;
    if (IsBombUsed == true)
    {
      clicked?.Animator.SetBool("Focused", false);
      clicked = null;
      second.Feature.SetFeatureType(FeatureType.Bomb);
      var toRemove = new List<Item>();
      toRemove.Add(second);      
      IsBombUsed = false;
      yield return gameMgr.StartCoroutine(gameMgr.ProcessBoard(toRemove));
    }
    else if (IsRecolorUsed == true)
    {
      clicked?.Animator.SetBool("Focused", false);
      clicked = null;
      second.Feature.SetFeatureType(FeatureType.Recolor);
      second.Feature.PerformFeature();
      second.Feature.SetFeatureType(FeatureType.None);
      
      IsRecolorUsed = false;
      yield return gameMgr.StartCoroutine(gameMgr.ProcessGame());
    }
    stepInProgress = false;
  }

  void OnTimerExpired()
  {
    StartCoroutine(FinishGame());
  }

  IEnumerator FinishGame()
  {
    yield return new WaitWhile(() => stepInProgress == true);
    IsGameEnded = true;
    OnGameEnded();
  }
}
