using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : Resource
{
    public override Sprite GetSprite()
    {
        return Prefabs.GetPopulationSprite();
    }
}
