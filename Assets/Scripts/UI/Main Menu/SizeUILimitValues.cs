using UnityEngine;
using TMPro;

public class SizeUILimitValues : MonoBehaviour
{
    public void MaxCharacters()
    {
        int maxChars = 3;
        GetComponent<TMP_InputField>().characterLimit = maxChars;
    }

    public void LimitSize()
    {
        int maxSize = 100;
        int minSize = 15;
        int input = int.Parse(GetComponent<TMP_InputField>().text);
        int size = Mathf.Clamp(input, minSize, maxSize);
        GetComponent<TMP_InputField>().text = size.ToString();
    }

    public void SetSize()
    {
        Settings.gridSizeY = int.Parse(GetComponent<TMP_InputField>().text);
        Settings.gridSizeX = int.Parse(GetComponent<TMP_InputField>().text);
    }
}
