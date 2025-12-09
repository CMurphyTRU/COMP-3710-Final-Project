using UnityEngine;
using System.Linq;

public class TerrainEvaluator : MonoBehaviour
{
    public float Fitness { get; private set; }

    private Rigidbody2D[] parts;

    private float startX;
    public float maxX;
    private float previousX;
    private float backwardPenalty;

    private bool initialized = false;
    private bool finished = false;
    private float timer = 0f;

    public float evaluationDuration = 10f;

    

    void Update()
    {
        if (finished) return;

        
        if (parts == null || parts.Length == 0)
        {
            parts = GetComponentsInChildren<Rigidbody2D>();

            if (parts.Length == 0)
            {
                Debug.Log("Parts.length is 0");
                return;
            }
            float x = GetCreatureX();
            startX = x;
            maxX = x;
            previousX = x;
            initialized = true;
            return;
        }

        
        timer += Time.deltaTime;

        if (timer >= evaluationDuration)
        {
            
            finished = true;

            
            Debug.Log($"Creature {gameObject.name} Fitness: {Fitness}");

            return;
        }

        
        Sample();
    }

    float GetCreatureX()
    {
        return parts.Average(p => p.position.x);
    }

    public void ResetFitness()
    {
        Fitness = 0;
        backwardPenalty = 0;
        initialized = false;
        finished = false;
        timer = 0;
        parts = null;
    }

    void Sample()
    {
        if (!initialized) 
        {
            return;
        }
        float x = GetCreatureX();

        if (x > maxX)
        {
            maxX = x;
        }
        if (x < previousX)
        {
            backwardPenalty += (previousX - x);
        }
            

        previousX = x;

        Fitness = Mathf.Max(0f, (maxX - startX));
    }

    
}
