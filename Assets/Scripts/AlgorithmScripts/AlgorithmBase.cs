using UnityEngine;

public abstract class AlgorithmBase : ScriptableObject
{
    public abstract void Apply(TilemapStructure tilemap);
}