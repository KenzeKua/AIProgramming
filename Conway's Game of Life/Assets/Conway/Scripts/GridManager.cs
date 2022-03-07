using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 41;
    public int cols = 21;
    static public int tempRows;
    static public int tempCols;
    bool mapSize = false;
    [SerializeField] GameObject pixelObject;

    GameObject[,] pixelArray;
    [SerializeField] bool[,] futureWorld;

    [SerializeField] bool playPause = false;
    [SerializeField] bool speedChange = true;
    [SerializeField] float timer = 0f;
    public float updateSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        pixelArray = new GameObject[rows, cols];
        futureWorld = new bool[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                pixelArray[r, c] = Instantiate(pixelObject, new Vector3((float)(r * 0.4), (float)(c * 0.4), 0), Quaternion.identity);
                futureWorld[r, c] = false;
            }
        }

        SetLife();
    }

    // Update is called once per frame
    void Update()
    {
        FutureChecker();

        if (playPause)
        {
            if (speedChange)
            {
                updateSpeed = 1f;
            }
            else
            {
                updateSpeed = 0.2f;
            }

            if (!mapSize)
            {
                tempRows = rows;
                tempCols = cols;
            }
            else
            {
                tempRows = rows / 2;
                tempCols = cols / 2;
            }

            timer += Time.deltaTime;
            if (timer >= updateSpeed)
            {
                timer = 0f;
                WorldSwitcher();
            }
        }
    }

    void SetLife()
    {
        pixelArray[2, 2].GetComponent<PixelObject>().SetBool(true);
        pixelArray[2, 3].GetComponent<PixelObject>().SetBool(true);
        pixelArray[2, 4].GetComponent<PixelObject>().SetBool(true);
        pixelArray[3, 1].GetComponent<PixelObject>().SetBool(true);
        pixelArray[3, 2].GetComponent<PixelObject>().SetBool(true);
        pixelArray[3, 3].GetComponent<PixelObject>().SetBool(true);
    }

    void FutureChecker()
    {
        for (int r = 0; r < tempRows; r++)
        {
            for (int c = 0; c < tempCols; c++)
            {
                // Checker
                int neighbours = 0;

                if (r - 1 >= 0 && c - 1 >= 0)
                {
                    if (pixelArray[r - 1, c - 1].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (r - 1 >= 0)
                {
                    if (pixelArray[r - 1, c].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (r - 1 >= 0 && c + 1 < tempCols)
                {
                    if (pixelArray[r - 1, c + 1].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (c - 1 >= 0)
                {
                    if (pixelArray[r, c - 1].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (c + 1 < tempCols)
                {
                    if (pixelArray[r, c + 1].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (r + 1 < rows && c - 1 >= 0)
                {
                    if (pixelArray[r + 1, c - 1].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (r + 1 < tempRows)
                {
                    if (pixelArray[r + 1, c].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }
                if (r + 1 < tempRows && c + 1 < tempCols)
                {
                    if (pixelArray[r + 1, c + 1].GetComponent<PixelObject>().GetBool())
                    {
                        neighbours++;
                    }
                }

                // Conway's Rules
                if (pixelArray[r, c].GetComponent<PixelObject>().GetBool() == true && neighbours < 2)
                {
                    futureWorld[r, c] = false;
                }
                else if (pixelArray[r, c].GetComponent<PixelObject>().GetBool() == true && (neighbours == 2 || neighbours == 3))
                {
                    futureWorld[r, c] = true;
                }
                else if (pixelArray[r, c].GetComponent<PixelObject>().GetBool() == true && neighbours > 3)
                {
                    futureWorld[r, c] = false;
                }
                else if (pixelArray[r, c].GetComponent<PixelObject>().GetBool() == false && neighbours == 3)
                {
                    futureWorld[r, c] = true;
                }
            }
        }
    }

    void WorldSwitcher()
    {
        for (int r = 0; r < tempRows; r++)
        {
            for (int c = 0; c < tempCols; c++)
            {
                pixelArray[r, c].GetComponent<PixelObject>().SetBool(futureWorld[r, c]);
            }
        }
    }

    public void SetPlayPause(bool playBoolean)
    {
        playPause = playBoolean;
    }
    public void ChangeSpeed(bool fastSlow)
    {
        speedChange = fastSlow;
    }
    public void ClearMap()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                pixelArray[r, c].GetComponent<PixelObject>().SetBool(false) ;
                futureWorld[r, c] = false;
            }
        }
    }
    public void MapSize(bool bigSmall)
    {
        mapSize = bigSmall;

        if (bigSmall)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (r < rows / 2 && c < cols / 2)
                    {
                        pixelArray[r, c].SetActive(true);
                    }
                    else
                    {
                        pixelArray[r, c].SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    pixelArray[r, c].SetActive(true);
                }
            }
        }
    }

    public void QuitLife()
    {
        Application.Quit();
    }
}
