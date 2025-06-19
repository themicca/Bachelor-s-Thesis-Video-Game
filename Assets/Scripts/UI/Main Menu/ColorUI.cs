using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{
    public void Click()
    {
        ColorChooserButton.ChangeColor(GetComponent<Image>().color);
    }
}
