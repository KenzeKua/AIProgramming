using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToMove : MonoBehaviour
{
    float startPosX;
    float startPosY;

    Vector3 mousePosition;

    bool isHeld = false;

    private void Update()
    {
        if (isHeld)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.localPosition = new Vector3(mousePosition.x - startPosX, mousePosition.y - startPosY, 0);
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startPosX = mousePosition.x - transform.localPosition.x;
            startPosY = mousePosition.y - transform.localPosition.y;

            isHeld = true;
        }
    }

    private void OnMouseUp()
    {
        isHeld = false;
    }
}
