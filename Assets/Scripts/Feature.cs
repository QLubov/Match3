using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FeatureType
{
  Bomb,
  Lighting,
  None,
  Recolor
}

public class Feature : MonoBehaviour
{
  public FeatureType fType;
  public Board board;

  public void Start()
  {
    if (Random.Range(0, 40) == 1)
    {
      SetFeatureType((FeatureType)Random.Range(0, 3));
    }    
  }

  public void SetFeatureType(FeatureType type)
  {
    fType = type;
    GetComponent<Animator>().SetInteger("FeatureType", (int)fType);
  }

  public void OnDestroySelection()
  {
    GetComponent<Animator>().SetInteger("FeatureType", (int) fType);
  }

  public List<GameObject> PerformFeature()
  {
    var res = new List<GameObject>();
    switch (fType)
    {
      case FeatureType.Bomb:
        {
          res.AddRange(board.GetAround(GetComponent<BoardField>()));
          break;
        }
      case FeatureType.Lighting:
        {
          res.AddRange(board.GetRandomElements());
          break;
        }
      case FeatureType.None:
        {
          break;
        }
      case FeatureType.Recolor:
        {
          board.RecolorElements(GetComponent<BoardField>());
          break;
        }
    }
    return res;
  }

  public void RemoveMe()
  {
    transform.SetParent(null);
    Destroy(gameObject);
  }
}
