using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    public float timeBeforeMove;
    public GameObject tailPartPrefab;
    public GameObject gameOverPrefab;

    private Vector2Int input;
    private Vector2Int lastUsedInput;
    private Rigidbody2D myRigidbody;
    private Transform tail;
    private int eatenFruits;
    private bool fruitEaten = true;
    private bool isAlive = true;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private IEnumerator Start()
    {
        tail = new GameObject("Tail").transform;
        yield return new WaitWhile(() => input == Vector2Int.zero);
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (isAlive)
        {
            lastUsedInput = input;
            var emptySpace = myRigidbody.position;
            myRigidbody.MovePosition(emptySpace + input);
            
            yield return new WaitForFixedUpdate();

            foreach (Transform tailPart in tail)
            {
                var lastPosition = tailPart.position;
                tailPart.position = emptySpace;
                emptySpace = lastPosition;
            }

            AddTailPart(emptySpace);
            yield return new WaitForSeconds(timeBeforeMove);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        var rawInput = context.ReadValue<Vector2Int>();
        var newInput = Vector2Int.RoundToInt(rawInput);
        if (newInput != lastUsedInput * -1)
            input = newInput;
    }

    private void AddTailPart(Vector2 newPartPosition)
    {
        if (!fruitEaten)
            return;
        Instantiate(tailPartPrefab, newPartPosition, Quaternion.identity, tail);
        fruitEaten = false;
    }

    public void EatFruit()
    {
        fruitEaten = true;
        eatenFruits++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayZone"))
            Die();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.parent == tail)
            Die();
    }

    private void Die()
    {
        if (!isAlive)
            return;
        isAlive = false;
        var gameOver = Instantiate(gameOverPrefab);
        gameOver.GetComponentInChildren<ScoreDisplayer>().SetScore(eatenFruits);
        Destroy(tail.gameObject);
        Destroy(gameObject);
    }
}
