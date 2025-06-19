using TMPro;
using UnityEngine;

public class MaxPlayersUI : MonoBehaviour
{
    public void SetMaxPlayers()
    {
        LobbyList.Instance.UpdateMaxPlayers(int.Parse(GetComponentInChildren<TMP_Text>().text));
    }
}
