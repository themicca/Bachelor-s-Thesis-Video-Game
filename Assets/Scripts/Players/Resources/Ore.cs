using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetOreSprite();
    }
}
