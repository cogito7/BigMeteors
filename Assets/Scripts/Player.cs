using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//SOLID Player class; follows single responsibility and open/closed principles; added comments and headers
public class Player : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject laserPrefab;// reference to laser prefab

    // Reference new input system
    private PlayerInputActions playerInputActions;

    [Header("Movement Settings")]
    private float speed = 8f; // player movement speed
    private float horizontalScreenLimit = 15f; // left/right screen boundaries
    private float verticalScreenLimit = 10f; // up/down screen boundaries

    [Header("Shooting Settings")]
    private bool canShoot = true; 

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        // Initialize input actions
        playerInputActions = new PlayerInputActions();
    }

    // Reference to Input Actions with enable and disable ability
    private void OnEnable()
    {
        playerInputActions.Player.Enable();

    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    // Update is called once per frame
    // Single Responsibility; update delegates focused functions
    void Update()
    {
        Movement();
        Shooting();
    }

    // Single Responsibility; handles player movement logic
    void Movement()
    {
        // Get movement input from new input system (WASD)
        Vector2 movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, movementInput.y, 0);
        transform.Translate(movement * Time.deltaTime * speed);


        
        // Screen wrapping - horizontal boundaries
        if (transform.position.x > horizontalScreenLimit || transform.position.x <= -horizontalScreenLimit)
        {
            // Teleport to opposite side of screen
            transform.position = new Vector3(transform.position.x * -1f, transform.position.y, 0);
        }

        // Screen wrapping - vertical boundaries  
        if (transform.position.y > verticalScreenLimit || transform.position.y <= -verticalScreenLimit)
        {
            // Teleport to opposite side of screen
            transform.position = new Vector3(transform.position.x, transform.position.y * -1, 0);
        }

        /*
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * speed);
        if (transform.position.x > horizontalScreenLimit || transform.position.x <= -horizontalScreenLimit)
        {
            transform.position = new Vector3(transform.position.x * -1f, transform.position.y, 0);
        }
        if (transform.position.y > verticalScreenLimit || transform.position.y <= -verticalScreenLimit)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y * -1, 0);
        }
        */
    }

    // Single Responsibility; handles shooting input and laser creation
    void Shooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            Instantiate(laserPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            canShoot = false;
            StartCoroutine("Cooldown");
        }
    }
    // Single Responsibility; handles shooting cooldown timing
    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        canShoot = true;
    }
}
