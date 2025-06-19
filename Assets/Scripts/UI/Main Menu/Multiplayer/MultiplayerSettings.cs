using UnityEngine;
using TMPro;

public class MultiplayerSettings : MonoBehaviour
{
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private TMP_InputField sizeInput;

    private void Awake()
    {
        if (LobbyManager.Instance.IsLobbyHost())
        {
            startGameButton.SetActive(true);
            sizeInput.interactable = true;
        }
    }
}
