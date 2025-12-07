using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShapeMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    [SerializeField] bool IsEnabled;

    public float movementDelay;
    public float movementMagnitude;
    public Vector2 direction;

    private float activationCounter = 0;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void EnableMovement(float movementDelay, float movementMagnitude, Vector2 direction)
    {
        IsEnabled = true;
        this.movementDelay = movementDelay;
        this.movementMagnitude = movementMagnitude;
        this.direction = direction;
    }


    void FixedUpdate()
    {
        if(IsEnabled)
        {
            if (activationCounter > movementDelay)
            {
                activationCounter -= movementDelay;
                rigidBody2D.AddForce(direction * movementMagnitude, ForceMode2D.Impulse);
            }
            activationCounter += Time.deltaTime;
        }
    }

}
