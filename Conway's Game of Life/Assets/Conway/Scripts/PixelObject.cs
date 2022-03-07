using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelObject : MonoBehaviour
{
    public bool isAlive = false;
    Color objectColour;

    // Start is called before the first frame update
    void Start()
    {
        objectColour = this.gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            objectColour.a = 1f;
            this.gameObject.GetComponent<SpriteRenderer>().color = objectColour;
        }
        else if (!isAlive)
        {
            objectColour.a = 0f;
            this.gameObject.GetComponent<SpriteRenderer>().color = objectColour;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (isAlive)
            {
                isAlive = false;
            }
            else if (!isAlive)
            {
                isAlive = true;
            }
        }
    }

    public bool GetBool()
    {
        return isAlive;
    }
    public void SetBool(bool lifeState)
    {
        isAlive = lifeState;
    }
}
