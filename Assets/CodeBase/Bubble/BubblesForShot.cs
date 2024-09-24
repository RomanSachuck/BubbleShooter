using System;
using System.Collections.Generic;
using UnityEngine;

public class BubblesForShot : MonoBehaviour, IBubbleCollisionObserver
{
    [SerializeField] private SpriteRenderer _modelSprite;
    [SerializeField] private Sprite[] _bubbleSprites;
    public BubbleType CurrentBubble { get; private set; }
    public int NumberRemainingBubbles => _bubbles.Count;
    public event Action<int, Sprite> OnChargeNextBubble;

    private Queue<BubbleType> _bubbles;
    private Vector3 _startPosition;

    private void Awake() =>
        _startPosition = transform.position;

    public void Initialize(int[] bubbles)
    {
        _bubbles = new Queue<BubbleType>();

        for (int i = 0; i < bubbles.Length; i++)
            _bubbles.Enqueue((BubbleType) bubbles[i]);

        ChargeNextBubble();
    }

    public void HandleBubbleCollision(Collider2D other, Transform bubble, bool megaShot) =>
        ChargeNextBubble();

    public void HandleMissBubbleCollision() =>
        ChargeNextBubble();

    private void ChargeNextBubble()
    {
        if (_bubbles.Count != 0)
        {
            transform.position = _startPosition;
            CurrentBubble = _bubbles.Dequeue();
            _modelSprite.sprite = _bubbleSprites[(int) CurrentBubble];
        
            if(_bubbles.Count == 0)
                OnChargeNextBubble?.Invoke(_bubbles.Count, _bubbleSprites[(int) CurrentBubble]);
            else
                OnChargeNextBubble?.Invoke(_bubbles.Count, _bubbleSprites[(int) _bubbles.Peek()]);
        }
        else
            OnChargeNextBubble?.Invoke(-1, _bubbleSprites[(int) CurrentBubble]);
    }
}