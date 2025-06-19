using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetStoneSprite();
    }
}
