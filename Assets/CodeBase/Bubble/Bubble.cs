using System;
using DG.Tweening;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private const string FloorTag = "Floor";
    
    [SerializeField] private int _numberOfPoints;
    [SerializeField] private BubbleType _type;
    public BubbleType Type => _type;

    public event Action<int> OnBrust;
    public Coords CellCoords;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == FloorTag)
            Burst();
    }

    private void OnJointBreak2D(Joint2D brokenJoint) =>
        brokenJoint.attachedRigidbody.velocity = Vector2.zero;

    public void Burst()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(Vector3.zero, .1f))
            .AppendCallback(() => Destroy(gameObject))
            .AppendCallback(() => OnBrust?.Invoke(_numberOfPoints));
    }
}