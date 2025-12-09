using UnityEngine;

[System.Serializable]
public class TerrainDNA
{
    public int pointCount;
    public float maxHeightDelta;
    public float smoothness;
    public Color groundColor;

    public float fitness;

    public float[] relativeHeights;

    public const int MinPoints = 20;
    public const int MaxPoints = 120;

    public TerrainDNA(bool randomize = true)
    {
        if (randomize)
        {
            Randomize();
        }
    }

    public void Randomize()
    {
        pointCount = Random.Range(MinPoints, MaxPoints);

        maxHeightDelta = Random.Range(0.1f, 2.5f);
        smoothness = Random.Range(0.1f, 2f);

        groundColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.7f, 1f);

        relativeHeights = new float[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            relativeHeights[i] = Random.Range(-maxHeightDelta, maxHeightDelta);
        }
    }

    public void Mutate(float mutationRate)
    {
        if (Random.value < mutationRate)
            maxHeightDelta += Random.Range(-0.5f, 0.5f);

        if (Random.value < mutationRate)
            smoothness += Random.Range(-0.2f, 0.2f);

        for (int i = 0; i < relativeHeights.Length; i++)
        {
            if (Random.value < mutationRate)
                relativeHeights[i] += Random.Range(-0.5f, 0.5f);
        }

        maxHeightDelta = Mathf.Clamp(maxHeightDelta, 0.1f, 3f);
        smoothness = Mathf.Clamp(smoothness, 0.05f, 2f);
    }


    public static TerrainDNA Crossover(TerrainDNA A, TerrainDNA B)
    {
        TerrainDNA child = new TerrainDNA(false);

        
        child.pointCount = (Random.value < 0.5f) ? A.pointCount : B.pointCount;

        child.maxHeightDelta = (Random.value < 0.5f) ? A.maxHeightDelta : B.maxHeightDelta;
        child.smoothness = (Random.value < 0.5f) ? A.smoothness : B.smoothness;
        child.groundColor = (Random.value < 0.5f) ? A.groundColor : B.groundColor;

        child.relativeHeights = new float[child.pointCount];

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
        TerrainDNA d = new TerrainDNA();

        d.pointCount = this.pointCount;
        d.groundColor = this.groundColor;
        d.smoothness = this.smoothness;

        d.relativeHeights = new float[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            d.relativeHeights[i] = Random.Range(-2f, 2f);
        }

        return d;
    }

}
