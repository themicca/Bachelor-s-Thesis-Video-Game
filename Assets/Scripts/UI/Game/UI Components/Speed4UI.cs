using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Speed4UI : MonoBehaviour, IHoverTooltip
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsHost) ChangeSpeed();
        });

        if (!NetworkManager.Singleton.IsHost)
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void CreateContent(ref string header, ref string description, ref Dictionary<Resource, int> costs,
        ref KeyValuePair<Resource, int> production, ref List<Condition> conditions)
    {
        header = "Speed 4";
        if (!NetworkManager.Singleton.IsHost) { description = "Only host can change the time speed."; }
    }

    public void ChangeSpeed()
    {
        Clock.SetGameSpeed(13);
        Utilities.SetSpeedSelected(GetComponent<Button>());
    }
}
