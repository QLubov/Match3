﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
  public GameObject ReferenceCell;
  public List<Color> colors = new List<Color>();
  public int Width { get; private set; }
  public int Height { get; private set; }

  System.Random r = new System.Random();

  public void Generate(int width, int height)
  {
    Width = width;
    Height = height;
    
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var obj = GameObject.Instantiate(ReferenceCell);
        Image img = obj.transform.GetChild(0).gameObject.GetComponent<Image>();
        BoardField e = img.GetComponent<BoardField>();
        e.X = i;
        e.Y = j;
        e.ID = GetRandomColor();
        img.color = colors[e.ID];
        obj.transform.SetParent(gameObject.transform);
       
      }
    }
  }

  public GameObject GetElement(int i, int j)
  {
    var cell = GetCell(i, j).transform;
    if (cell.childCount > 0)
      return cell.GetChild(0).gameObject;
    return null;
  }

  public void SwapElements(GameObject first, GameObject second)
  {
    Transform temp = first.transform.parent;
    first.transform.SetParent(second.transform.parent);
    second.transform.SetParent(temp);    
    SwapIndexes(first, second);
  }

  public List<GameObject> GetMatchThreeElements(GameObject element)
  {
    var ge = element.GetComponent<BoardField>();
    var result = new List<GameObject>();
    result.AddRange(GetElementsFromLine(ge));
    result.AddRange(GetElemenetsFromColumn(ge));
    
    if (result.Count != 0)
      result.Add(element);

    return result;
  }

  List<GameObject> GetElementsFromLine(BoardField ge)
  {
    var result = new List<GameObject>();

    for (int i = ge.X - 1; i >= 0; --i)
    {
      var go = GetElement(i, ge.Y);
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    for (int i = ge.X + 1; i < Width; ++i)
    {
      var go = GetElement(i, ge.Y);
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    if (result.Count < 2)
      return new List<GameObject>();
    return result;
  }

  List<GameObject> GetElemenetsFromColumn(BoardField ge)
  {
    var result = new List<GameObject>();

    for (int i = ge.Y + 1; i < Height; ++i)
    {
      var go = GetElement(ge.X, i);
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    for (int i = ge.Y - 1; i >= 0; --i)
    {
      var go = GetElement(ge.X, i);
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    if (result.Count < 2)
      return new List<GameObject>();
    return result;
  }

  bool IsSameElements(GameObject go, BoardField ge)
  {
    if (go == null)
      return false;
    if (go.GetComponent<BoardField>().ID != ge.ID)
    {
      return false;
    }
    return true;
  }

  public void RemoveElement(GameObject go)
  {
    RemoveObject(go);
  }

  void RemoveObject(GameObject go)
  {
    go.GetComponent<Animator>().SetBool("IsDestroyed", true);
    go.transform.SetParent(GameObject.Find("Canvas").transform);
  }

  GameObject GetCell(int i, int j)
  {
    return gameObject.transform.GetChild(i * Width + j).gameObject.transform.gameObject;
  }

  void SwapIndexes(GameObject element, GameObject other)
  {
    var e1 = element.GetComponent<BoardField>();
    var e2 = other.GetComponent<BoardField>();
    int tX = e1.X;
    int tY = e1.Y;

    e1.X = e2.X;
    e1.Y = e2.Y;
    e2.X = tX;
    e2.Y = tY;
  }

  List<GameObject> GetAbove(int row, int col)
  {
    var result = new List<GameObject>();
    for (int i = row; i >= 0; --i)
    {
      var go = GetElement(i, col);
      if (go != null)
        result.Add(go);
    }
    return result;
  }

  public void MoveAllDown()
  {
    for (int j = 0; j < Width; ++j)
    {
      for (int i = Height - 1; i >= 0; --i)
      {
        var go = GetElement(i, j);
        if (go == null)
        {
          var objects = GetAbove(i, j);
          if (objects.Count == 0)
            break;
          int k = i;
          foreach (var obj in objects)
          {
            //obj.transform.SetParent(GetCell(k, j).transform);
            obj.GetComponent<Moving>().SetTarget(GetCell(k, j).transform);
            var ge = obj.GetComponent<BoardField>();
            ge.X = k;
            ge.Y = j;
            k--;
          }
          j++;
          break;
        }
      }
    }
  }

  public bool IsNeighbours(GameObject first, GameObject second)
  {
    var e1 = first.GetComponent<BoardField>();
    var e2 = second.GetComponent<BoardField>();

    return (Math.Abs(e1.X - e2.X) < 2 && (e1.Y == e2.Y)) || (Math.Abs(e1.Y - e2.Y) < 2 && (e1.X == e2.X));
  }

  int GetRandomColor()
  {    
    int rand = r.Next(colors.Count - 1);
    return rand;
  }

  public void GenerateNewElements()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (GetElement(i, j) == null)
        {
          var img = GameObject.Instantiate(ReferenceCell.transform.GetChild(0).gameObject).GetComponent<Image>();
          //img.GetComponent<Moving>().SetTarget(GetCell(i, j).transform);
          img.transform.SetParent(GetCell(i, j).transform);
          BoardField e = img.GetComponent<BoardField>();
          e.X = i;
          e.Y = j;
          e.ID = GetRandomColor();
          img.color = colors[e.ID];
          //img.transform.SetParent(GetCell(i, j).transform);
        }
      }
    }
  }

  void SetParent(GameObject obj, Transform newParent)
  {
    Debug.Log("SetParent!");
    StartCoroutine(SetParentCorutine(obj, newParent));
  }

  IEnumerator SetParentCorutine(GameObject obj, Transform newParent)
  {
    var oldParent = obj.transform.parent;
    //var root = oldParent.transform.parent;
    //obj.transform.SetParent(root);
    if (oldParent == null) // just generated object
    {
      //obj.GetComponent<Moving>().startPos = new Vector3(newParent.position.x, 650, newParent.position.z);
      //obj.GetComponent<Moving>().endPos = newParent.position;
    }
    else
    {
     // obj.GetComponent<Moving>().startPos = oldParent.position;
     // obj.GetComponent<Moving>().endPos = newParent.position;
      //obj.GetComponent<Moving>().startPos = newParent.worldToLocalMatrix.MultiplyVector(oldParent.TransformVector(oldParent.position));
      //obj.GetComponent<Moving>().endPos = newParent.worldToLocalMatrix.MultiplyVector(newParent.TransformVector(newParent.position));
    }

    obj.GetComponent<Animator>().SetTrigger("IsMoved");
    
    //Func<bool> comparer = delegate() { return obj.GetComponent<Moving>().IsAnimEnded; };
    yield return new WaitUntil(()=> { return (obj.transform.position - newParent.position).magnitude < 1.0f; });
    //yield return new WaitForEndOfFrame();
    obj.transform.SetParent(newParent);
  }
}
