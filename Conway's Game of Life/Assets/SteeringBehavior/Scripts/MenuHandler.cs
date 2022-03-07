using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public CanvasGroup mainStuffs;
    public CanvasGroup sliders;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAlpha(float alpha)
    {
        mainStuffs.alpha = alpha;
        sliders.alpha = alpha;
    }
}
