using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TerrainPopulationManager : MonoBehaviour
{
    [Header("Population Settings")]
    public int populationSize = 10;

    [Header("References")]
    public GameObject terrainPrefab;



    private List<TerrainDNA> populationDNA = new List<TerrainDNA>();
    private List<GameObject> activeTerrains = new List<GameObject>();

    void Start()
    {
        GenerateInitialPopulation();
        SpawnPopulation();
        EvaluatePopulation();
    }

    
    void GenerateInitialPopulation()
    {
        populationDNA.Clear();

        for (int i = 0; i < populationSize; i++)
        {
            TerrainDNA newDNA = new TerrainDNA();
            populationDNA.Add(newDNA);
        }
    }

    
    void SpawnPopulation()
    {
        ClearExistingTerrains();
        activeTerrains.Clear();

        float xOffset = 0f; 

        for (int i = 0; i < populationDNA.Count; i++)
        {
            GameObject terrain = Instantiate(terrainPrefab);

            
            terrain.transform.position = new Vector3(xOffset, 0, 0);

            TerrainGenerator generator = terrain.GetComponent<TerrainGenerator>();
            generator.GenerateFromDNA(populationDNA[i]);

            activeTerrains.Add(terrain);

            xOffset += 50f;
            
        }
    }

    void ClearExistingTerrains()
    {
        foreach (var t in activeTerrains)
        {
            Destroy(t);
        }
    }


    // Placeholder For Your Creature Fitness Evaluation
    //Replace this entire method with your creature fitness evaluation and change the method call in EvaluatePopulation()        
    float EvaluateTerrainFitness(TerrainDNA dna)
    {
        float totalRoughness = 0f;

        for (int i = 1; i < dna.relativeHeights.Length; i++)
        {
            totalRoughness += Mathf.Abs(dna.relativeHeights[i] - dna.relativeHeights[i - 1]);
        }

        return 1f / (1f + totalRoughness);
    }

    void EvaluatePopulation()
    {
        for (int i = 0; i < populationDNA.Count; i++)
        {
            float score = EvaluateTerrainFitness(populationDNA[i]);
            populationDNA[i].fitness = score;
            Debug.Log($"Terrain {i} Fitness: {score}");
        }
    }

    
}
