using System.Collections;
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

  public void RemoveMe()
  {    
    transform.SetParent(null);
    Destroy(gameObject);    
  }
}
