using UnityEngine;

public class ButtonExit : MonoBehaviour
{
    [SerializeField] private GameObject _exitContainer; 

    public void PressButtonExit() =>
        _exitContainer.SetActive(true);

    public void PressButtonYes() =>
        Exit();
    
    public void PressButtonNo() =>
        _exitContainer.SetActive(false);

    private void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }
}
