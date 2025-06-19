using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private RectTransform bgRectTransform;
    [SerializeField] private RectTransform tooltipCanvasRT;

    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI desctiprionField;
    [SerializeField] private GameObject costField;
    [SerializeField] private GameObject productionField;

    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWrapLimit;

    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image costIcon;

    [SerializeField] private GameObject conditionField;
    [SerializeField] private GameObject conditionParent;
    [SerializeField] private Sprite conditionYes;
    [SerializeField] private Sprite conditionNo;

    private static Tooltip instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        bgRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SetPosition();
    }

    public void SetText(string toolTipText = "", string header = "Tooltip missing!", Dictionary<Resource, int> costs = null,
        KeyValuePair<Resource, int> production = new())
    {
        AddCostToTooltip(costs);
        AddProductionToTooltip(production);

        headerField.text = header;
        desctiprionField.text = toolTipText;

        headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));
        desctiprionField.gameObject.SetActive(!string.IsNullOrEmpty(toolTipText));

        int headerLength = headerField.text.Length;
        int contentLength = desctiprionField.text.Length;

        layoutElement.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;
        SetPosition();
    }

    private void AddCostToTooltip(Dictionary<Resource, int> costs)
    {
        if (costs == null)
        {
            return;
        }
        foreach(Resource resource in costs.Keys)
        {
            Image icon = Instantiate(costIcon, costField.transform);
            icon.sprite = resource.GetSprite();
            TextMeshProUGUI text = Instantiate(costText, costField.transform);
            text.text = costs[resource].ToString();
        }
    }

    private void AddProductionToTooltip(KeyValuePair<Resource, int> production)
    {
        if (production.Key == null)
        {
            productionField.gameObject.SetActive(false);
            return;
        }
        productionField.gameObject.SetActive(true);
        productionField.GetComponentInChildren<TMP_Text>().text = "+ " + production.Value.ToString();
        productionField.GetComponentInChildren<Image>().sprite = production.Key.GetSprite();
    }

    void SetPosition()
    {
        Vector2 anchoredPosition = Input.mousePosition / tooltipCanvasRT.localScale.x;
        bool offsetHorizontal = true;

        if (anchoredPosition.x + bgRectTransform.rect.width > tooltipCanvasRT.rect.width)
        {
            anchoredPosition.x = anchoredPosition.x - bgRectTransform.rect.width;
            offsetHorizontal = false;
        }
        if (anchoredPosition.y + bgRectTransform.rect.height > tooltipCanvasRT.rect.height)
        {
            anchoredPosition.y = anchoredPosition.y - bgRectTransform.rect.height;
            if (offsetHorizontal)
                anchoredPosition.x += 8 / tooltipCanvasRT.localScale.x;
        }

        bgRectTransform.anchoredPosition = anchoredPosition;
    }

    public static GameObject GetConditionField() { return instance.conditionField; }
    public static GameObject GetConditionParent() { return instance.conditionParent; }
    public static Sprite GetConditionYes() { return instance.conditionYes; }
    public static Sprite GetConditionNo() { return instance.conditionNo; }
}
