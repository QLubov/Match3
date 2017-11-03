using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Controller : MonoBehaviour
{
  static GameObject clicked;
  public GameManager gameMgr;
     
  public void OnClick()
  {    
    if (clicked == null)
    {
      clicked = gameObject;
      clicked.GetComponent<Animator>().SetBool("Focused", true);
    }
    else
    {
      clicked.GetComponent<Animator>().SetBool("Focused", false);
      //StartCoroutine(gameMgr.DoGameStep(clicked, gameObject));
      gameMgr.StartCoroutine(gameMgr.DoGameStep(clicked, gameObject));
      clicked = null;      
    }
  }

  //temp solution for HELP button
  public void ShowStep()
  {
    var pair = gameMgr.board.GetCombination();
    StartCoroutine(HighlightPair(pair));
  }

  IEnumerator HighlightPair(List<GameObject> pair)
  {
    StartCoroutine(gameMgr.Move(pair[1], pair[0].transform.parent.position));
    yield return StartCoroutine(gameMgr.Move(pair[0], pair[1].transform.parent.position));
    StartCoroutine(gameMgr.Move(pair[1], pair[1].transform.parent.position));
    yield return StartCoroutine(gameMgr.Move(pair[0], pair[0].transform.parent.position));
  }
}
