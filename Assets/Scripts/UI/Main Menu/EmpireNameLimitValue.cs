using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmpireNameLimitValue : MonoBehaviour
{
    public void MaxCharacters()
    {
        int maxChars = 20;
        GetComponent<TMP_InputField>().characterLimit = maxChars;
    }

    public void SetName()
    {
        Settings.empireName = GetComponent<TMP_InputField>().text;
    }
}
