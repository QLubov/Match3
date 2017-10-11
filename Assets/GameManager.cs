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
    board.Generate(Width, Height);
  } 

  public bool MoveElements(GameObject element, GameObject other)
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

    board.MoveAllDown();
    return true;
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
  }  
}
