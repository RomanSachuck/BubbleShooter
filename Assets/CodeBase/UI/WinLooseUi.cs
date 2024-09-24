using UnityEngine;

public class WinLooseUi : MonoBehaviour
{
    [SerializeField] private Bootstrap _bootstrap;
    
    [SerializeField] private GameObject _victoryText;
    [SerializeField] private GameObject _looseText;
    
    public void OnWin()
    {
        gameObject.SetActive(true);
        _looseText.SetActive(false);
        _victoryText.SetActive(true);
    }
    
    public void OnLoose()
    {
        gameObject.SetActive(true);
        _victoryText.SetActive(false);
        _looseText.SetActive(true);
    }

    public void ButtonRestart()
    {
        gameObject.SetActive(false);
        _bootstrap.Restart();
    }

    public void ButtonMenu()
    {
        
    }
}