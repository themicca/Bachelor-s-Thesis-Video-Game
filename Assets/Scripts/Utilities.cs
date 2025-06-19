using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Utilities
{
    private static Button s_speedSelected;
    private static GameObject s_mouseOverUIElement;

    public static Vector3Int OffsetToCube(Vector2Int offset)
    {
        var q = offset.x - (offset.y + (offset.y % 2)) / 2;
        var r = offset.y;
        return new Vector3Int(q, r, -q-r);
    }

    public static void GetTownCost(out int woodCost, out int stoneCost, out int foodCost, out int manpowerCost)
    {
        woodCost = 350; stoneCost = 400; foodCost = 500; manpowerCost = 2000;
    }

    public static bool CheckMouseOverUIElement()
    {
        return s_mouseOverUIElement != null;
    }

    public static void SetMouseOverUIElement(GameObject mouseElement) { s_mouseOverUIElement = mouseElement; }

    public static void ClearMouseOverUIElement() { s_mouseOverUIElement = null; }

    public static void SetSpeedSelected(Button speed)
    {
        if (speed == s_speedSelected) return;


        ColorBlock colors;
        if (s_speedSelected != null)
        {

            colors = s_speedSelected.colors;
            colors.normalColor = new Color32(88, 92, 96, 255);
            s_speedSelected.colors = colors;
        }
        s_speedSelected = speed;
        colors = s_speedSelected.colors;
        colors.normalColor = new Color32(0, 188, 43, 255);
        s_speedSelected.colors = colors;
    }

    public static void SpawnNetworkObject(GameObject gameObject)
    {
        if (NetworkManager.Singleton.IsHost && gameObject.TryGetComponent(out NetworkObject network))
        {
            network.Spawn();
        }
    }

    public static void ReparentNetworkObject(Transform parent, Transform child)
    {
        if (NetworkManager.Singleton.IsHost && parent.TryGetComponent(out NetworkObject _) && child.TryGetComponent(out NetworkObject _))
        {
            child.SetParent(parent, true);
        }
    }

    public static void ReparentNetworkObject(GameObject parent, GameObject child)
    {
        if(parent != null && child != null) ReparentNetworkObject(parent.transform, child.transform);
    }
}
