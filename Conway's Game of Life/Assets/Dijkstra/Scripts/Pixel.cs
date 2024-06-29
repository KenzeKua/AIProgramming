using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    SpriteRenderer objectSR;
    bool isObstacle = false;
    bool isStart = false;
    bool isEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        objectSR = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        if (!isStart && !isEnd)
        {
            isObstacle = !isObstacle;
        }
    }

    // Empty tile
    public void SetWhite()
    {
        objectSR.color = Color.white;
    }
    // Start point
    public bool GetStart()
    {
        return isStart;
    }
    public void SetStart(bool boolean)
    {
        isStart = boolean;
    }
    public void SetGreen()
    {
        objectSR.color = Color.green;
    }
    // End point
    public bool GetEnd()
    {
        return isEnd;
    }
    public void SetEnd(bool boolean)
    {
        isEnd = boolean;
    }
    public void SetRed()
    {
        objectSR.color = Color.red;
    }
    public void SetBlue()
    {
        objectSR.color = Color.blue;
    }
    // Set obstacle
    public void SetBlack()
    {
        objectSR.color = Color.black;
    }
    public bool GetObstacle()
    {
        return isObstacle;
    }
    public void SetObstacle(bool boolean)
    {
        isObstacle = boolean;
    }
    // Set scan tile
    public void SetCyan()
    {
        objectSR.color = Color.cyan;
    }
    // Set visited tile
    public void SetGray()
    {
        objectSR.color = Color.gray;
    }
}
