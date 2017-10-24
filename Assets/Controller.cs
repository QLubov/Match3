using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  public GameManager gameMgr;
  static Controller clicked = null;
  public Animator animator;

  public void OnClick()
  {
    //add somthing about focus    
    if (clicked == null)
    {
      animator.SetBool("IsFocused", true);
      clicked = this;
    }
    else
    {
      bool res = gameMgr.MoveElements(gameObject, clicked.gameObject);
      if (!res)
      {
        //set previous animation false
        clicked.animator.SetBool("IsFocused", false);
        clicked = this;
        animator.SetBool("IsFocused", true);
      }
      else
      {
        animator.SetBool("IsFocused", false);
        clicked = null;
      }      
    }
  }

  public void RemoveMe()
  {
    Destroy(gameObject);
  }

  public void MoveAllDown()
  {
    gameMgr.MoveAllDown();
  }
}
