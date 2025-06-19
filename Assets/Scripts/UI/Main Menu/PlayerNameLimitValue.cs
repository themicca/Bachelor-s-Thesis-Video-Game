using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerNameLimitValue : MonoBehaviour
{
    private void Awake()
    {
        LobbyManager.Instance.UpdatePlayerName(GetComponent<TMP_InputField>().text);
    }

    public void MaxCharacters()
    {
        int maxChars = 20;
        GetComponent<TMP_InputField>().characterLimit = maxChars;
    }

    public void SetName()
    {
        LobbyManager.Instance.UpdatePlayerName(GetComponent<TMP_InputField>().text);
    }
}
