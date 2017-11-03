using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
  public int Width = 8;
  public int Height = 8;
  public Board board;

  public void Start()
  {
    StartCoroutine(GenerateCoroutine());
  }

  IEnumerator GenerateCoroutine()
  {
    board.Generate(Width, Height);
    yield return StartCoroutine(MoveAllDown());
        
    var toRemove = board.GetMatchThreeElements();
    while (toRemove.Count != 0)
    {
      Debug.Log("MatchThree...");
      yield return StartCoroutine(RemoveElements(toRemove));
      board.RemoveHoles();
      yield return StartCoroutine(MoveAllDown());
      toRemove = board.GetMatchThreeElements();
    }
    if (board.HasEmptyCell())
    {
      yield return StartCoroutine(GenerateCoroutine());
    }

    if (board.GetCombination().Count == 0)
    {
      board.Clear();
      yield return StartCoroutine(GenerateCoroutine());
    }
  }

  IEnumerator MoveAllDown()
  {
    List<Coroutine> cors = new List<Coroutine>();
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var go = board.GetElement(i, j);
        if (go != null)
          cors.Add(StartCoroutine(Move(go, go.transform.parent.position)));
      }
    }
    foreach (var c in cors)
      yield return c;
  }

  public IEnumerator Move(GameObject go, Vector3 target, float speed = 6.0f)
  {
    go.GetComponent<LayoutElement>().ignoreLayout = true;
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
    go.GetComponent<LayoutElement>().ignoreLayout = false;
  }

  public IEnumerator DoGameStep(GameObject first, GameObject second)
  {
    //add if for moving availability (item is locked (frozen))
    if (!board.IsNeighbours(first, second))
    {
      //set focused state for second object and change it for first      
      second.GetComponent<Animator>().SetBool("Focused", true);
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

  IEnumerator Swap(GameObject first, GameObject second)
  {
    StartCoroutine(Move(first, second.transform.parent.position, 4.0f));
    yield return StartCoroutine(Move(second, first.transform.parent.position, 4.0f));

    board.SwapElements(first, second);
  }

  IEnumerator RemoveElements(List<GameObject> toRemove)
  {
    for(int i = 0; i < toRemove.Count; ++i)
    {
      var go = toRemove[i];
      if (go != null)
      {
        toRemove.AddRange(go.GetComponent<Feature>().PerformFeature());

        Debug.Log(go.GetComponent<Feature>().fType);
        go.GetComponent<Animator>().SetBool("Destroyed", true);
      }
    }

    yield return new WaitWhile(() => { return board.HasElementWithStatus("Destroyed", true); });
    //yield break;
  }
}
