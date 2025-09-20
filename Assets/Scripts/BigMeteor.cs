using UnityEngine;


// SOLID changes; incl extension of parent to child class for shared properties

public class BigMeteor : Meteor
{
    // Meteor properties - in inspector
    [SerializeField] private int maxHealth = 5; // number of laser hits needed to destroy

    // Private state variables
    private int currentHealth; // current health of meteor

    private void Start()
    {
        // Initialize meteor health to max health
        currentHealth = maxHealth;

        // Find and store reference to game manager
        gameManager = Object.FindFirstObjectByType<GameManager>();

        // Get Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Randomize orbit distance slightly
        orbitDistance += Random.Range(0f, 2f);
    }

    // Single Responsibility; handle laser collision logic
    protected override void HandleLaserHit(Collider2D laser)
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
            // Delay destruction slightly so shake can start
            StartCoroutine(DelayedDestruction());
    }
}
