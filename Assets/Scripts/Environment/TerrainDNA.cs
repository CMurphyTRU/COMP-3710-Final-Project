using UnityEngine;

[System.Serializable]
public class TerrainDNA
{
    // dna variables
    public int pointCount;               // how many points the terrain will have
    public float maxHeightDelta;         // how much each point can vary in height
    public float smoothness;             // how smooth or rough the terrain shape is
    public Color groundColor;            // color of the terrain

    // fitness value used by the genetic algorithm
    public float fitness;

    public float[] relativeHeights;      // height values for each terrain point

    public const int MinPoints = 20;     // minimum allowed terrain points
    public const int MaxPoints = 120;    // maximum allowed terrain points

    public TerrainDNA(bool randomize = true)
    {
        // optionally randomize DNA on creation
        if (randomize)
        {
            Randomize();
        }
    }

    public void Randomize()
    {
        // pick random terrain size
        pointCount = Random.Range(MinPoints, MaxPoints);

        // randomize terrain shape features
        maxHeightDelta = Random.Range(0.1f, 2.5f);
        smoothness = Random.Range(0.1f, 2f);

        // random ground color
        groundColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.7f, 1f);

        // create height array
        relativeHeights = new float[pointCount];

        // fill heights with random values
        for (int i = 0; i < pointCount; i++)
        {
            relativeHeights[i] = Random.Range(-maxHeightDelta, maxHeightDelta);
        }
    }

    public void Mutate(float mutationRate)
    {
        // small chance to change height delta
        if (Random.value < mutationRate)
            maxHeightDelta += Random.Range(-0.5f, 0.5f);

        // small chance to change smoothness
        if (Random.value < mutationRate)
            smoothness += Random.Range(-0.2f, 0.2f);

        // small chance to mutate each height point
        for (int i = 0; i < relativeHeights.Length; i++)
        {
            if (Random.value < mutationRate)
                relativeHeights[i] += Random.Range(-0.5f, 0.5f);
        }

        // keep values in valid ranges
        maxHeightDelta = Mathf.Clamp(maxHeightDelta, 0.1f, 3f);
        smoothness = Mathf.Clamp(smoothness, 0.05f, 2f);
    }

    public static TerrainDNA Crossover(TerrainDNA A, TerrainDNA B)
    {
        // create child DNA without randomizing it
        TerrainDNA child = new TerrainDNA(false);

        // child inherits values from either parent
        child.pointCount = (Random.value < 0.5f) ? A.pointCount : B.pointCount;
        child.maxHeightDelta = (Random.value < 0.5f) ? A.maxHeightDelta : B.maxHeightDelta;
        child.smoothness = (Random.value < 0.5f) ? A.smoothness : B.smoothness;
        child.groundColor = (Random.value < 0.5f) ? A.groundColor : B.groundColor;

        // create height array for child
        child.relativeHeights = new float[child.pointCount];

        // mix heights from parents
        for (int i = 0; i < child.pointCount; i++)
        {
            float aH = A.relativeHeights[i % A.relativeHeights.Length];
            float bH = B.relativeHeights[i % B.relativeHeights.Length];

            child.relativeHeights[i] = (Random.value < 0.5f) ? aH : bH;
        }

        return child;
    }

    public TerrainDNA CloneRandomized()
    {
        // create new DNA object
        TerrainDNA d = new TerrainDNA();

        // copy basic settings
        d.pointCount = this.pointCount;
        d.groundColor = this.groundColor;
        d.smoothness = this.smoothness;

        // create new height array
        d.relativeHeights = new float[pointCount];

        // fill heights with fresh random values
        for (int i = 0; i < pointCount; i++)
        {
            d.relativeHeights[i] = Random.Range(-2f, 2f);
        }

        return d;
    }
}
