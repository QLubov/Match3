using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementState
{
  MoveDown,
  Idle,
  Focused,
  Swapped,
  SwapSwapBack,  
  Destroying
  //ReadyToSwap
}

public class Controller : MonoBehaviour
{
  public GameManager gameMgr;
  static Controller clicked = null;
  static Controller current = null;
  public Animator animator;

  public void SetState(ElementState state)
  {
    //Debug.Log("Set state to " + state.ToString());
    animator.SetInteger("State", (int)state);
  }

  public ElementState GetState()
  {
    return (ElementState)animator.GetInteger("State");
  }

  public static void SetStateForObjects(List<GameObject> list, ElementState state)
  {
    foreach (var go in list)
      go.GetComponent<Controller>().SetState(state);
  }

  public static bool HasReadyElements()
  {
    return clicked != null && current != null;
  }

  public static List<GameObject> GetReadyElements()
  {
    var res = new List<GameObject>();
    res.Add(clicked.gameObject);
    res.Add(current.gameObject);
    return res;
  }

  public void OnClick()
  {
    //add somthing about focus    
    if (clicked == null)
    {
      SetState(ElementState.Focused);
      clicked = this;
    }
    else
    {
      //extract to separate func, which
      //checks behaviour availability also
      //(e.x. frozen element)

      if (gameMgr.board.IsNeighbours(clicked.gameObject, gameObject))
      {
        current = this;
        //clicked.SetState(ElementState.ReadyToSwap);
        //SetState(ElementState.ReadyToSwap);
      }
      else
      {
        clicked.SetState(ElementState.Idle);
        SetState(ElementState.Focused);
        clicked = this;
        current = null;
      }
    }
  }
}
