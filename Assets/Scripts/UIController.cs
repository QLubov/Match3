using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
  public GameManager gameMgr;
  public Text TimerText;
  public Text ScoreText;
  public Image FinishWindow;

  void Start()
  {
    gameMgr.timer.OnTimerExpired += OnTimerExpired;
    Controller.OnGameEnded += OnGameEnded;
  }

  void Update()
  {
    TimerText.text = $"{gameMgr.timer.Remains.Minutes} : {gameMgr.timer.Remains.Seconds}";
    ScoreText.text = "" + gameMgr.bCounter.counter.Score;
  }

  //Help button
  public void ShowStep()
  {
    var pair = gameMgr.board.GetCombination();
    StartCoroutine(HighlightPair(pair));
  }

  IEnumerator HighlightPair(List<Item> pair)
  {
    StartCoroutine(gameMgr.Move(pair[1], pair[0].transform.parent.position, gameMgr.SwapSpeed));
    yield return StartCoroutine(gameMgr.Move(pair[0], pair[1].transform.parent.position, gameMgr.SwapSpeed));
    StartCoroutine(gameMgr.Move(pair[1], pair[1].transform.parent.position, gameMgr.SwapSpeed));
    yield return StartCoroutine(gameMgr.Move(pair[0], pair[0].transform.parent.position, gameMgr.SwapSpeed));
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
    gameMgr.StartCoroutine(gameMgr.ProcessGame());
  }

  void OnTimerExpired()
  {
    Debug.Log("UIController is ended!");
    //Deactivate UI
  }

  void OnGameEnded()
  {
    //show message box
    Text score = FinishWindow.transform.Find("Score").GetComponent<Text>();
    score.text = "" + gameMgr.bCounter.counter.Score;
    FinishWindow.gameObject.SetActive(true);
  }

}
