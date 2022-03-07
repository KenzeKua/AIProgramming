using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public bool isPlaying = true;
    public bool isDiagonal = true;

    [SerializeField] int rows = 20;
    [SerializeField] int cols = 20;
    [SerializeField] float weight = 1;

    [SerializeField] int startingXPosition = 3;
    [SerializeField] int startingYPosition = 3;
    [SerializeField] int destinationXPosition = 11;
    [SerializeField] int destinationYPosition = 11;

    [SerializeField] bool reachedEnd = false;

    int[] directionRows = { -1, 1, 0, 0 };
    int[] directionCols = { 0, 0, 1, -1 };
    int[] diagonalRows = { 1, -1, 1, -1 };
    int[] diagonalCols = { 1, -1, -1, 1 };

    // Variables
    Queue<int> openX;
    Queue<int> openY;
    int[,] grid;
    float[,] gCost;
    float[,] hCost;
    float[,] fCost;
    bool[,] scan;
    bool[,] visited;
    bool[,] inOpen;

    // Choose smallest variables
    Queue<int> smallX;
    Queue<int> smallY;
    int tempSmallX = 0;
    int tempSmallY = 0;

    // Parents
    int[] parentX;
    int[] parentY;
    Queue<int> openParentX;
    Queue<int> openParentY;

    // Path maker variables
    int tempX = 0;
    int tempY = 0;
    int tempPX = 0;
    int tempPY = 0;

    // Current update
    int currentRow = 0;
    int currentCol = 0;
    int nextRow = 0;
    int nextCol = 0;

    // Pixel instantiate
    [SerializeField] GameObject pixelObject;
    GameObject[,] pixelArray;

    // Update control
    [SerializeField] float timer = 0f;
    [SerializeField] float updateSpeed = 1f;

    void Start()
    {
        InitializeScript();
    }

    void Update()
    {
        Reader();
        Draw();

        if (isPlaying)
        {
            if (!reachedEnd)
            {
                timer += Time.deltaTime;
                if (timer >= updateSpeed)
                {
                    timer = 0f;
                    CheckNeighbour();
                }
            }
            else
            {
                Pathing();
                Draw();
            }
        }
    }

    // Initialize grid
    public void InitializeScript()
    {
        // Variables
        openX = new Queue<int>();
        openY = new Queue<int>();
        grid = new int[rows, cols];
        gCost = new float[rows, cols];
        hCost = new float[rows, cols];
        fCost = new float[rows, cols];
        scan = new bool[rows, cols];
        visited = new bool[rows, cols];
        inOpen = new bool[rows, cols];
        pixelArray = new GameObject[rows, cols];

        // Choose smallest variables
        smallX = new Queue<int>();
        smallY = new Queue<int>();

        // Parent variables
        parentX = new int[rows];
        parentY = new int[cols];
        openParentX = new Queue<int>();
        openParentY = new Queue<int>();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[r, c] = 0;
                gCost[r, c] = Mathf.Infinity;
                hCost[r, c] = Mathf.Infinity;
                fCost[r, c] = Mathf.Infinity;
                scan[r, c] = false;
                inOpen[r, c] = false;
                pixelArray[r, c] = Instantiate(pixelObject, new Vector3((float)(r * 0.3), (float)(c * 0.3), 0), Quaternion.identity);
            }
        }

        // 0 Tile
        // 1 Obstacle
        // 2 Start point
        // 3 End point
        // 4 Path

        // Set starting node
        grid[startingXPosition, startingYPosition] = 2;
        pixelArray[startingXPosition, startingYPosition].GetComponent<Pixel>().SetStart(true);
        gCost[startingXPosition, startingYPosition] = 0;
        scan[startingXPosition, startingYPosition] = true;
        openX.Enqueue(startingXPosition);
        openY.Enqueue(startingYPosition);

        // Set destination
        grid[destinationXPosition, destinationYPosition] = 3;
        pixelArray[destinationXPosition, destinationYPosition].GetComponent<Pixel>().SetEnd(true);

        // Set obstacle
        grid[10, 11] = 1;
        pixelArray[10, 11].GetComponent<Pixel>().SetObstacle(true);
        grid[10, 10] = 1;
        pixelArray[10, 10].GetComponent<Pixel>().SetObstacle(true);
        grid[11, 10] = 1;
        pixelArray[11, 10].GetComponent<Pixel>().SetObstacle(true);
    }

    int Heuristics(int nodeX, int nodeY)
    {
        int hValue = (Mathf.Abs(nodeX - destinationXPosition) + Mathf.Abs(nodeY - destinationYPosition));

        return (10 * hValue);
    }

    void ChooseSmallest()
    {
        if (openX.Count == 1)
        {
            currentRow = openX.Peek();
            currentCol = openY.Peek();
            openX.Dequeue();
            openY.Dequeue();
        }
        else if (openX.Count > 1)
        {
            tempSmallX = openX.Peek();
            tempSmallY = openY.Peek();
            openX.Dequeue();
            openY.Dequeue();

            while (openX.Count != 0)
            {
                if (fCost[tempSmallX, tempSmallY] < fCost[openX.Peek(), openY.Peek()]
                   || fCost[tempSmallX, tempSmallY] == fCost[openX.Peek(), openY.Peek()])
                {
                    smallX.Enqueue(openX.Peek());
                    smallY.Enqueue(openY.Peek());
                    openX.Dequeue();
                    openY.Dequeue();
                }
                else
                {
                    smallX.Enqueue(tempSmallX);
                    smallY.Enqueue(tempSmallY);
                    tempSmallX = openX.Peek();
                    tempSmallY = openY.Peek();
                    openX.Dequeue();
                    openY.Dequeue();
                }
            }
            if (openX.Count == 0)
            {
                currentRow = tempSmallX;
                currentCol = tempSmallY;
            }
        }

        // Reset the open list
        while (smallX.Count != 0)
        {
            openX.Enqueue(smallX.Peek());
            openY.Enqueue(smallY.Peek());
            smallX.Dequeue();
            smallY.Dequeue();
        }
    }

    void CheckNeighbour()
    {
        ChooseSmallest();

        scan[currentRow, currentCol] = true;
        inOpen[currentRow, currentCol] = false;

        for (int i = 0; i < 4; i++)
        {
            nextRow = currentRow + directionRows[i];
            nextCol = currentCol + directionCols[i];

            if (nextRow < 0 || nextCol < 0)
            {
                continue;
            }
            else if (nextRow >= rows || nextCol >= cols)
            {
                continue;
            }
            else if (grid[nextRow, nextCol] == 1)
            {
                continue;
            }
            else if (!inOpen[nextRow, nextCol] && !scan[nextRow, nextCol])
            {
                openX.Enqueue(nextRow);
                openY.Enqueue(nextCol);
                inOpen[nextRow, nextCol] = true;
                visited[nextRow, nextCol] = true;
                gCost[nextRow, nextCol] = gCost[currentRow, currentCol] + 10;
                hCost[nextRow, nextCol] = Heuristics(nextRow, nextCol);
                fCost[nextRow, nextCol] = gCost[nextRow, nextCol] + (weight * hCost[nextRow, nextCol]);
                parentX[nextRow] = currentRow;
                parentY[nextCol] = currentCol;
            }
            else if ((gCost[currentRow, currentCol] + 10) < gCost[nextRow, nextCol])
            {
                    parentX[nextRow] = currentRow;
                    parentY[nextCol] = currentCol;
                    gCost[nextRow, nextCol] = gCost[currentRow, currentCol] + 10;
                    fCost[nextRow, nextCol] = gCost[nextRow, nextCol] + (weight * hCost[nextRow, nextCol]);
            }
        }
        if (isDiagonal)
        {
            for (int i = 0; i < 4; i++)
            {
                nextRow = currentRow + diagonalRows[i];
                nextCol = currentCol + diagonalCols[i];

                if (nextRow < 0 || nextCol < 0)
                {
                    continue;
                }
                else if (nextRow >= rows || nextCol >= cols)
                {
                    continue;
                }
                else if (grid[nextRow, nextCol] == 1)
                {
                    continue;
                }
                else if (!inOpen[nextRow, nextCol] && !scan[nextRow, nextCol])
                {
                    openX.Enqueue(nextRow);
                    openY.Enqueue(nextCol);
                    inOpen[nextRow, nextCol] = true;
                    visited[nextRow, nextCol] = true;
                    gCost[nextRow, nextCol] = gCost[currentRow, currentCol] + 14;
                    hCost[nextRow, nextCol] = Heuristics(nextRow, nextCol);
                    fCost[nextRow, nextCol] = gCost[nextRow, nextCol] + (weight * hCost[nextRow, nextCol]);
                    parentX[nextRow] = currentRow;
                    parentY[nextCol] = currentCol;
                }
                else if ((gCost[currentRow, currentCol] + 14) < gCost[nextRow, nextCol])
                {
                    parentX[nextRow] = currentRow;
                    parentY[nextCol] = currentCol;
                    gCost[nextRow, nextCol] = gCost[currentRow, currentCol] + 14;
                    fCost[nextRow, nextCol] = gCost[nextRow, nextCol] + (weight * hCost[nextRow, nextCol]);
                }
            }
        }
        if (grid[currentRow, currentCol] == 3)
        {
            scan[currentRow, currentCol] = true;
            reachedEnd = true;
        }
    }

    void Reader()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (pixelArray[r, c].GetComponent<Pixel>().GetObstacle())
                {
                    grid[r, c] = 1;
                }
                if (!pixelArray[r, c].GetComponent<Pixel>().GetObstacle() &&
                    !pixelArray[r, c].GetComponent<Pixel>().GetStart() &&
                       !pixelArray[r, c].GetComponent<Pixel>().GetEnd())
                {
                    grid[r, c] = 0;
                }
                if (pixelArray[r, c].GetComponent<Pixel>().GetStart())
                {
                    grid[r, c] = 2;
                }
                if (pixelArray[r, c].GetComponent<Pixel>().GetEnd())
                {
                    grid[r, c] = 3;
                }
            }
        }
    }

    void Draw()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // Start point
                if (grid[r, c] == 2)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetGreen();
                }
                // End point
                else if (grid[r, c] == 3)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetRed();
                }
                // Path
                else if (grid[r, c] == 4)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetBlue();
                }
                // Scanned tile
                else if (grid[r, c] == 0 && scan[r, c] == true)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetCyan();
                }
                // Visited tile
                else if (grid[r, c] == 0 && visited[r, c] == true)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetGray();
                }
                // Empty tile
                else if (grid[r, c] == 0)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetWhite();
                }
                // Obstacle
                else if (grid[r, c] == 1)
                {
                    pixelArray[r, c].GetComponent<Pixel>().SetBlack();
                }
            }
        }
    }

    void Pathing()
    {
        while (openParentX.Count > 0)
        {
            tempX = openParentX.Peek();
            tempY = openParentY.Peek();
            openParentX.Dequeue();
            openParentY.Dequeue();

            tempPX = parentX[tempX];
            tempPY = parentY[tempY];

            if (tempPX == -1)
            {
                while (openParentX.Count != 0)
                {
                    openParentX.Dequeue();
                    openParentY.Dequeue();
                }
                return;
            }
            else
            {
                grid[tempX, tempY] = 4;
                openParentX.Enqueue(tempPX);
                openParentY.Enqueue(tempPY);
            }
        }
    }

    public void SetPlaying(bool toPlay)
    {
        isPlaying = toPlay;
    }
    public void SetDiagonal(bool toDiagonal)
    {
        isDiagonal = toDiagonal;
    }
    public void SetX(int xValue)
    {
        startingXPosition = xValue;
    }
    public void SetY(int yValue)
    {
        startingYPosition = yValue;
    }
    public void EndX(int xValue)
    {
        destinationXPosition = xValue;
    }
    public void EndY(int yValue)
    {
        destinationYPosition = yValue;
    }
    public void WeightSetting(float heaviness)
    {
        weight = heaviness;
    }
    public void RowsSet(int rowsOfRows)
    {
        rows = rowsOfRows;
    }
    public void ColsSet(int colsOfCols)
    {
        cols = colsOfCols;
    }
    public void SetActivity(bool activity)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                pixelArray[r, c].SetActive(activity);
            }
        }
    }
}