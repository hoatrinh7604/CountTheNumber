using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentController : MonoBehaviour
{
    [SerializeField] GameObject rowObject;
    [SerializeField] GameObject itemPrefab;

    public void SpawRow(int numberOfRow)
    {
        for(int i = 0; i<numberOfRow; i++)
        {
            GameObject row = Instantiate(rowObject, Vector3.zero, Quaternion.identity, transform);
            row.GetComponent<RowController>().indexOfRow = i;
        }
    }

    public void SpawItem(int idItem, int indexRow, int indexCol)
    {
        GameObject temp = transform.GetChild(indexRow).gameObject;
        GameObject item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, temp.transform);
        item.GetComponent<ItemController>().UpdatePos(indexRow, indexCol);
        item.GetComponent<ItemController>().SetID(idItem);
    }

    public void SpawItems(int[] arrayData, int numberOfRow)
    {
        SpawRow(numberOfRow);
        int numberOfCol = arrayData.Length / numberOfRow;
        for (int i = 0; i < numberOfRow; i++)
        {
            for(int j = 0; j < numberOfCol; j++)
            {
                SpawItem(arrayData[i*numberOfCol + j], i, j);
            }
        }
    }

    public void HideItem(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().Hide(true);
    }

    public void UnTicked(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().UnTicked();
    }

    public void ChangeSibling(int row, int col, int newRow, int newIndex)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.transform.SetParent(transform.GetChild(newRow));
        colParent.transform.SetSiblingIndex(newIndex);
        //colParent.GetComponent<ItemController>().UpdatePos(newRow, newIndex);
    }

    public void UpdatePosItems()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            for(int j = 0; j < transform.GetChild(i).transform.childCount; j++)
            {
                transform.GetChild(i).transform.GetChild(j).GetComponent<ItemController>().UpdatePos(i, j);
            }
        }
    }

    public void Reset()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject temp = transform.GetChild(i).gameObject;
            temp.transform.SetParent(null);
            Destroy(temp);
        }
    }
}
