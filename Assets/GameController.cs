using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private int remainChoosing;

    private int idFirstChoosing;
    public int firstChoosingRowIndex;
    public int firstChoosingColIndex;

    private int idLastChoosing;
    private int lastChoosingRowIndex;
    private int lastChoosingColIndex;

    [SerializeField] ContentController contentController;

    [SerializeField] int[] list = {1,1,2,2,1,2};
    [SerializeField] int[] listControl = {1,1,2,2,1,2};
    [SerializeField] List<int> listId = new List<int>();

    private int row = 6;
    private int col = 6;
    [SerializeField] int[] rowLevel = {8, 10, 12};
    [SerializeField] int[] colLevel = {6, 8, 10 };

    [SerializeField] float[] timeLevel = { 200, 250, 300 };
    [SerializeField] float time = 200;

    private int remainingCouple;
    private int currentNumber;

    private int shuffeRemaining;


    // Start is called before the first frame update
    void Start()
    {
        StartGame();
        ListMaker();
        contentController.SpawItems(list,row);
    }

    private void Update()
    {
        time -= Time.deltaTime;
        UpdateSliderValue(time);
        if(time < 0)
        {
            GameOver(false);
        }
    }

    public void StartGame()
    {
        if (PlayerPrefs.GetInt("level") < 1)
            PlayerPrefs.SetInt("level", 1);

        row = rowLevel[PlayerPrefs.GetInt("level")-1];
        col = colLevel[PlayerPrefs.GetInt("level")-1];
        time = timeLevel[PlayerPrefs.GetInt("level") - 1];
        remainingCouple = row * col;
        shuffeRemaining = 3;
        listId.Clear();
        UpdateRemainingShuffe();
        SetSliderValue(time);
        currentNumber = 0;
        StartNewTurn();
        Time.timeScale = 1;
    }

    public void StartNewTurn()
    {
        currentNumber++;
        GetComponent<UIController>().UpdateCurrentNumber(currentNumber);
    }

    public void UserChooseItem(int idItem, int row, int col)
    {
        if (idItem == currentNumber)
        {
            contentController.HideItem(row, col);
            listControl[row * this.col + col] = 0;

            if (currentNumber == remainingCouple)
            {
                GameOver(true);
                return;
            }

            StartNewTurn();
        }
        else
        {
            GameOver(false);
        }
    }

    public void ListMaker()
    {
        int[] temp = new int[row * col];

        int value = 1;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                listId.Add(value++);
            }
        }

        for (int i = 0; i < row; i++)
        {
            for(int j = 0; j< col; j++)
            {
                int index = i * col + j;
                if (temp[index] != 0) continue;
                int random = GetRandomIndex(listId);
                int id = listId[random];
                temp[index] = id;
                listId[random] = 0;
            }
        }

        list = temp;
        listControl = temp;
    }

    public int GetRandomIndex (List<int> array)
    {
        List<int> remainingSlot = new List<int>();
        for(int i =0; i< array.Count;i++)
        {
            if(array[i]!=0)
            {
                remainingSlot.Add(i);
            }
        }
        if (remainingSlot.Count == 0) return 0;
        return remainingSlot[Random.Range(0, remainingSlot.Count)];
    }

    public bool Checking(int row1, int col1, int row2, int col2)
    {
        // Check with border
        if ((CheckWithLeftBorder(row1, col1) && CheckWithLeftBorder(row2, col2)) || (CheckWithRightBorder(row1, col1) && CheckWithRightBorder(row2, col2))
            || (CheckWithTopBorder(row1, col1) && CheckWithTopBorder(row2, col2)) || (CheckWithBottomBorder(row1, col1) && CheckWithBottomBorder(row2, col2)))
            return true;

        int indexFirst = (row1 * this.col + col1);
        int indexLast = (row2 * this.col + col2);

        //CheckVertical
        List<int> listThroughVertical = GetListThroughVertical(row1, col1, row2, col2);
        for (int it = 0; it < listThroughVertical.Count; it++)
        {
            if(CheckThroughHorizontal(listThroughVertical[it], col1, row1, indexFirst) && CheckThroughHorizontal(listThroughVertical[it], col2, row2, indexLast))
            {
                return true;
            }
        }

        //CheckHorizontal
        List<int> listThroughHorizontal = GetListThroughHorizontal(row1, col1, row2, col2);
        for (int it = 0; it < listThroughHorizontal.Count; it++)
        {
            if(CheckThroughVertical(listThroughHorizontal[it], row1, col1, indexFirst) && CheckThroughVertical(listThroughHorizontal[it], row2, col2, indexLast))
            {
                return true;
            }
        }

        return false;
    }

    #region Checking Logic
    public List<int> GetListThroughVertical(int row1, int col1, int row2, int col2)
    {
        List<int> list = new List<int>();
        int index1 = (row1 * this.col + col1);
        int index2 = (row2 * this.col + col2);
        for (int i = 0; i < this.col; i++)
        {
            if(CheckThroughVertical(row1, row2, i, index1, index2))
            {
                list.Add(i);
            }
        }

        return list;
    }

    public List<int> GetListThroughHorizontal(int row1, int col1, int row2, int col2)
    {
        List<int> list = new List<int>();
        int index1 = (row1 * this.col + col1);
        int index2 = (row2 * this.col + col2);
        for (int j = 0; j < this.row; j++)
        {
            if (CheckThroughHorizontal(col1, col2, j, index1, index2))
            {
                list.Add(j);
            }
        }

        return list;
    }

    public bool CheckThroughVertical(int row1, int row2, int col, int index1, int index2)
    {
        int start = row1;
        int end = row2;
        if(end < start)
        {
            start = row2;
            end = row1;
        }

        for(int j = start; j <= end;j++)
        {
            int index = j * this.col + col;

            if (index == index1 || index == index2) continue;
            if(listControl[index] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckThroughHorizontal(int col1, int col2, int row, int index1, int index2)
    {
        int start = col1;
        int end = col2;
        if (end < start)
        {
            start = col2;
            end = col1;
        }

        for (int i = start; i <= end; i++)
        {
            int index = row * this.col + i;

            if (index == index1 || index == index2) continue;
            if (listControl[index] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckThroughHorizontal(int col1, int col2, int row, int index0)
    {
        int start = col1;
        int end = col2;
        if (end < start)
        {
            start = col2;
            end = col1;
        }

        for (int i = start; i <= end; i++)
        {
            int index = row * this.col + i;

            if (index == index0) continue;
            if (listControl[index] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckThroughVertical(int row1, int row2, int col, int index0)
    {
        int start = row1;
        int end = row2;
        if (end < start)
        {
            start = row2;
            end = row1;
        }

        for (int j = start; j <= end; j++)
        {
            int index = j * this.col + col;

            if (index == index0) continue;
            if (listControl[index] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckWithLeftBorder(int row, int col)
    {
        int index = row * this.col + col;
        return CheckThroughHorizontal(0, col, row, index);
    }

    public bool CheckWithRightBorder(int row, int col)
    {
        int index = row * this.col + col;
        return CheckThroughHorizontal(col, this.col-1, row, index);
    }

    public bool CheckWithTopBorder(int row, int col)
    {
        int index = row * this.col + col;
        return CheckThroughVertical(0, row, col, index);
    }

    public bool CheckWithBottomBorder(int row, int col)
    {
        int index = row * this.col + col;
        return CheckThroughVertical(row, this.row-1, col, index);
    }


    public bool CheckBorderLine(int row1, int col1, int row2, int col2)
    {
        if(row1 == row2 && (row1 == 0 || row1 == row-1))
        {
            return true;
        }
        else if (col1 == col2 && (col1 == 0 || col1 == col - 1))
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Shuffe feature
    public void Shuffe()
    {
        List<int> remainingList = new List<int>();
        for(int i = 0; i < listControl.Length; i++)
        {
            if(listControl[i] != 0)
            {
                remainingList.Add(i);
            }
        }

        for (int i = 0; i < listControl.Length; i++)
        {
            if (listControl[i] != 0)
            {
                int switchIndex = remainingList[Random.Range(0, remainingList.Count - 1)];

                int temp = listControl[i];
                listControl[i] = listControl[switchIndex];
                listControl[switchIndex] = temp;
            }
        }

        list = listControl;
    }

    public void ShuffeAndRecreated()
    {
        shuffeRemaining--;
        if (shuffeRemaining < 0) return;
        UpdateRemainingShuffe();
        Shuffe();
        contentController.Reset();
        contentController.SpawItems(list, row);
        StartNewTurn();
    }
    #endregion

    public void GameOver(bool isWin)
    {
        GetComponent<UIController>().GameOver(isWin);
    }

    public void UpdateRemainingShuffe()
    {
        
    }

    public void UpdateSliderValue(float value)
    {
        GetComponent<UIController>().UpdateSliderValue(value);
    }

    public void SetSliderValue(float value)
    {
        GetComponent<UIController>().SetSlider(value);
    }
}
