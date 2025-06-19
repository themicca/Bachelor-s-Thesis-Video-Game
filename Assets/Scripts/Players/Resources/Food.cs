using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetFoodSprite();
    }
}
