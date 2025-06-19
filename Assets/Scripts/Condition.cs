using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Condition
{
    private string text;
    private bool status;
    private Sprite statusSprite;
    private GameObject conditionField;

    public Condition(string text, bool status)
    {
        this.text = text;
        Tooltip.GetConditionParent().SetActive(true);
        conditionField = Object.Instantiate(Tooltip.GetConditionField(), Tooltip.GetConditionParent().transform);
        SetStatus(status);
        conditionField.GetComponentInChildren<TMP_Text>().text = text;
    }

    public bool GetStatus() { return status; }
    public string GetText() { return text; }

    public void SetStatus(bool status)
    {
        if (status) { statusSprite = Tooltip.GetConditionYes(); }
        else { statusSprite = Tooltip.GetConditionNo(); }
        conditionField.GetComponentInChildren<ConditionStatusUI>().GetComponent<Image>().sprite = statusSprite;
        this.status = status;
    }

    public void Destroy ()
    {
        Object.Destroy(conditionField);
        if (Tooltip.GetConditionParent().transform.childCount == 1)
        {
            Tooltip.GetConditionParent().SetActive(false);
        }
    }
}
