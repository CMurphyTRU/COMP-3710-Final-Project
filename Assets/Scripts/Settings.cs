using Assets.Scripts.Crossover;
using System.Runtime.CompilerServices;
using UnityEngine;

static public class Settings
{
    public static int pathSize = 7;
    public static int creatureFeatures = 3;
    public static float crossoverRate = 0.95f;
    public static float uniformCrossoverChance = 0.5f;
    public static CrossoverFunction crossoverFunction = new UniformCrossover();
    public static float mutationRate = 0.1f;
}
