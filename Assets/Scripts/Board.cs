using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

public class Board : MonoBehaviour
{
  public int Width = 8;
  public int Height = 8;
  public Cell[,] cells { get; private set; }

  private void Awake()
  {
    cells = new Cell[Width, Height];
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        cells[i, j] = transform.GetChild(i * Width + j).GetComponent<Cell>();
      }
    }
  }

  public void Generate()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (!cells[i, j].IsEmpty)
          continue;

        var item = ItemFactory.Instance.Create(i, j);
        //calculate start position for new elements instead of "100"
        item.transform.position = cells[0, j].transform.position + new Vector3(0, 100, 0);
        item.transform.SetParent(cells[i, j].transform);
      }
    }
  }

  /*public Item GetElement(int i, int j)
  {
    return GetCell(i, j)?.Item;
  }

  Cell GetCell(int i, int j)
  {
    if (i < 0 || i >= Width || j < 0 || j >= Height)
      return null;
    return gameObject.transform.GetChild(i * Width + j).GetComponent<Cell>();
  }*/

  public bool HasElementWithStatus(String name, bool val)
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (cells[i, j].IsEmpty)
          continue;
        if (cells[i, j].Item.Animator.GetBool(name) == val)
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
        if (cells[i, j].IsEmpty)
          return true;
      }
    }
    return false;
  }

  public bool HasMatchThree()
  {
    return GetMatchThreeElements().Count != 0;
  }

  public List<Item> GetMatchThreeElements()
  {
    var res = new List<Item>();

    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (cells[i, j].IsEmpty)
          continue;
        if (res.Contains(cells[i, j].Item))
          continue;
        res.AddRange(GetMatchThreeElements(cells[i, j].Item));
      }
    }
    return res;
  }

  public void SwapElements(Item first, Item second)
  {
    Transform temp = first.transform.parent;
    first.transform.SetParent(second.transform.parent);
    second.transform.SetParent(temp);
    SwapIndexes(first, second);
  }

  void SwapIndexes(Item element, Item other)
  {
    var e1 = element.GetComponent<Item>();
    var e2 = other.GetComponent<Item>();
    int tX = e1.X;
    int tY = e1.Y;

    e1.X = e2.X;
    e1.Y = e2.Y;
    e2.X = tX;
    e2.Y = tY;
  }

  public List<Item> GetMatchThreeElements(Item item)
  {
    var result = new List<Item>();
    result.AddRange(GetElementsFromLine(item));
    result.AddRange(GetElemenetsFromColumn(item));

    if (result.Count != 0)
      result.Add(item);

    return result;
  }

  List<Item> GetElementsFromLine(Item ge)
  {
    var result = new List<Item>();

    for (int i = ge.X - 1; i >= 0; --i)
    {
      var go = cells[i, ge.Y].Item;
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    for (int i = ge.X + 1; i < Width; ++i)
    {
      var go = cells[i, ge.Y].Item;
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    if (result.Count < 2)
      return new List<Item>();
    return result;
  }

  List<Item> GetElemenetsFromColumn(Item ge)
  {
    var result = new List<Item>();

    for (int i = ge.Y + 1; i < Height; ++i)
    {
      var go = cells[ge.X, i].Item;
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    for (int i = ge.Y - 1; i >= 0; --i)
    {
      var go = cells[ge.X, i].Item;
      if (!IsSameElements(go, ge))
        break;
      result.Add(go);
    }
    if (result.Count < 2)
      return new List<Item>();
    return result;
  }

  bool IsSameElements(Item go, Item ge)
  {
    if (go == null)
      return false;
    if (go.ID != ge.ID)
    {
      return false;
    }
    return true;
  }

  List<Item> GetAbove(int row, int col)
  {
    var result = new List<Item>();
    for (int i = row; i >= 0; --i)
    {
      var go = cells[i, col].Item;
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
        if (cells[i, j].IsEmpty)
        {
          var objects = GetAbove(i, j);
          if (objects.Count == 0)
            break;
          int k = i;
          foreach (var obj in objects)
          {
            obj.GetComponent<LayoutElement>().ignoreLayout = true;
            obj.transform.SetParent(cells[k, j].transform);
            var ge = obj.GetComponent<Item>();
            ge.X = k;
            ge.Y = j;
            k--;
          }
          break;
        }
      }
    }
  }

  public bool IsNeighbours(Item first, Item second)
  {
    return Abs(first.X - second.X) < 2 && (first.Y == second.Y) || Abs(first.Y - second.Y) < 2 && (first.X == second.X);
  }

  bool CheckStep(Item first, Item second)
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

  public List<Item> GetCombination()
  {
    var res = new List<Item>();
    for (int i = 0; i < Width - 1; ++i)
    {
      for (int j = 0; j < Height - 1; ++j)
      {
        if (CheckStep(cells[i, j].Item, cells[i + 1, j].Item))
        {
          res.Add(cells[i, j].Item);
          res.Add(cells[i + 1, j].Item);
          return res;
        }
        if (CheckStep(cells[i, j].Item, cells[i, j + 1].Item))
        {
          res.Add(cells[i, j].Item);
          res.Add(cells[i, j + 1].Item);
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
        if (!cells[i, j].IsEmpty)
          Destroy(cells[i, j].Item);
      }
    }
  }

  public List<Item> GetAround(Item bf, int count = 8)
  {
    var res = new List<Item>();
    for (int i = bf.X - 1; i <= bf.X + 1; ++i)
    {
      for (int j = bf.Y - 1; j <= bf.Y + 1; ++j)
      {
        if (i == bf.X && j == bf.Y)
          continue;
        if (i < 0 || i >= Width || j < 0 || j >= Height)
          continue;
        var go = cells[i, j].Item;
        if (go != null && go.Animator.GetBool("Destroyed") == false)
          res.Add(go);
      }
    }
    return res;
  }

  public List<Item> GetRandomElements(int count = 8)
  {
    var res = new List<Item>();
    for (int i = 0; i < count;)
    {
      var x = UnityEngine.Random.Range(0, Width);
      var y = UnityEngine.Random.Range(0, Height);
      var element = cells[x, y].Item;
      if (res.Contains(element) || element == null)
        continue;
      if (!element.Animator.GetBool("Destroyed"))
        res.Add(element);
      ++i;
    }

    return res;
  }

  public void RecolorElements(Item item)
  {
    var list = GetRandomElements(7);
    list.Add(item);
    foreach (var it in list)
    {
      it.Recolor();
    }
  }
}
