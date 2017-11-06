using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
  public GameObject ReferenceItem;
  public List<Color> colors = new List<Color>();
  public int Width { get; private set; }
  public int Height { get; private set; }

  public void Generate(int width, int height)
  {
    Width = width;
    Height = height;

    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (GetCell(i, j).transform.childCount != 0)
          continue;

        var item = GameObject.Instantiate(ReferenceItem);
        Image img = item.GetComponent<Image>();
        item.SetActive(true);
        BoardField e = img.GetComponent<BoardField>();
        e.X = i;
        e.Y = j;
        e.ID = GetRandomColor();
        img.color = colors[e.ID];

        var cell = GetCell(i, j);
        //calculate start position for new elements instead of "100"
        item.transform.position = GetCell(0, j).transform.position + new Vector3(0, 100, 0);
        item.transform.SetParent(cell.transform);
      }
    }
  }

  public GameObject GetElement(int i, int j)
  {
    var cell = GetCell(i, j);
    if (cell == null)
      return null;
    if (cell.transform.childCount > 0)
      return cell.transform.GetChild(0).gameObject;
    return null;
  }

  public bool HasElementWithStatus(String name, bool val)
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var ctrl = GetElement(i, j);
        if (ctrl == null)
          continue;
        if (ctrl.GetComponent<Animator>().GetBool(name) == val)
        {
          return true;
        }
      }
    }
    return false;
  }

  public bool HasEmptyCell()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var cell = GetCell(i, j);
        if (cell.transform.childCount == 0)
          return true;
      }
    }
    return false;
  }

  public bool HasMatchThree()
  {
    return GetMatchThreeElements().Count != 0;
  }

  public List<GameObject> GetMatchThreeElements()
  {
    var res = new List<GameObject>();

    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var go = GetElement(i, j);
        if (go == null)
          continue;
        if (res.Contains(go))
          continue;
        res.AddRange(GetMatchThreeElements(go));
      }
    }
    return res;
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

  GameObject GetCell(int i, int j)
  {
    if (i < 0 || i >= Width || j < 0 || j >= Height)
      return null;
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

  public void RemoveHoles()
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
            obj.GetComponent<LayoutElement>().ignoreLayout = true;
            obj.transform.SetParent(GetCell(k, j).transform);
            var ge = obj.GetComponent<BoardField>();
            ge.X = k;
            ge.Y = j;
            k--;
          }
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
    return UnityEngine.Random.Range(0, colors.Count);
  }

  bool CheckStep(GameObject first, GameObject second)
  {
    SwapElements(first, second);
    var list = GetMatchThreeElements(first);
    list.AddRange(GetMatchThreeElements(second));
    SwapElements(first, second);
    if (list.Count != 0)
    {
      return true;
    }
    return false;
  }

  public List<GameObject> GetCombination()
  {
    var res = new List<GameObject>();
    for (int i = 0; i < Width - 1; ++i)
    {
      for (int j = 0; j < Height - 1; ++j)
      {
        if (CheckStep(GetElement(i, j), GetElement(i + 1, j)))
        {
          res.Add(GetElement(i, j));
          res.Add(GetElement(i + 1, j));
          return res;
        }
        if (CheckStep(GetElement(i, j), GetElement(i, j + 1)))
        {
          res.Add(GetElement(i, j));
          res.Add(GetElement(i, j + 1));
          return res;
        }
      }
    }
    return res;
  }

  public void Clear()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var go = GetElement(i, j);
        if (go != null)
          Destroy(go);
      }
    }
  }

  public List<GameObject> GetAround(BoardField bf, int count = 8)
  {    
    var res = new List<GameObject>();
    for (int i = bf.X - 1; i <= bf.X + 1; ++i)
    {
      for (int j = bf.Y - 1; j <= bf.Y + 1; ++j)
      {
        if (i == bf.X && j == bf.Y)
          continue;
        var go = GetElement(i, j);
        if (go != null && go.GetComponent<Animator>().GetBool("Destroyed") == false)
          res.Add(go);
      }
    }
    return res;
  }

  public List<GameObject> GetRandomElements(int count = 8)
  {
    var res = new List<GameObject>();
    for (int i = 0; i < count;)
    {
      var x = UnityEngine.Random.Range(0, Width);
      var y = UnityEngine.Random.Range(0, Height);
      var element = GetElement(x, y);
      if (res.Contains(element) || element == null)
        continue;
      if (!element.GetComponent<Animator>().GetBool("Destroyed"))
        res.Add(element);
      ++i;
    }

    return res;
  }

  public void RecolorElements(BoardField bf)
  {
    var list = GetRandomElements(7);
    list.Add(bf.gameObject);

    foreach (var go in list)
    {
      var id = GetRandomColor();
      go.GetComponent<BoardField>().ID = id;
      go.GetComponent<Image>().color = colors[id];
    }
  }
}
