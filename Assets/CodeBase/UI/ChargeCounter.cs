using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargeCounter : MonoBehaviour
{
    [SerializeField] private BubblesForShot _bubblesForShot;
    [SerializeField] private Image _nextBubbleImage;
    [SerializeField] private TextMeshProUGUI _textCounter;
    
    private void OnEnable() =>
        _bubblesForShot.OnChargeNextBubble += UpdateValue; 

    private void OnDisable() =>
        _bubblesForShot.OnChargeNextBubble -= UpdateValue; 

    private void UpdateValue(int value, Sprite nextBubble)
    {
        if (value < 0)
            value = 0;
        
        _textCounter.text = value.ToString();
        _nextBubbleImage.sprite = nextBubble;
    }
}