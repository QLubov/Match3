using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
  public GameManager gameMgr;
  public Text timerText;

  void Start()
  {
    gameMgr.timer.OnGameEnded += OnGameEnded;
  }

  void Update()
  {
    if (gameMgr.timer.IsStarted == false)
      timerText.text = "" + gameMgr.duration;
    else
      timerText.text = "" + gameMgr.timer.Remains.Seconds;
  }

  //Help button
  public void ShowStep()
  {
    var pair = gameMgr.board.GetCombination();
    StartCoroutine(HighlightPair(pair));
  }

  IEnumerator HighlightPair(List<Item> pair)
  {
    StartCoroutine(gameMgr.Move(pair[1], pair[0].transform.parent.position));
    yield return StartCoroutine(gameMgr.Move(pair[0], pair[1].transform.parent.position));
    StartCoroutine(gameMgr.Move(pair[1], pair[1].transform.parent.position));
    yield return StartCoroutine(gameMgr.Move(pair[0], pair[0].transform.parent.position));
  }

  public void UseBomb()
  {
    Controller.IsBombUsed = true;
  }

  public void UseRecolor()
  {
    Controller.IsRecolorUsed = true;
  }

  public void RegenerateBoard()
  {
    gameMgr.board.Clear();
    gameMgr.StartCoroutine(gameMgr.GenerateCoroutine());
  }

  void OnGameEnded()
  {
    Debug.Log("Game is ended!");
  }

}
