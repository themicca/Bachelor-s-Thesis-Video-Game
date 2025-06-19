using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetMoneySprite();
    }
}
