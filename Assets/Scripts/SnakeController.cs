using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    public float timeBeforeMove;
    public GameObject tailPartPrefab;
    public GameObject gameOverPrefab;

    private Vector2Int input;
    private Vector2Int direction;
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
            direction = input;
            var emptySpace = myRigidbody.position;
            myRigidbody.MovePosition(emptySpace + direction);
            
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
        var floatInput = context.ReadValue<Vector2>();
        var intInput = Vector2Int.RoundToInt(floatInput);
        if (intInput != direction * -1)
            input = intInput;
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
        isAlive = false;
        var gameOver = Instantiate(gameOverPrefab);
        gameOver.GetComponentInChildren<ScoreDisplayer>().SetScore(eatenFruits);
        Destroy(tail.gameObject);
        Destroy(gameObject);
    }
}
