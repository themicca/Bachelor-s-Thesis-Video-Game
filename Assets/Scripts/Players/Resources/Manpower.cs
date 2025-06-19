using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manpower : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetManpowerSprite();
    }
}
