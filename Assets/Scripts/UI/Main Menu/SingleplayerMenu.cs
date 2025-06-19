using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SingleplayerMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField sizeInput;
    [SerializeField] private TMP_InputField empireNameInput;
    [SerializeField] private Image playerColorInput;

    private void OnEnable()
    {
        sizeInput.text = Settings.gridSizeX.ToString();
        empireNameInput.text = Settings.empireName;
        playerColorInput.color = Settings.playerColor;
    }
}
