using UnityEngine;

public class BubbleLauncher : MonoBehaviour
{
    [SerializeField] private TrajectoryCalculator _trajectoryCalculator;
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
    [SerializeField] private BubbleMove _bubbleMove;
    
    private Camera _camera;
    private WinLooseActor _winLooseActor;
    private bool _readyToLaunch;
    private bool _megaShot;

    public void Initialize(WinLooseActor winLooseActor) =>
        _winLooseActor = winLooseActor;
    
    private void Awake() => 
        _camera = Camera.main;

    private void Update()
    {
        if (Input.GetMouseButton(0) && !_bubbleMove.IsMoving && !_winLooseActor.GameOver)
        {
            _readyToLaunch = true;
            PullTheBall();
            _trajectoryRenderer.ShowTension(transform.position);

            if (_megaShot)
                _trajectoryRenderer.ShowTrajectory(_trajectoryCalculator.CalculateMegaTrajectory());
            else
                _trajectoryRenderer.ShowTrajectory(_trajectoryCalculator.CalculateTrajectory());
        }
        else if (_readyToLaunch)
        {
            _readyToLaunch = false;
            Launch();
            _trajectoryRenderer.HideTension();
            _trajectoryRenderer.HideTrajectory();
        }
    } 

    private void PullTheBall()
    {
        Vector3 cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0;

        if (!_trajectoryCalculator.FullTension(cursorPosition))
        {
            transform.position = cursorPosition;
            _megaShot = false;
        }
        else
        {
            transform.position = _trajectoryCalculator.CalculateFullTensionPosition(cursorPosition);
            _megaShot = true;
        }
    }
    
    private void Launch()
    {
        Vector3 direction = _trajectoryCalculator.CalculateDirection(_megaShot);
        float speed = _trajectoryCalculator.CalculateSpeed();
        _bubbleMove.Move(direction, speed, _megaShot);
    }
}
