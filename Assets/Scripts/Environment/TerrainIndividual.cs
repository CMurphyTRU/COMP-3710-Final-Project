using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TerrainIndividual
{
    public TerrainDNA dna;
    public GameObject terrainGO;
    public TerrainGenerator generator;

    public GameObject creatureGO;

    public TerrainEvaluator evaluator;
}
