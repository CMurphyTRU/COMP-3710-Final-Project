using UnityEditor.PackageManager;
using UnityEngine;

public static class MathUtils
{

    public static bool rollOdds(float percentage)
    {
        return Random.value <= percentage;
    }
    
}
