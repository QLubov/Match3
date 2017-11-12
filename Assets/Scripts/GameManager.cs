using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public Board board;
  public Timer timer;
  public BonusCounter bCounter;
  [Range(0.1f, 20.0f)]
  public float SwapSpeed = 4.0f;
  [Range(0.1f, 50.0f)]
  public float MoveSpeed = 6.0f;

  void Start()
  {
    StartCoroutine(ProcessGame(true));
  }

  public IEnumerator ProcessGame(bool resetTimer = false)
  {    
    board.Generate();
    yield return StartCoroutine(MoveAllDown());

    var toRemove = board.GetMatchThreeElements();    
    yield return StartCoroutine(ProcessBoard(toRemove));

    if (board.GetCombination().Count == 0)
    {
      board.Clear();
      yield return StartCoroutine(ProcessGame());
    }
    if (resetTimer == true)
    {
      timer.StartTimer();
    }
  }

  public IEnumerator ProcessBoard(List<Item> toRemove)
  {
    while (toRemove.Count != 0)
    {      
      yield return StartCoroutine(RemoveElements(toRemove));
      
      board.RemoveHoles();
      yield return StartCoroutine(MoveAllDown());
      toRemove = board.GetMatchThreeElements();
    }
    if (board.HasEmptyCell())
    {
      yield return StartCoroutine(ProcessGame());
    }
  }

  IEnumerator MoveAllDown()
  {
    List<Coroutine> cors = new List<Coroutine>();
    for (int i = 0; i < board.Width; ++i)
    {
      for (int j = 0; j < board.Height; ++j)
      {
        var item = board.cells[i, j].Item;
        if (item != null && (item.transform.position != item.transform.parent.position))
          cors.Add(StartCoroutine(Move(item, item.transform.parent.position, MoveSpeed)));
      }
    }
    foreach (var c in cors)
      yield return c;
  }

  public IEnumerator Move(Item go, Vector3 target, float speed)
  {
    go.LayoutElement.ignoreLayout = true;
    var direction = (target - go.transform.position);
    while (direction.magnitude >= speed)
    {
      //if (go == null)
      //  yield break;
      go.transform.position += speed * direction.normalized;
      direction = (target - go.transform.position);
      yield return new WaitForFixedUpdate();
    }
    go.transform.position = target;
    go.LayoutElement.ignoreLayout = false;
  }

  public IEnumerator DoGameStep(Item first, Item second)
  {
    //add if for moving availability (item is locked (frozen))
    if (!board.IsNeighbours(first, second))
    {
      //set focused state for second object and change it for first      
      second.Animator.SetBool("Focused", true);
      yield break;
    }

    yield return StartCoroutine(Swap(first, second));
    var toRemove = board.GetMatchThreeElements(first);

    toRemove.AddRange(board.GetMatchThreeElements(second));
    if (toRemove.Count == 0)
    {
      yield return StartCoroutine(Swap(first, second));
      yield break;
    }
    bCounter.SetUserInput(toRemove);
    yield return StartCoroutine(ProcessBoard(toRemove));
  }

  IEnumerator Swap(Item first, Item second)
  {
    StartCoroutine(Move(first, second.transform.parent.position, SwapSpeed));
    yield return StartCoroutine(Move(second, first.transform.parent.position, SwapSpeed));

    board.SwapElements(first, second);
  }

  IEnumerator RemoveElements(List<Item> toRemove)
  {
    for (int i = 0; i < toRemove.Count; ++i)
    {
      var item = toRemove[i];
      if (item != null)
      {
        toRemove.AddRange(item.Feature.PerformFeature());

        item.Animator.SetBool("Destroyed", true);
      }
    }    
    yield return new WaitWhile(() => { return board.HasElementWithStatus("Destroyed", true); });
    bCounter.CalculateScore(toRemove);
  }

  void OnApplicationQuit()
  {
    Application.Quit();
  }
}
