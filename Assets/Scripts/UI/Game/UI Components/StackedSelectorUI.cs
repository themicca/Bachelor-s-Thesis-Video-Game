using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedSelectorUI : MonoBehaviour
{
    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out UnitBookmarkUI bookmark))
            {
                GameManager.RemoveBookmark(bookmark);
                Destroy(child);
            }
        }
    }
}
