using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Creatures;
using Unity.VisualScripting.FullSerializer;

public class TerrainEvolutionManager : MonoBehaviour
{
    [Header("Prefabs & Setup")]
    public GameObject terrainPrefab;         // terrain object to spawn
    public GameObject creaturePrefab;        // creature object to spawn on each terrain
    public int populationSize = 5;           // number of terrains per generation
    public float terrainSpacing = 50f;       // vertical spacing between terrains

    [Header("Evolution Settings")]
    public float evaluationDuration = 10f;   // how long each generation runs
    public float mutationRate = 0.1f;        // mutation chance
    [Range(0f, 0.5f)]
    public float eliteFraction = 0.1f;       // fraction of best DNA to keep unchanged

    private List<TerrainIndividual> population = new();  // active generation
    private List<TerrainDNA> currentDNAs = new();        // DNA used to spawn terrains

    private CreatureDNA fixedCreatureDNA;    // one creature DNA used for all terrains
    private int generationIndex = 0;         // current generation number
    private float generationTimer = 0f;      // tracks time per generation

    private void Start()
    {
        // create a single creature DNA that all creatures will use
        fixedCreatureDNA = new CreatureDNA();

        // generate initial random terrain DNAs
        currentDNAs.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            currentDNAs.Add(new TerrainDNA());
        }

        // spawn the first generation
        SpawnGeneration();
    }

    private void Update()
    {
        generationTimer += Time.deltaTime;

        // when enough time passes, end the generation
        if (generationTimer >= evaluationDuration)
        {
            EndGenerationAndEvolve();
        }
    }

    private void SpawnGeneration()
    {
        // remove previous generation
        CleanupCurrentPopulation();
        population.Clear();

        for (int i = 0; i < currentDNAs.Count; i++)
        {
            TerrainDNA dna = currentDNAs[i];

            // create terrain object
            GameObject terrainGO = Instantiate(terrainPrefab);
            terrainGO.name = "Terrain Generation Number " + generationIndex + "_" + i;

            // position terrain
            terrainGO.transform.position = new Vector3(21.6f, i * terrainSpacing, 0f);

            // generate terrain shape from DNA
            TerrainGenerator generator = terrainGO.GetComponent<TerrainGenerator>();
            generator.GenerateFromDNA(dna);

            // create creature object
            GameObject creatureGO = Instantiate(creaturePrefab);

            // generate creature from fixed DNA
            var creatureGen = creatureGO.GetComponent<CreatureGenerator>();
            creatureGen.generateFromDNA(fixedCreatureDNA);

            // set creature start position
            Vector3 startPos = terrainGO.transform.position + new Vector3(-19f, 10f, 0f);
            creatureGO.transform.position = startPos;

            // setup evaluator
            TerrainEvaluator eval = creatureGO.GetComponent<TerrainEvaluator>();
            eval.evaluationDuration = evaluationDuration;
            eval.ResetFitness();

            // save this individual
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
        // destroy all leftover creatures and terrains
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
        // collect fitness info
        float maxFitness = 0f;
        float totalFitness = 0f;
        float[] scores = new float[populationSize];

        for (int i = 0; i < population.Count; i++)
        {
            var indiv = population[i];
            float f = (indiv.evaluator != null) ? indiv.evaluator.Fitness : 0f;

            indiv.dna.fitness = f;
            scores[i] = f;

            totalFitness += f;
            if (f > maxFitness) maxFitness = f;
        }

        // compute average fitness
        float avg = 0;
        for (int i = 0; i < populationSize; i++)
            avg += scores[i];

        avg /= populationSize;

        Debug.Log("Generation Number " + generationIndex +
                  " Finished, Max Terrain Fitness Was " + maxFitness +
                  ". The average fitness was " + avg);

        // evolve DNA into next generation
        List<TerrainDNA> nextDNAs = EvolveTerrains(currentDNAs, totalFitness);

        // replace old DNA with new DNA
        currentDNAs = nextDNAs;
        generationIndex++;

        // spawn next generation
        SpawnGeneration();
    }

    private List<TerrainDNA> EvolveTerrains(List<TerrainDNA> oldPopulation, float totalFitness)
    {
        List<TerrainDNA> next = new List<TerrainDNA>();

        if (oldPopulation.Count == 0)
            return next;

        // number of elites to copy unchanged
        int eliteCount = Mathf.Max(1, Mathf.RoundToInt(oldPopulation.Count * eliteFraction));

        // sort by fitness (best first)
        List<TerrainDNA> sorted = new List<TerrainDNA>(oldPopulation);
        sorted.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        // detect if all fitness values are zero
        bool allZero = totalFitness <= 0.0001f;

        // add elites to next generation
        for (int i = 0; i < eliteCount && i < sorted.Count; i++)
        {
            TerrainDNA eliteCopy = CopyDNA(sorted[i]);
            next.Add(eliteCopy);
        }

        // fill the rest using crossover + mutation
        while (next.Count < oldPopulation.Count)
        {
            TerrainDNA parentA;
            TerrainDNA parentB;

            if (allZero)
            {
                // random selection if no fitness differences
                parentA = sorted[Random.Range(0, sorted.Count)];
                parentB = sorted[Random.Range(0, sorted.Count)];
            }
            else
            {
                // roulette wheel selection
                parentA = RouletteSelect(sorted, totalFitness);
                parentB = RouletteSelect(sorted, totalFitness);
            }

            // make a child
            TerrainDNA child = TerrainDNA.Crossover(parentA, parentB);
            child.Mutate(mutationRate);
            next.Add(child);
        }

        return next;
    }

    private TerrainDNA RouletteSelect(List<TerrainDNA> population, float totalFitness)
    {
        // pick random value between 0 and total fitness
        float r = Random.Range(0f, totalFitness);
        float cumulative = 0f;

        // accumulate fitness until threshold reached
        foreach (var dna in population)
        {
            cumulative += dna.fitness;
            if (cumulative >= r)
                return dna;
        }

        // fallback (should rarely hit)
        return population[population.Count - 1];
    }

    private TerrainDNA CopyDNA(TerrainDNA src)
    {
        // make a copy of DNA without randomizing
        TerrainDNA d = new TerrainDNA(false);
        d.pointCount = src.pointCount;
        d.maxHeightDelta = src.maxHeightDelta;
        d.smoothness = src.smoothness;
        d.groundColor = src.groundColor;
        d.fitness = 0f;

        // copy height values
        if (src.relativeHeights != null)
        {
            d.relativeHeights = new float[src.relativeHeights.Length];
            for (int i = 0; i < src.relativeHeights.Length; i++)
                d.relativeHeights[i] = src.relativeHeights[i];
        }

        return d;
    }
}
