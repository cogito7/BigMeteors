using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

// SOLID principles added
public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject meteorPrefab;
    public GameObject bigMeteorPrefab;

    [Header("Game State")]
    public bool gameOver = false;
    public int meteorCount = 0;

    private CameraShake cameraShake;

    // Configurable settings
    [Header("Meteor Spawn Settings")]
    [SerializeField] private int meteorsUntilBig = 5;      // meteors needed before spawning a big one
    [SerializeField] private float meteorSpawnDelay = 1f;  // delay before first spawn
    [SerializeField] private float meteorSpawnInterval = 0.5f; // interval between waves

    [Header("Wave Spawn Settings")]
    [SerializeField] private int meteorWaveCount = 5;   // number of meteors per wave
    [SerializeField] private float minSpawnRadius = 3f; // closest meteors can spawn
    [SerializeField] private float maxSpawnRadius = 7f; // farthest meteors can spawn

    [Header("Screen Shake Settings")]
    [SerializeField] private float shakeIntensity = 4f;
    [SerializeField] private float shakeDuration = 1f;

    private CinemachineCamera cinemachineCamera;
    private CinemachineBasicMultiChannelPerlin noiseExtension;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
        StartMeteorSpawning();
        cameraShake = GetComponent<CameraShake>();
    }

    void Update()
    {
        HandleGameOver();
        HandleRestart();
        CheckBigMeteorSpawn();
    }

    // Single Responsibility; handle game over state
    void HandleGameOver()
    {
        if (gameOver)
        {
            CancelInvoke(); // stop meteor spawning when game over
        }
    }

    // Single Responsibility; handle restart input
    void HandleRestart()
    {
        if (Input.GetKeyDown(KeyCode.R) && gameOver)
        {
            SceneManager.LoadScene("Week5Lab");
        }
    }

    // Single Responsibility; set up screen shake when meteor object is destroyed
    public void TriggerScreenShake()
    {
        // Pass your values to the CameraShake script
        cameraShake.ShakeWithCustomValues(shakeIntensity, shakeDuration);
    }

    // Single Responsibility; check if big meteor should spawn
    void CheckBigMeteorSpawn()
    {
        if (meteorCount >= meteorsUntilBig)
        {
            SpawnBigMeteor();
        }
    }

    // Single Responsibility: Spawn player and set camera target
    void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);

            // Find the Cinemachine Camera (Unity 6 component)
            CinemachineCamera vcam = Object.FindFirstObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                // Unity 6 Cinemachine Camera syntax
                vcam.Follow = player.transform;
                vcam.LookAt = player.transform;
            }
        }
    }

    // Single Responsibility; start meteor spawning
    void StartMeteorSpawning()
    {
        // call the wave-based spawner
        InvokeRepeating("SpawnMeteorWave", meteorSpawnDelay, meteorSpawnInterval);
    }

    // Single Responsibility; spawn a wave of meteors around the player
    void SpawnMeteorWave()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        Transform player = playerObj.transform;

        for (int i = 0; i < meteorWaveCount; i++)
        {
            // pick random radius between min and max
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

            // spread meteors evenly around a circle
            float angle = i * Mathf.PI * 2f / meteorWaveCount;
            Vector2 spawnPos = (Vector2)player.position +
                               new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            // spawn
            GameObject newMeteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            // assign the player reference to meteor script
            Meteor meteorScript = newMeteor.GetComponent<Meteor>();
            if (meteorScript != null)
            {
                meteorScript.player = player;
            }
        }
    }

    // Single Responsibility; spawn big meteor (random angle around player)
    void SpawnBigMeteor()
    {
        meteorCount = 0;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        Transform player = playerObj.transform;

        // pick random radius and angle
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius) + 1;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 spawnPos = (Vector2)player.position +
                           new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        GameObject newBigMeteor = Instantiate(bigMeteorPrefab, spawnPos, Quaternion.identity);

        // assign the player reference
        Meteor meteorScript = newBigMeteor.GetComponent<Meteor>();
        if (meteorScript != null)
        {
            meteorScript.player = player;
        }
    }
}