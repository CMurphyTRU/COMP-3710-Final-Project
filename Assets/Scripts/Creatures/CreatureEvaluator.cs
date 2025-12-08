using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[System.Serializable]
public class CreatureEvaluator : MonoBehaviour
{
    public float totalDistance;
    public float currentDisplacementFromFinish;
    public float timeToFinish;
    public float timeAlive;
    public bool hasFinished;

    private Vector2 lastLocation;
    public Vector2 startPosition;
    public Vector2 finishPosition;

    public static float DisplacementFitnessWeight = 40;
    public static float TotalDistanceFitnessWeight = 40;
    public static float TimeToFinishFitnessWeight = 20;
    public static float TotalFitnessWeight = DisplacementFitnessWeight + TimeToFinishFitnessWeight + TotalDistanceFitnessWeight;
    void Start()
    {
        totalDistance = 1;
        hasFinished = false;
        timeAlive = 0;
        startPosition = transform.position;
        lastLocation = startPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        timeToFinish = timeAlive;
        currentDisplacementFromFinish = 0.5f;
        hasFinished = true;
    }

    void Update()
    {
        this.timeAlive += Time.deltaTime;

        if (!hasFinished)
        {
            Vector2 currentPosition = transform.position;
            float distanceFromLast = Vector2.Distance(lastLocation, currentPosition);
            this.totalDistance += distanceFromLast;
            this.lastLocation = currentPosition;
            this.currentDisplacementFromFinish = finishPosition.x - currentPosition.x;
        }
    }

    public float getFitness(float generationTime)
    {
        if (!hasFinished) timeToFinish = generationTime;
        
        float timeToFinishScore = (generationTime / timeToFinish) * (TimeToFinishFitnessWeight / TotalDistanceFitnessWeight);
        float totalDistanceScore = (1 / totalDistance * 10) * (TotalDistanceFitnessWeight / TotalDistanceFitnessWeight);
        float displacementFromFinishScore = (1 / ((currentDisplacementFromFinish) / (finishPosition.x - startPosition.x))) * (DisplacementFitnessWeight / TotalDistanceFitnessWeight);

        
        return timeToFinishScore + totalDistanceScore + displacementFromFinishScore;
    }
}
