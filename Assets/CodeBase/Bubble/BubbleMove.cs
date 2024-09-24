using System.Collections.Generic;
using UnityEngine;

public class BubbleMove : MonoBehaviour
{
    private const string FloorTag = "Floor";
    private const string RoofTag = "Roof";
    private const string WallTag = "Wall";
    private const string BubbleTag = "Bubble";
    
    public bool IsMoving { get; private set; }
    
    private List<IBubbleCollisionObserver> _observers = new List<IBubbleCollisionObserver>();
    private Vector3 _movementVector;
    private bool _isMegaShot;

    public void AddObserver(IBubbleCollisionObserver observer) =>
        _observers.Add(observer);
    
    private void Update()
    {
        if (_movementVector != Vector3.zero)
        {
            transform.position += _movementVector * Time.deltaTime;
            _movementVector += Physics.gravity * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == WallTag)
            _movementVector.x *= -1;
        else if (other.tag == BubbleTag && IsMoving)
        {
            StopMovement();
            
            for (int i = 0; i < _observers.Count; i++)
                _observers[i].HandleBubbleCollision(other, transform, _isMegaShot);
        }
        else if (other.tag == FloorTag || other.tag == RoofTag)
        {
            StopMovement();
            for (int i = 0; i < _observers.Count; i++)
                _observers[i].HandleMissBubbleCollision();
        }
    }

    public void Move(Vector3 direction, float speed, bool megaShot)
    {
        IsMoving = true;
        _isMegaShot = megaShot;
        _movementVector = direction * speed;
    }

    private void StopMovement()
    {
        IsMoving = false;
        _movementVector = Vector3.zero;
    }
}
