using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class CreatureMovement : MonoBehaviour, GeneHolder
{
    // Arguments
    [SerializeField] public float delay;
    [SerializeField] public float magnitude;
    [SerializeField] public Vector2 direction;

    // Physics Components

    [SerializeField] Rigidbody2D rigidBody;

    // Tracking Variables

    [SerializeField] private float timeSinceLastMovement;

    private static float MIN_DELAY = 0.5f;
    private static float MAX_DELAY = 3f;
    private static float MIN_MAGNITUDE = 100f;
    private static float MAX_MAGNITUDE = 300f;
    private static Vector2 MAX_DIR = Vector2.one;


    public string genes {  get; set; }

    void Start()
    {
        timeSinceLastMovement = 0;
        rigidBody = GetComponent<Rigidbody2D>();

        delay = Random.Range(MIN_DELAY, MAX_DELAY);
        magnitude = Random.Range(MIN_MAGNITUDE, MAX_MAGNITUDE);
        direction = new Vector2(Random.Range(0.1f, MAX_DIR.x), Random.Range(0.1f, MAX_DIR.y));

    }

    // Update is called once per frame
    void Update()
    {
        if(timeSinceLastMovement >= delay)
        {
            timeSinceLastMovement -= delay;
            rigidBody.AddForce(direction * magnitude);
        }
        timeSinceLastMovement += Time.deltaTime;
    }
}
