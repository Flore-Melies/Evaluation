using UnityEngine;

public class Fruit : MonoBehaviour
{
    private Spawner spawner;
    private void Start()
    {
        GetComponent<SpriteRenderer>().color = Random.ColorHSV(0, 1, 0, 1, 1, 1, 1, 1);
        spawner = FindObjectOfType<Spawner>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<SnakeController>(out var snake)) return;
        
        snake.EatFruit();
        spawner.SpawnFruit();
        Destroy(gameObject);
    }
}
