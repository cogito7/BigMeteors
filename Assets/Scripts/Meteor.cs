using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SOLID principles added with orbiting behavior
public class Meteor : MonoBehaviour
{
    [Header("Orbiting Behavior")]
    public Transform player;                     // Player reference
    public float orbitDistance = 4f;            // Distance from player
    public float orbitSpeed = 3f;              // Orbiting speed
    public float minSpeed = 1f;               // Minimum speed
    public float maxSpeed = 6f;             // Maximum speed
    public float avoidanceStrength = 1f; // Strength meteors push away
    public float avoidanceRadius = 2f;    // How close to get before repelling

    [Header("Original Meteor Settings")]
    [SerializeField] private float speed = 2f; // meteor movement speed (fallback)
    [SerializeField] private float destroyY = -15f; // y position where meteor gets destroyed

    // Reference to GameManager instead of gameobject.find
    private GameManager gameManager;
    private Rigidbody2D rb; // Add Rigidbody2D for physics movement

    void Start()
    {
        // Find GameManager at start
        gameManager = Object.FindFirstObjectByType<GameManager>();

        // Get Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Randomize orbit distance slightly
        orbitDistance += Random.Range(0f, 2f);
    }

    void Update()
    {
        // Single Responsibility: updates and calls focused functions
        CheckBounds();
    }

    void FixedUpdate()
    {
        // Use FixedUpdate for physics-based movement
        if (player == null)
        {
            // Fallback to original movement if no player
            FallbackMovement();
        }
        else
        {
            // New orbiting behavior
            OrbitAroundPlayer();
            AvoidOtherMeteors();
        }
    }

    // Single Responsibility: Original movement as fallback
    private void FallbackMovement()
    {
        if (rb != null)
            rb.velocity = Vector2.down * speed;
        else
            transform.Translate(Vector3.down * Time.deltaTime * speed);
    }

    // Single Responsibility: Orbit around player
    private void OrbitAroundPlayer()
    {
        Vector2 directionToPlayer = (Vector2)(transform.position - player.position);
        float distance = directionToPlayer.magnitude;

        // Calculate scaled orbit speed based on distance
        float scaledOrbitSpeed = orbitSpeed;
        if (distance < 2f)
            scaledOrbitSpeed = maxSpeed;
        else if (distance < 5f)
            scaledOrbitSpeed = orbitSpeed + (8f / (distance + 0.1f));
        else
            scaledOrbitSpeed = minSpeed;

        // Keep fixed orbit distance
        Vector2 targetPosition = (Vector2)player.position + directionToPlayer.normalized * orbitDistance;

        // Find perpendicular direction to orbit
        Vector2 orbitDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x).normalized;
        Vector2 moveVector = orbitDirection * scaledOrbitSpeed;

        // Correction to maintain orbit distance
        Vector2 correction = (targetPosition - (Vector2)transform.position);
        moveVector += correction;

        rb.velocity = moveVector;
    }

    // Single Responsibility: Avoid other meteors
    private void AvoidOtherMeteors()
    {
        GameObject[] allMeteors = GameObject.FindGameObjectsWithTag("Meteor"); // "Meteor" tag in inspector
        Vector2 avoidanceVector = Vector2.zero;

        foreach (GameObject other in allMeteors)
        {
            if (other == this.gameObject) continue;

            Vector2 offset = transform.position - other.transform.position;
            float sqrDist = offset.sqrMagnitude;

            if (sqrDist < avoidanceRadius * avoidanceRadius)
            {
                avoidanceVector += offset.normalized * avoidanceStrength / Mathf.Max(sqrDist, 0.01f);
            }
        }

        rb.velocity += avoidanceVector;
    }

    // Single Responsibility: handles boundary checks
    private void CheckBounds()
    {
        if (transform.position.y < destroyY)
            DestroyMeteor();
    }

    // Handle collision detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Use CompareTag for better performance
        if (collision.CompareTag("Player"))
            HandlePlayerHit(collision);
        else if (collision.CompareTag("Laser"))
            HandleLaserHit(collision);
    }

    // Single Responsibility: handles player collision
    private void HandlePlayerHit(Collider2D player)
    {
        gameManager.gameOver = true;
        Destroy(player.gameObject);
        DestroyMeteor();
    }

    // Single Responsibility: handle laser collision
    private void HandleLaserHit(Collider2D laser)
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.meteorCount++;
            gameManager.TriggerScreenShake(); // Trigger shake first
        }
        Destroy(laser.gameObject);
        // Delay destruction slightly so shake can start
        StartCoroutine(DelayedDestruction());
    }

    private IEnumerator DelayedDestruction()
    {
        yield return new WaitForSeconds(0.1f); // Small delay
        DestroyMeteor();
    }

    // Single Responsibility: handle meteor destruction
    private void DestroyMeteor()
    {
        Destroy(gameObject);
    }
}
