using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class ColorRule
{
    public string Name;

    public int FirstColor;
    public int SecondColor;

    public int ResultColor;
}

[CreateAssetMenu (menuName = "Genialogy/Color Gene")]
public class ColorScriptableObject : ScriptableObject
{
    public GeneType Type;

    public Color DefaultColor; //default resulting color if no rules match

    public List<Color> Values;

    public List<ColorRule> ColorRules;
}