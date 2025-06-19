using UnityEngine;
using UnityEngine.UI;

public class ColorChooserButton : MonoBehaviour
{
    [SerializeField] private GameObject colorChooser;
    private static ColorChooserButton instance;

    private void Awake()
    {
        instance = this;
    }

    public static void ChangeColor(Color color)
    {
        instance.GetComponent<Image>().color = color;
        Settings.playerColor = color;
    }

    public void ActivateColorChooser()
    {
        colorChooser.SetActive(!colorChooser.activeInHierarchy);
    }
}
