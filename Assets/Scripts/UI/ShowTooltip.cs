using System.Collections.Generic;
using UnityEngine;

public class ShowTooltip : MonoBehaviour
{
    private string header;
    private string description;
    private Dictionary<Resource, int> costs;
    private KeyValuePair<Resource, int> production;
    private List<Condition> conditions;
    private static ShowTooltip instance;

    [SerializeField] private GameObject costField;

    private void Awake()
    {
        instance = this;
        description = "";
        header = "Tooltip missing!";
        costs = new Dictionary<Resource, int>();
        production = new KeyValuePair<Resource, int>();
        conditions = new List<Condition>();
    }

    public static void ShowTooltipToPlayer(GameObject tooltipObject)
    {
        instance.CreateContent(tooltipObject);
        TooltipManager.EnterMouse(instance.description, instance.header, instance.costs, instance.production);
    }

    public static void HideTooltip()
    {
        instance.header = "Tooltip missing!";
        instance.description = "";
        instance.production = new KeyValuePair<Resource, int>();
        instance.costs = new Dictionary<Resource, int>();

        foreach(Condition con in instance.conditions)
        {
            con.Destroy();
        }

        instance.conditions = new List<Condition>();
        if (Tooltip.GetConditionParent() != null) Tooltip.GetConditionParent().SetActive(false);

        if (instance.costField != null && instance.costField.transform.childCount > 0)
        {
            GameObject[] children = new GameObject[instance.costField.transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = instance.costField.transform.GetChild(i).gameObject;
            }
            foreach (GameObject child in children)
            {
                Destroy(child);
            }
        }
        
        TooltipManager.ExitMouse();
    }

    void CreateContent(GameObject tooltipObject)
    {
        if (tooltipObject.TryGetComponent(out IHoverTooltip tooltip)) tooltip.CreateContent(ref header, ref description, ref costs, ref production, ref conditions);
    }
}