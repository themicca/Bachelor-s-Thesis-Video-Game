using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetWoodSprite();
    }
}
