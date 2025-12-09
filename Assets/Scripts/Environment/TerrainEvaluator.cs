using UnityEngine;
using System.Linq;

public class TerrainEvaluator : MonoBehaviour
{
    public float Fitness { get; private set; }   // fitness score for this creature

    private Rigidbody2D[] parts;                 // all body parts of the creature

    private float startX;                        // starting x-position
    public float maxX;                           // farthest x-position reached
    private float previousX;                     // x-position from last frame
    private float backwardPenalty;               // penalty for moving backwards

    private bool initialized = false;            // true once first position is set
    private bool finished = false;               // true when evaluation ends
    private float timer = 0f;                    // how long the creature has been evaluated

    public float evaluationDuration = 10f;       // how long the evaluation lasts



    void Update()
    {
        // stop if evaluation is complete
        if (finished) return;

        // setup creature parts on the first update
        if (parts == null || parts.Length == 0)
        {
            parts = GetComponentsInChildren<Rigidbody2D>();

            // if creature has no rigidbodies, nothing to evaluate
            if (parts.Length == 0)
            {
                Debug.Log("Parts.length is 0");
                return;
            }

            // initialize movement tracking
            float x = GetCreatureX();
            startX = x;
            maxX = x;
            previousX = x;
            initialized = true;
            return;
        }

        // update timer
        timer += Time.deltaTime;

        // stop evaluation when time is up
        if (timer >= evaluationDuration)
        {
            finished = true;

            Debug.Log($"Creature {gameObject.name} Fitness: {Fitness}");
            return;
        }

        // take a fitness sample
        Sample();
    }

    float GetCreatureX()
    {
        // average x-position of all body parts
        return parts.Average(p => p.position.x);
    }

    public void ResetFitness()
    {
        // reset all evaluation values
        Fitness = 0;
        backwardPenalty = 0;
        initialized = false;
        finished = false;
        timer = 0;
        parts = null;
    }

    void Sample()
    {
        // cannot sample before initialization
        if (!initialized)
        {
            return;
        }

        float x = GetCreatureX();

        // update farthest forward distance
        if (x > maxX)
        {
            maxX = x;
        }

        // apply penalty for backward movement
        if (x < previousX)
        {
            backwardPenalty += (previousX - x);
        }

        previousX = x;

        // compute fitness (distance traveled forward)
        Fitness = Mathf.Max(0f, (maxX - startX));
    }
}
