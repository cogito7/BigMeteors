using UnityEngine;

// SOLID principles added
public class Laser : MonoBehaviour
{
    // Configurable properties
    [SerializeField] private float speed = 8f; // laser speed
    [SerializeField] private float destroyY = 11f; // y position where laser gets destroyed

    void Start()
    {

    }

    void Update()
    {
        // Single Responsibility; updates and calls focused functions
        Move();
        CheckBounds();
    }

    // Single Responsibility; handles movement
    private void Move()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    // Single Responsibility; handles boundary checks
    private void CheckBounds()
    {
        if (transform.position.y > destroyY)
        {
            DestroyLaser();
        }
    }

    // Single Responsibility; handles laser destruction
    private void DestroyLaser()
    {
        Destroy(gameObject);
    }
}
