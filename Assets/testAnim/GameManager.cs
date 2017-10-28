using System.Collections;

using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
  public int Width = 8;
  public int Height = 8;
  public Board board;

  void Start()
  {
    Generate();
  }

  void Generate()
  {
    board.Generate(Width, Height);
    StartCoroutine(WaitForMovingDown());
  }

  IEnumerator WaitForMovingDown()
  {
    yield return new WaitWhile(() => { return board.HasElementWithStatus(ElementState.MoveDown); });
    if (board.HasEmptyCell())
      Generate();
    if (board.HasMatchThree())
      StartCoroutine(MatchThree());
    else
      StartCoroutine(WaitForInput());
  }

  IEnumerator WaitForInput()
  {
    yield return new WaitWhile(() => { return !Controller.HasReadyElements(); });
    var objs = Controller.GetReadyElements();
    StartCoroutine(Swap(objs[0], objs[1]));
  }

  IEnumerator WaitForDestroying()
  {
    yield return new WaitWhile(() => { return board.HasElementWithStatus(ElementState.Destroying); });
    StartCoroutine(WaitForMovingDown());
  }

  IEnumerator MatchThree()
  {
    board.MatchThree();
    yield return StartCoroutine(WaitForDestroying());
    if (IsGamePlayable())
      StartCoroutine(WaitForInput());
    //TODO: REgenerate board
  }

  bool IsGamePlayable()
  {
    return true;
  }

  IEnumerator Swap(GameObject first, GameObject second)
  {
    board.SwapElements(first, second);
    var toRemove1 = board.GetMatchThreeElements(first);
    var toRemove2 = board.GetMatchThreeElements(second);
    board.SwapElements(first, second);
    if (toRemove1.Count == 0 && toRemove2.Count == 0)
    {
      //set SWAP + SWAP_BACK status for both objects
      first.GetComponent<Controller>().SetState(ElementState.SwapSwapBack);
      second.GetComponent<Controller>().SetState(ElementState.SwapSwapBack);
      first.GetComponent<Moving>().SetTarget(second.transform.parent, true);
      second.GetComponent<Moving>().SetTarget(first.transform.parent, true);

      StartCoroutine(WaitForInput());
      yield break;
    }
    //set swap for both
    first.GetComponent<Controller>().SetState(ElementState.Swapped);
    second.GetComponent<Controller>().SetState(ElementState.Swapped);
    first.GetComponent<Moving>().SetTarget(second.transform.parent);
    second.GetComponent<Moving>().SetTarget(first.transform.parent);
    yield return new WaitWhile(() => {return board.HasElementWithStatus(ElementState.Swapped); });
    //probably need to wait swap animation is ended
    if (toRemove1.Count != 0)
    {
      Controller.SetStateForObjects(toRemove1, ElementState.Destroying);
    }
    /*else
    {
      //set DESTROYING to first
    }*/
    if (toRemove2.Count != 0)
    {
      Controller.SetStateForObjects(toRemove2, ElementState.Destroying);
    }
    /*else
    {
      
    }*/

    StartCoroutine(WaitForDestroying());
  }

  bool HasMatchThree()
  {
    return false;
  }

  /*public bool MoveElements(GameObject element, GameObject other)
  {
    //extract to separate func, which
    //checks behaviour availability also
    //(e.x. frozen element)

    if (!board.IsNeighbours(element, other))
    {
      return false;
    }

    board.SwapElements(element, other);
    //play animation
    var toRemove = new List<GameObject>();
    toRemove.AddRange(board.GetMatchThreeElements(element));
    toRemove.AddRange(board.GetMatchThreeElements(other));
    if (toRemove.Count == 0)
    {
      board.SwapElements(element, other);
      //play animation?
      return false;
    }
    MatchThree(toRemove);
       
    return true;
  }

  public void MoveAllDown()
  {
    board.MoveAllDown();
    //StartCoroutine(GenerateNewCorutine());
    board.GenerateNewElements();
  }

  IEnumerator GenerateNewCorutine()
  {
    yield return new WaitWhile(() =>
    {

      var canvas = GameObject.Find("Canvas");
      return canvas.transform.FindChild("Image") != null;
    });
    board.GenerateNewElements();
  }

  void MatchThree(List<GameObject> toRemove)
  {
    foreach (var go in toRemove)
    {
      //TODO:
      //call behaviour.Exec() to add new elements to toRemove
      //call View.Exec() to play anim, etc.
      board.RemoveElement(go);
    }
  } */
}
