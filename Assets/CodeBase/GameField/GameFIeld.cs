using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameFIeld : MonoBehaviour, IBubbleCollisionObserver
{
    [SerializeField] private GameObject[] _bubblePrefabs;
    [SerializeField] private BubblesForShot _bubblesForShot;

    public event Action<int> OnAddScore;
    public event Action<int[,], int> GameStateHasBeenUpdated;

    private int[,] _field;
    private Dictionary<Coords, GameObject> _bubbles;
    private Vector3 _startFieldCoords = new Vector3(-4, 5, 0);

    private void Awake() =>
        _bubblesForShot.OnChargeNextBubble += (int numberRemainingBubbles, Sprite s) =>
            GameStateHasBeenUpdated?.Invoke(_field, numberRemainingBubbles);

    public void Build(int[,] field)
    {
        _field = field;
        _bubbles = new Dictionary<Coords, GameObject>();

        for (int i = 1; i < field.GetLength(0); i++)
        {
            for (int j = 1; j < field.GetLength(1); j++)
            {
                if (field[i, j] != 0)
                {
                    Vector3 position = new Vector3(_startFieldCoords.x + (j - 1), _startFieldCoords.y - (i - 1), 0);
                    if (i % 2 == 1)
                        position.x += .5f;

                    CreateBubble(_bubblePrefabs[field[i, j]], position, new Coords(j, i));
                }
            }
        }
    }

    public void Restart(int[,] field)
    {
        List<Coords> coordsForDestroy = new List<Coords>();
        
        foreach (var bubble in _bubbles)
            coordsForDestroy.Add(bubble.Key);

        for (int i = 0; i < coordsForDestroy.Count; i++)
            DestroyBubble(coordsForDestroy[i]);

        Build(field);
    }

    public void HandleBubbleCollision(Collider2D other, Transform bubble, bool megaShot)
    {
        Coords cellCoords;

        if (megaShot)
        {
            var hitCoords = other.GetComponent<Bubble>().CellCoords;
            DestroyBubble(hitCoords);
            cellCoords = hitCoords;
        }
        else
        {
            cellCoords = DefineCellCoords(other, bubble);
        }

        SetBubbleInCell(cellCoords, _bubblesForShot.CurrentBubble);
        ShakeBubbles(cellCoords, -(bubble.position - other.transform.position));
        DestroyRepeatingBubbles(cellCoords,
            () => GameStateHasBeenUpdated?.Invoke(_field, _bubblesForShot.NumberRemainingBubbles));
    }

    public void HandleMissBubbleCollision()
    {
    }

    public void HandleWin() =>
        DropAndDestroyAllBubbles();

    private void CreateBubble(GameObject bubblePrefab, Vector3 position, Coords cellCoords)
    {
        GameObject bubbleObj = Instantiate(bubblePrefab, position, quaternion.identity, transform);
        Bubble bubble = bubbleObj.GetComponent<Bubble>();

        bubble.CellCoords = cellCoords;
        bubble.OnBrust += OnAddScore;
        _bubbles[cellCoords] = bubbleObj;
    }

    private void SetBubbleInCell(Coords cellCoords, BubbleType type)
    {
        _field[cellCoords.Y, cellCoords.X] = (int) type;

        Vector3 position = new Vector3(_startFieldCoords.x + (cellCoords.X - 1),
            _startFieldCoords.y - (cellCoords.Y - 1), 0);
        if (cellCoords.Y % 2 == 1)
            position.x += .5f;

        CreateBubble(_bubblePrefabs[_field[cellCoords.Y, cellCoords.X]], position, cellCoords);
    }

    private void ShakeBubbles(Coords targetCoords, Vector2 impulseDirection) =>
        _bubbles[targetCoords].GetComponent<Rigidbody2D>().AddForce(impulseDirection * 15, ForceMode2D.Impulse);

    private Coords DefineCellCoords(Collider2D other, Transform bubble)
    {
        Coords hitCoords = other.GetComponent<Bubble>().CellCoords;
        Vector3 hitDirection = (bubble.position - other.transform.position).normalized;
        double directionInRadians = Math.Atan2(hitDirection.y, hitDirection.x);
        Coords cellCoords = DefineCell(hitCoords, directionInRadians);
        return cellCoords;
    }

    private Coords DefineCell(Coords hitCoords, double directionInRadians)
    {
        if (Utilities.BubbleOnTheRight(directionInRadians))
        {
            return new Coords(hitCoords.X + 1, hitCoords.Y);
        }
        else if (Utilities.BubbleOnTheRightAndTop(directionInRadians))
        {
            if (hitCoords.Y % 2 == 1)
                return new Coords(hitCoords.X + 1, hitCoords.Y - 1);
            else
                return new Coords(hitCoords.X, hitCoords.Y - 1);
        }
        else if (Utilities.BubbleOnTheLeftAndTop(directionInRadians))
        {
            if (hitCoords.Y % 2 == 1)
                return new Coords(hitCoords.X, hitCoords.Y - 1);
            else
                return new Coords(hitCoords.X - 1, hitCoords.Y - 1);
        }
        else if (Utilities.BubbleOnTheLeft(directionInRadians))
        {
            return new Coords(hitCoords.X - 1, hitCoords.Y);
        }
        else if (Utilities.BubbleOnTheLeftAndDown(directionInRadians))
        {
            if (hitCoords.Y % 2 == 1)
                return new Coords(hitCoords.X, hitCoords.Y + 1);
            else
                return new Coords(hitCoords.X - 1, hitCoords.Y + 1);
        }
        else if (Utilities.BubbleOnTheRightAndDown(directionInRadians))
        {
            if (hitCoords.Y % 2 == 1)
                return new Coords(hitCoords.X + 1, hitCoords.Y + 1);
            else
                return new Coords(hitCoords.X, hitCoords.Y + 1);
        }

        return new Coords();
    }

    public void DestroyRepeatingBubbles(Coords targetCoords, Action callBack)
    {
        Coords[] repeatingBubbles = FindRepeatingBubbles(targetCoords);

        if (repeatingBubbles.Length > 2)
            StartCoroutine(DestroyBubblesOneByOne(repeatingBubbles,
                new Action[] {DestroyBubblesSuspendedInTheAir, callBack}));
    }

    private void DestroyBubblesSuspendedInTheAir()
    {
        Coords[] bubblesInTheAir = FindBubblesSuspendedInTheAir();

        for (int i = 0; i < bubblesInTheAir.Length; i++)
        {
            _bubbles[bubblesInTheAir[i]].tag = "Untagged";
            _bubbles[bubblesInTheAir[i]].GetComponent<SpringJoint2D>().enabled = false;
            _bubbles.Remove(bubblesInTheAir[i]);

            _field[bubblesInTheAir[i].Y, bubblesInTheAir[i].X] = 0;
        }
    }

    private void DropAndDestroyAllBubbles()
    {
        foreach (var bubble in _bubbles)
        {
            _field[bubble.Key.Y, bubble.Key.X] = 0;
            bubble.Value.GetComponent<SpringJoint2D>().enabled = false;
        }

        _bubbles.Clear();
    }

    private Coords[] FindBubblesSuspendedInTheAir()
    {
        List<Coords> result = new List<Coords>();
        List<Coords> pinnedObjects = new List<Coords>();
        Queue<Coords> points = new Queue<Coords>();

        for (int i = 1; i < _field.GetLength(1) - 1; i++)
        {
            if (_field[1, i] != 0)
                points.Enqueue(new Coords(i, 1));
        }

        while (points.Count != 0)
        {
            pinnedObjects.Add(points.Peek());
            Coords[] neighbors = FindNeighbors(points.Dequeue(), false);

            if (neighbors.Length > 0)
            {
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (!pinnedObjects.Contains(neighbors[i]) && !points.Contains(neighbors[i]))
                        points.Enqueue(neighbors[i]);
                }
            }
        }

        for (int i = 0; i < _field.GetLength(0); i++)
        {
            for (int j = 0; j < _field.GetLength(1); j++)
            {
                if (!pinnedObjects.Contains(new Coords(j, i)) && _field[i, j] != 0)
                    result.Add(new Coords(j, i));
            }
        }

        return result.ToArray();
    }

    private Coords[] FindRepeatingBubbles(Coords targetCoords)
    {
        List<Coords> result = new List<Coords>();
        Queue<Coords> points = new Queue<Coords>();
        points.Enqueue(targetCoords);

        while (points.Count != 0)
        {
            result.Add(points.Peek());
            Coords[] neighbors = FindNeighbors(points.Dequeue(), true);

            if (neighbors.Length > 0)
            {
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (!result.Contains(neighbors[i]) && !points.Contains(neighbors[i]))
                        points.Enqueue(neighbors[i]);
                }
            }
        }

        return result.ToArray();
    }

    private Coords[] FindNeighbors(Coords targetCoords, bool repeated)
    {
        List<Coords> result = new List<Coords>();

        Coords[] neighborsCoords;

        if (targetCoords.Y % 2 == 1)
            neighborsCoords = Utilities.AdjacentCoordinatesForAnOddRow(targetCoords);
        else
            neighborsCoords = Utilities.AdjacentCoordinatesForAnEvenRow(targetCoords);

        for (int i = 0; i < neighborsCoords.Length; i++)
        {
            if (repeated)
            {
                if (_field[targetCoords.Y, targetCoords.X] == _field[neighborsCoords[i].Y, neighborsCoords[i].X])
                    result.Add(neighborsCoords[i]);
            }
            else
            {
                if (_field[neighborsCoords[i].Y, neighborsCoords[i].X] != 0)
                    result.Add(neighborsCoords[i]);
            }
        }

        return result.ToArray();
    }

    private void InvokeAddScore(int points) =>
        OnAddScore?.Invoke(points);

    private IEnumerator DestroyBubblesOneByOne(Coords[] bubbles, Action[] callBacks)
    {
        for (int i = 0; i < bubbles.Length; i++)
        {
            yield return new WaitForSeconds(.1f);
            DestroyBubble(bubbles[i]);
        }

        for (int i = 0; i < callBacks.Length; i++)
            callBacks[i]?.Invoke();
    }

    private void DestroyBubble(Coords bubbleCoords)
    {
        Bubble bubble = _bubbles[bubbleCoords].GetComponent<Bubble>();
        bubble.Burst();
        bubble.OnBrust -= InvokeAddScore;
        _bubbles.Remove(bubbleCoords);
        _field[bubbleCoords.Y, bubbleCoords.X] = 0;
    }
}