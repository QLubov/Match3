using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
  public Board board;
  public Timer timer;
  public float duration;
  DateTime stamp;
  void Start()
  {
    StartCoroutine(GenerateCoroutine(true));
  }

  public IEnumerator GenerateCoroutine(bool resetTimer = false)
  {    
    board.Generate();
    yield return StartCoroutine(MoveAllDown());

    var toRemove = board.GetMatchThreeElements();    
    yield return StartCoroutine(ProcessBoard(toRemove));

    if (board.GetCombination().Count == 0)
    {
      board.Clear();
      yield return StartCoroutine(GenerateCoroutine());
    }
    if (resetTimer == true)
    {
      timer.StartTimer(TimeSpan.FromSeconds(duration));
    }
    timer.onAction = false;
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
      yield return StartCoroutine(GenerateCoroutine());
    }
  }

  IEnumerator MoveAllDown()
  {
    stamp = DateTime.Now;
    List<Coroutine> cors = new List<Coroutine>();
    for (int i = 0; i < board.Width; ++i)
    {
      for (int j = 0; j < board.Height; ++j)
      {
        var go = board.cells[i, j].Item;
        if (go != null && (go.transform.position != go.transform.parent.position))
          cors.Add(StartCoroutine(Move(go, go.transform.parent.position)));
      }
    }
    foreach (var c in cors)
      yield return c;
    var time = DateTime.Now - stamp;
    Debug.Log(time);
  }

  public IEnumerator Move(Item go, Vector3 target, float speed = 6.0f)
  {
    go.LayoutElement.ignoreLayout = true;
    var direction = (target - go.transform.position);
    while (direction.magnitude >= speed)
    {
      if (go == null)
        yield break;
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
      second.GetComponent<Animator>().SetBool("Focused", true);
      yield break;
    }

    timer.onAction = true;

    yield return StartCoroutine(Swap(first, second));
    var toRemove = board.GetMatchThreeElements(first);

    toRemove.AddRange(board.GetMatchThreeElements(second));
    if (toRemove.Count == 0)
    {
      yield return StartCoroutine(Swap(first, second));
      yield break;
    }
    /*else
    {
      while (toRemove.Count != 0)
      {
        yield return StartCoroutine(RemoveElements(toRemove));
        board.RemoveHoles();
        yield return StartCoroutine(MoveAllDown());
        toRemove = board.GetMatchThreeElements();
      }
    }*/
    
    StartCoroutine(GenerateCoroutine());
  }

  IEnumerator Swap(Item first, Item second)
  {
    StartCoroutine(Move(first, second.transform.parent.position, 4.0f));
    yield return StartCoroutine(Move(second, first.transform.parent.position, 4.0f));

    board.SwapElements(first, second);
  }

  IEnumerator RemoveElements(List<Item> toRemove)
  {
    for (int i = 0; i < toRemove.Count; ++i)
    {
      var go = toRemove[i];
      if (go != null)
      {
        toRemove.AddRange(go.Feature.PerformFeature());
                
        go.GetComponent<Animator>().SetBool("Destroyed", true);
      }
    }

    yield return new WaitWhile(() => { return board.HasElementWithStatus("Destroyed", true); });
    //yield break;
  }
}
