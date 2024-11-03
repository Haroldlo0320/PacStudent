using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    [Header("Cherry Settings")]
    public GameObject cherryPrefab;           // Assign the CherryPrefab in the Inspector
    public float spawnInterval = 10f;         // Time between spawns in seconds
    public float moveDuration = 5f;           // Time it takes for the cherry to move across the screen

    private Camera mainCamera;
    private float cameraHalfWidth;
    private float cameraHalfHeight;

    void Start()
    {
        mainCamera = Camera.main;
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        // Start the spawning coroutine
        StartCoroutine(SpawnCherryRoutine());
    }

    /// <summary>
    /// Coroutine to spawn cherries at regular intervals.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCherryRoutine()
    {
        while (true)
        {
            SpawnCherry();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Spawns a cherry at a random position outside the camera view and initiates its movement.
    /// </summary>
    void SpawnCherry()
    {
        // Determine a random side: 0 = Top, 1 = Bottom, 2 = Left, 3 = Right
        int side = Random.Range(0, 4);
        Vector2 spawnPosition = Vector2.zero;
        Vector2 endPosition = Vector2.zero;

        switch (side)
        {
            case 0: // Top
                spawnPosition = new Vector2(Random.Range(-cameraHalfWidth, cameraHalfWidth), cameraHalfHeight + 1f);
                endPosition = new Vector2(Random.Range(-cameraHalfWidth, cameraHalfWidth), -cameraHalfHeight - 1f);
                break;
            case 1: // Bottom
                spawnPosition = new Vector2(Random.Range(-cameraHalfWidth, cameraHalfWidth), -cameraHalfHeight - 1f);
                endPosition = new Vector2(Random.Range(-cameraHalfWidth, cameraHalfWidth), cameraHalfHeight + 1f);
                break;
            case 2: // Left
                spawnPosition = new Vector2(-cameraHalfWidth - 1f, Random.Range(-cameraHalfHeight, cameraHalfHeight));
                endPosition = new Vector2(cameraHalfWidth + 1f, Random.Range(-cameraHalfHeight, cameraHalfHeight));
                break;
            case 3: // Right
                spawnPosition = new Vector2(cameraHalfWidth + 1f, Random.Range(-cameraHalfHeight, cameraHalfHeight));
                endPosition = new Vector2(-cameraHalfWidth - 1f, Random.Range(-cameraHalfHeight, cameraHalfHeight));
                break;
        }

        // Instantiate the cherry
        GameObject cherry = Instantiate(cherryPrefab, spawnPosition, Quaternion.identity);
        cherry.transform.SetParent(this.transform); // Optional: Organize under CherryController in Hierarchy

        // Start the movement coroutine
        StartCoroutine(MoveCherryRoutine(cherry.transform, endPosition));
    }

    /// <summary>
    /// Moves the cherry from its spawn position to the end position over moveDuration seconds.
    /// </summary>
    /// <param name="cherryTransform">Transform of the cherry GameObject</param>
    /// <param name="endPos">Destination position</param>
    /// <returns></returns>
    IEnumerator MoveCherryRoutine(Transform cherryTransform, Vector2 endPos)
    {
        Vector2 startPos = cherryTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            cherryTransform.position = Vector2.Lerp(startPos, endPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure cherry reaches the end position
        cherryTransform.position = endPos;

        // Optionally, check if the cherry is out of camera bounds before destroying
        if (IsOutOfCameraView(cherryTransform.position))
        {
            Destroy(cherryTransform.gameObject);
        }
    }

    /// <summary>
    /// Checks if a position is outside the camera view.
    /// </summary>
    /// <param name="position">World position to check</param>
    /// <returns>True if out of view, else False</returns>
    bool IsOutOfCameraView(Vector2 position)
    {
        return (position.x < -cameraHalfWidth - 1f || position.x > cameraHalfWidth + 1f ||
                position.y < -cameraHalfHeight - 1f || position.y > cameraHalfHeight + 1f);
    }
}