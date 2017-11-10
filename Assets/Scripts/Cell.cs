using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
  public Item Item
  {
    get
    {
      if(transform.childCount > 0)
        return transform.GetChild(0).GetComponent<Item>();
      return null;
    }
    set
    {
      value.transform.SetParent(gameObject.transform);
    }
  }

  public bool IsEmpty
  {
    get
    {
      return Item == null;
    }
  }
}
