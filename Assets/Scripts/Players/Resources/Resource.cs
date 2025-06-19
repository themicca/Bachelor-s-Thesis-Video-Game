using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumResources
{
    Money,
    Wood,
    Stone,
    Food,
    Ore,
    Manpower
}

public abstract class Resource
{
    public abstract Sprite GetSprite();
}
