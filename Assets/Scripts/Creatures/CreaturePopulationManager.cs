using Assets.Scripts.Creatures;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class CreaturePopulationManager : MonoBehaviour
{
    public static int populationCount = 20;
    public static bool keepBest = true;
    public int bestIndex;
    public static int creaturesToKeep = Mathf.FloorToInt(populationCount * 0.5f);
    public List<CreatureDNA> populationDNA = new();
    public float[] cumulativeFitness;
    public List<GameObject> creatures = new();


    [SerializeField] public TerrainDNA terrain;
    [SerializeField] GameObject terrainPrefab;
    [SerializeField] GameObject terrainObject;
    [SerializeField] Vector3 goalPosition;

    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private GameObject flagPrefab;

    private Camera mainCamera;
    private float cameraScale = 30f;

    private float generationTime = 10f;
    private float timeElapsed = 0;
    private int generationId = 1;
    void Start()
    {
        prepareTerrain();
        initialPopulation();
        spawnCreatures();

        Debug.Log($"population = {populationCount}, Shapes Per Creature = {populationDNA[0].shapeCount}, Points Per Shape = {populationDNA[0].pointsPerShape}");
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > generationTime)
        {
            timeElapsed = 0;

            cumulativeFitness = evaluateFitness();
            roulette(creaturesToKeep);
            breed();

            generationId++;
            spawnCreatures();
        }
    }

    private void prepareTerrain()
    {
        mainCamera = Camera.main;
        mainCamera.orthographicSize = 30f;
        terrain = new TerrainDNA();
        terrainObject = Instantiate(terrainPrefab);
        terrainObject.GetComponent<TerrainGenerator>().GenerateFromDNA(terrain);
        terrainObject.transform.position += new Vector3(0, -cameraScale / mainCamera.aspect);
        goalPosition = terrainObject.GetComponent<PolygonCollider2D>().bounds.max;
        GameObject flag = Instantiate(flagPrefab);
        flag.transform.position = goalPosition - new Vector3(2, 0);
    }

    private void initialPopulation(string savedData = null)
    {
        if (savedData != null)
        {
            // Create from string provided and mutate until population filled
            return;
        }

        for (int i = 0; i < populationCount; i++)
        {
            populationDNA.Add(new CreatureDNA());
        }
    }

    // Empties creature game objects
    private void clearCreatures()
    {
        foreach(GameObject creature in creatures)
        {
            Destroy(creature);
        }
        creatures.Clear();
    }

    // Spawns the creature game objects based on the population dna list
    private void spawnCreatures()
    {
        clearCreatures();

        for (int i = 0; i < populationDNA.Count; i++)
        {
            GameObject createdCreature = Instantiate(creaturePrefab);
            createdCreature.name = $"Creature{i}";
            createdCreature.transform.position = new Vector3(-cameraScale, 2);
            CreatureGenerator creatureGenerator = createdCreature.GetComponent<CreatureGenerator>();
            creatureGenerator.goalPosition = goalPosition;
            creatureGenerator.generateFromDNA(populationDNA[i]);

            creatures.Add(createdCreature);
        }
    }

    /*
     * Determines the score of every creature and stores their fitness and also determines the highest one.
     * Generates a list of cumulative scores for roulette selection.
     */
    private float[] evaluateFitness()
    {
        float[] cumulativeFitness = new float[populationCount];
        int best = 0;
        float highestFitness = 0;
        int totalFinished = 0;

        for (int i = 0; i < populationCount; i++)
        {
            GameObject creature = creatures[i];
            CreatureGenerator generator = creature.GetComponent<CreatureGenerator>();
            CreatureEvaluator creatureEvaluator = generator.shapes[0].GetComponent<CreatureEvaluator>();
            float creatureFitness = creatureEvaluator.getFitness(generationTime);
            populationDNA[i].fitness = creatureFitness;

            cumulativeFitness[i] = creatureFitness;
            if (i > 0) cumulativeFitness[i] += cumulativeFitness[i - 1];


            if (creatureFitness > highestFitness)
            {
                best = i;
                highestFitness = creatureFitness;
            }

            if(creatureEvaluator.hasFinished)
            {
                totalFinished++;
            }
        }

        bestIndex = best;

        Debug.Log($"Generation {generationId}: {cumulativeFitness[populationCount - 1]},  totalFinished: {totalFinished}");
        Debug.Log($"Best creature: creature{best}: fitness: {populationDNA[bestIndex].fitness}, dna: {populationDNA[bestIndex].ToString()}");
        return cumulativeFitness;
    }

    /*
     * https://stackoverflow.com/questions/10765660/roulette-wheel-selection-procedure
     */
    private void roulette(int toKeep)
    {
        List<CreatureDNA> nextGeneration = new List<CreatureDNA>();
        if (keepBest)
        {
            CreatureDNA best = populationDNA[bestIndex];
            nextGeneration.Add(best);
            toKeep--;
        }

        for (int i = 0; i < toKeep; i++)
        {
            float rouletteRoll = Random.Range(0, cumulativeFitness[populationCount - 1]);
            int populationIndex = 0;
            while (cumulativeFitness[populationIndex] < rouletteRoll)
            {
                populationIndex++;
            }
            nextGeneration.Add(populationDNA[populationIndex]);
        }

        populationDNA = nextGeneration;

    }
    private void breed()
    {
        int lastIndex = populationDNA.Count - 1;
        while (populationDNA.Count < populationCount)
        {
            CreatureDNA mother = getRandomDNA(lastIndex);
            CreatureDNA father = getRandomDNA(lastIndex);

            CreatureDNA child = CreatureDNA.Crossover(mother, father);

            populationDNA.Add(child);
        }
    }

    private CreatureDNA getRandomDNA(int lastIndex)
    {
        return populationDNA[Random.Range(0, lastIndex)];
    }
}
