using UnityEngine;

using System.Collections.Generic;

public enum GeneType
{
    HEAD,
    BODY,
    LEG,
    HAT,
    COLOR
}

[System.Serializable]
public class GeneValue
{
    public string Name;

    public Sprite Image;
    public Sprite Outline;
}

[CreateAssetMenu (menuName = "Genialogy/Gene")]
public class GeneScriptableObject : ScriptableObject
{
    public GeneType Type;

    public List<GeneValue> Values;
}