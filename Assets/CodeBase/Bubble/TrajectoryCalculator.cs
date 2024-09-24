using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrajectoryCalculator : MonoBehaviour
{
    private const string BubbleTag = "Bubble";

    [SerializeField] private float _speedMultiplier;
    [SerializeField] private float _maxTensionLength;
    [SerializeField] private float _spreadAngelMegaShot;
    [SerializeField] private Transform _leftWall;
    [SerializeField] private Transform _rightWall;

    private Vector3 _startPosition;
    private int _trajectoryAccuracy = 100;

    private void Awake() =>
        _startPosition = transform.position;

    public Vector3 CalculateDirection(bool megaShot = false)
    {
        Vector3 direction = (_startPosition - transform.position).normalized;

        if (megaShot)
        {
            float spreadAngel = Random.Range(-_spreadAngelMegaShot, _spreadAngelMegaShot);
            direction = AddSpread(spreadAngel, direction);
        }

        return direction;
    }

    public float CalculateSpeed() =>
        (_startPosition - transform.position).magnitude * _speedMultiplier;

    public bool FullTension(Vector3 cursorPosition) =>
        (_startPosition - cursorPosition).magnitude >= _maxTensionLength;

    public Vector3 CalculateFullTensionPosition(Vector3 cursorPosition) =>
        _startPosition + (cursorPosition - _startPosition).normalized * _maxTensionLength;

    public Vector3[] CalculateTrajectory()
    {
        Vector3[] points = new Vector3[15 * _trajectoryAccuracy];
        Vector3 movementVector = CalculateDirection() * CalculateSpeed();
        float timeInterval = .1f / _trajectoryAccuracy;

        points[0] = transform.position;

        for (int i = 1; i < points.Length; i++)
        {
            points[i] = points[i - 1] + movementVector * timeInterval;
            movementVector += Physics.gravity * timeInterval;

            if (points[i].x <= _leftWall.position.x || points[i].x >= _rightWall.position.x)
                movementVector.x *= -1;
        }

        points = ReduceArray(points);
        return RemovePointsBehindTheBalls(points.ToList());
    }

    public Vector3[] CalculateTrajectory(Vector3 direction, out Vector3 pointRefraction)
    {
        Vector3[] points = new Vector3[15 * _trajectoryAccuracy];
        Vector3 movementVector = direction * CalculateSpeed();
        float timeInterval = .05f / _trajectoryAccuracy;
        pointRefraction = Vector3.zero;

        points[0] = transform.position;

        for (int i = 1; i < points.Length; i++)
        {
            points[i] = points[i - 1] + movementVector * timeInterval;
            movementVector += Physics.gravity * timeInterval;

            if (points[i].x <= _leftWall.position.x || points[i].x >= _rightWall.position.x)
            {
                movementVector.x *= -1;
                pointRefraction = points[i];
            }
        }

        points = ReduceArray(points);
        return RemovePointsBehindTheBalls(points.ToList());
    }

    public Vector3[][] CalculateMegaTrajectory()
    {
        Vector3 pointRefractionOne;
        Vector3 pointRefractionTwo;

        List<List<Vector3>> result = new List<List<Vector3>>();
        result.Add(CalculateTrajectory(AddSpread(_spreadAngelMegaShot, CalculateDirection()), out pointRefractionOne)
            .ToList());
        result.Add(CalculateTrajectory(AddSpread(-_spreadAngelMegaShot, CalculateDirection()), out pointRefractionTwo)
            .ToList());

        return new[]
        {
            result[0].ToArray(),
            result[1].ToArray()
        };
    }

    private Vector3 AddSpread(float spreadAngel, Vector3 direction)
    {
        spreadAngel = (float) (Math.PI / 180 * spreadAngel);

        float x = (float) (direction.x * Math.Cos(spreadAngel) - direction.y * Math.Sin(spreadAngel));
        float y = (float) (direction.x * Math.Sin(spreadAngel) + direction.y * Math.Cos(spreadAngel));

        return new Vector3(x, y, direction.z);
    }

    private Vector3[] ReduceArray(Vector3[] points)
    {
        Vector3[] result = new Vector3[15];
        int index = 0;

        for (int i = 0; i < points.Length; i += _trajectoryAccuracy)
            result[index++] = points[i];

        return result;
    }

    private Vector3[] RemovePointsBehindTheBalls(List<Vector3> points)
    {
        int startRemoveIndex = 0;

        for (int i = 0; i < points.Count; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(points[i], Vector3.forward);
            if (hit.transform && hit.transform.tag == BubbleTag) 
            {
                startRemoveIndex = i;
                break;
            }
        }
        
        if(startRemoveIndex != 0)
            points.RemoveRange(startRemoveIndex, points.Count - startRemoveIndex);
        
        return points.ToArray();
    }
}