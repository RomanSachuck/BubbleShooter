using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private GameFIeld _gameField;
    [SerializeField] private TextMeshProUGUI _scoreNumber;

    private int _score;
    
    private void OnEnable() =>
        _gameField.OnAddScore += Addscore;

    private void OnDisable() =>
        _gameField.OnAddScore -= Addscore;

    private void Addscore(int score)
    {
        _score += score;
        _scoreNumber.text = _score.ToString();
    } 
}
