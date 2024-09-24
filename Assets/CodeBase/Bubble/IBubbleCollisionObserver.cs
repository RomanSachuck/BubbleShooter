using UnityEngine;

public interface IBubbleCollisionObserver
{
    void HandleBubbleCollision(Collider2D other, Transform bubble, bool megaShot);
    void HandleMissBubbleCollision();
}