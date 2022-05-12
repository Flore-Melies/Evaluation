using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject fruit;
    public BoxCollider2D playZone;

    private Vector2Int minSpawnPos;
    private Vector2Int maxSpawnPos;

    private void Start()
    {
        InitializeMinMaxPositions();
        SpawnFruit();
    }

    private void InitializeMinMaxPositions()
    {
        var zoneBounds = playZone.bounds;
        var maxPos = zoneBounds.max;
        var minPos = zoneBounds.min;
        maxSpawnPos = Vector2Int.FloorToInt(maxPos);
        minSpawnPos = Vector2Int.CeilToInt(minPos);
    }

    public void SpawnFruit()
    {
        var xPos = Random.Range(minSpawnPos.x, maxSpawnPos.x);
        var yPos = Random.Range(minSpawnPos.y, maxSpawnPos.y);
        var position = new Vector2(xPos, yPos);
        Instantiate(fruit, position, Quaternion.identity);
    }
}
