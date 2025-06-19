using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TooltipManager : MonoBehaviour
{
    private static TooltipManager instance;

    [SerializeField]private GameObject tooltip;
    
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance.tooltip.SetActive(false);
    }

    public static void EnterMouse(string description = "", string header = "Tooltip missing!", Dictionary<Resource, int> costs = null,
        KeyValuePair<Resource, int> production = new())
    {
        instance.tooltip.SetActive(true);
        instance.tooltip.GetComponent<Tooltip>().SetText(description, header, costs, production);
    }

    public static void ExitMouse()
    {
        if (instance != null && instance.tooltip != null) instance.tooltip.SetActive(false);
    }
}
