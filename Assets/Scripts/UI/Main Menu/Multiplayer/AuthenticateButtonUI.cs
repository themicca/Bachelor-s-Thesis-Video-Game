using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateButtonUI : MonoBehaviour, IHoverTooltip
{
    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs, ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Autenticate and Sign in";
    }

    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(LobbyManager.Instance.GetPlayerName());
        });
    }
}