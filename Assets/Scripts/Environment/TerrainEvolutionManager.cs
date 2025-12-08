using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Creatures;
using Unity.VisualScripting.FullSerializer;


public class TerrainEvolutionManager : MonoBehaviour
{
    [Header("Prefabs & Setup")]
    public GameObject terrainPrefab;
    public GameObject creaturePrefab;
    public int populationSize = 5;
    public float terrainSpacing = 50f;

    [Header("Evolution Settings")]
    public float evaluationDuration = 10f;
    public float mutationRate = 0.1f;
    [Range(0f, 0.5f)]
    public float eliteFraction = 0.1f;

    private List<TerrainIndividual> population = new();
    private List<TerrainDNA> currentDNAs = new();

    private CreatureDNA fixedCreatureDNA;  
    private int generationIndex = 0;
    private float generationTimer = 0f;

    private void Start()
    {
        
        fixedCreatureDNA = new CreatureDNA();

        
        currentDNAs.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            currentDNAs.Add(new TerrainDNA());
        }

        
        SpawnGeneration();
    }

    private void Update()
    {
        generationTimer += Time.deltaTime;

        if (generationTimer >= evaluationDuration)
        {
            
            EndGenerationAndEvolve();
        }
    }

    

    private void SpawnGeneration()
    {
        
        CleanupCurrentPopulation();
        population.Clear();

        for (int i = 0; i < currentDNAs.Count; i++)
        {
            TerrainDNA dna = currentDNAs[i];

            
            GameObject terrainGO = Instantiate(terrainPrefab);
            terrainGO.name = "Terrain Generation Number " + generationIndex + "_" + i;

            
            terrainGO.transform.position = new Vector3(21.6f, i * terrainSpacing, 0f);

            TerrainGenerator generator = terrainGO.GetComponent<TerrainGenerator>();
            generator.GenerateFromDNA(dna);

            
            GameObject creatureGO = Instantiate(creaturePrefab);
            

            
            var creatureGen = creatureGO.GetComponent<CreatureGenerator>();
            creatureGen.generateFromDNA(fixedCreatureDNA);

            
            Vector3 startPos = terrainGO.transform.position + new Vector3(-19f, 10f, 0f);
            creatureGO.transform.position = startPos;

            
            TerrainEvaluator eval = creatureGO.GetComponent<TerrainEvaluator>();
            eval.evaluationDuration = evaluationDuration;
            eval.ResetFitness();

            
            TerrainIndividual indiv = new TerrainIndividual
            {
                dna = dna,
                terrainGO = terrainGO,
                generator = generator,
                creatureGO = creatureGO,
                evaluator = eval
            };

            population.Add(indiv);
        }

        generationTimer = 0f;
        Debug.Log("Spawned Generation Number " + generationIndex);
    }

    private void CleanupCurrentPopulation()
    {
        foreach (var indiv in population)
        {
            if (indiv.creatureGO != null)
                Destroy(indiv.creatureGO);
            if (indiv.terrainGO != null)
                Destroy(indiv.terrainGO);
        }
    }

    

    private void EndGenerationAndEvolve()
    {
        
        float maxFitness = 0f;
        float totalFitness = 0f;
        float[] scores = new float[populationSize];

        for (int i = 0; i < population.Count; i++)
        {
            var indiv = population[i];
            float f = (indiv.evaluator != null) ? indiv.evaluator.Fitness : 0f;

            indiv.dna.fitness = f;
            scores[i] = indiv.dna.fitness;

            totalFitness += f;
            if (f > maxFitness) maxFitness = f;
        }

        float avg = 0;
        for (int i = 0; i < populationSize; i++)
        {
            avg += scores[i];
        }

        avg = avg / populationSize;
        Debug.Log("Generation Number "+ generationIndex + "Finished, Max Terrain Fitness Was " + maxFitness + ". The average fitness was " + avg);

        
        List<TerrainDNA> nextDNAs = EvolveTerrains(currentDNAs, totalFitness);

        
        currentDNAs = nextDNAs;
        generationIndex++;

        

        SpawnGeneration();
    }

    

    private List<TerrainDNA> EvolveTerrains(List<TerrainDNA> oldPopulation, float totalFitness)
    {
        List<TerrainDNA> next = new List<TerrainDNA>();

        if (oldPopulation.Count == 0)
            return next;

        int eliteCount = Mathf.Max(1, Mathf.RoundToInt(oldPopulation.Count * eliteFraction));

        
        List<TerrainDNA> sorted = new List<TerrainDNA>(oldPopulation);
        sorted.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        
        bool allZero = totalFitness <= 0.0001f;

        
        for (int i = 0; i < eliteCount && i < sorted.Count; i++)
        {
            TerrainDNA eliteCopy = CopyDNA(sorted[i]);
            next.Add(eliteCopy);
        }

        
        while (next.Count < oldPopulation.Count)
        {
            TerrainDNA parentA;
            TerrainDNA parentB;

            if (allZero)
            {
                
                parentA = sorted[Random.Range(0, sorted.Count)];
                parentB = sorted[Random.Range(0, sorted.Count)];
            }
            else
            {
                parentA = RouletteSelect(sorted, totalFitness);
                parentB = RouletteSelect(sorted, totalFitness);
            }

            TerrainDNA child = TerrainDNA.Crossover(parentA, parentB);
            child.Mutate(mutationRate);
            next.Add(child);
        }

        return next;
    }

    private TerrainDNA RouletteSelect(List<TerrainDNA> population, float totalFitness)
    {
        float r = Random.Range(0f, totalFitness);
        float cumulative = 0f;

        foreach (var dna in population)
        {
            cumulative += dna.fitness;
            if (cumulative >= r)
                return dna;
        }

        
        return population[population.Count - 1];
    }

    private TerrainDNA CopyDNA(TerrainDNA src)
    {
        TerrainDNA d = new TerrainDNA(false);
        d.pointCount = src.pointCount;
        d.maxHeightDelta = src.maxHeightDelta;
        d.smoothness = src.smoothness;
        d.groundColor = src.groundColor;
        d.fitness = 0f;

        if (src.relativeHeights != null)
        {
            d.relativeHeights = new float[src.relativeHeights.Length];
            for (int i = 0; i < src.relativeHeights.Length; i++)
                d.relativeHeights[i] = src.relativeHeights[i];
        }

        return d;
    }
}
