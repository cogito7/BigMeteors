using UnityEngine;
using System.Collections;

// SOLID changes

public class BigMeteor : MonoBehaviour
{
    // Meteor properties - in inspector
    [SerializeField] private float speed = 0.5f;        // speed meteor moves down
    [SerializeField] private int maxHealth = 5;         // number of laser hits needed to destroy
    [SerializeField] private float destroyY = -15f;     // y position where meteor gets destroyed

    // Private state variables
    private int currentHealth;          // current health of meteor
    private GameManager gameManager;   // reference to game manager

    private void Start()
    {
        // Initialize meteor health to max health
        currentHealth = maxHealth;

        // Find and store reference to game manager
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        // Handle meteor behavior
        Move();         // move meteor down
        CheckBounds();  // check if meteor is out of bounds
    }

    // Single Responsibility; only handle movement
    private void Move()
    {
        transform.Translate(Vector3.down * Time.deltaTime * speed);
    }

    // Single Responsibility; only handle boundary checks
    private void CheckBounds()
    {
        if (transform.position.y < destroyY)
            DestroyMeteor();
    }

    // Handle all collision detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Use CompareTag instead of string compare
        if (collision.CompareTag("Player"))
            HandlePlayerHit(collision);
        else if (collision.CompareTag("Laser"))
            HandleLaserHit(collision);
    }

    // Single Responsibility; handle player collision logic
    private void HandlePlayerHit(Collider2D player)
    {
        // Tell game manager to end game; not direct reference)
        gameManager.gameOver = true;

        // Destroy the player
        Destroy(player.gameObject);
    }

    // Single Responsibility; handle laser collision logic
    private void HandleLaserHit(Collider2D laser)
    {
        // Take damage and destroy the laser
        TakeDamage();
        Destroy(laser.gameObject);
    }

    // Single Responsibility; handle damage logic
    private void TakeDamage()
    {
        currentHealth--;

        // Check if meteor should be destroyed
        if (currentHealth <= 0)
            DestroyMeteor();
    }

    // Single Responsibility; handle meteor destruction

    /*private void DestroyMeteor()
    {
        // Trigger stronger screen shake for big meteor
        GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.TriggerScreenShake();
        

        Destroy(gameObject);*/

    private void DestroyMeteor()
    {
        // Trigger screen shake first
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.TriggerScreenShake();
        }

        // Delay destruction
        StartCoroutine(DelayedDestruction());
    }

    private IEnumerator DelayedDestruction()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

}
