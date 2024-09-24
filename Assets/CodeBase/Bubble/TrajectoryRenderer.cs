using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineTensionLeft;
    [SerializeField] private LineRenderer _lineTensionRight;
    [SerializeField] private GameObject _blackPointPrefab;
    [SerializeField] private GameObject _redPointPrefab;
    
    private List<GameObject> _trajectoryPoints = new List<GameObject>();
    private List<GameObject> _oneTrajectoryPointsMegaShot = new List<GameObject>();
    private List<GameObject> _twoTrajectoryPointsMegaShot = new List<GameObject>();

    public void ShowTension(Vector3 position)
    {
        _lineTensionLeft.enabled = true;
        _lineTensionRight.enabled = true;
        
        position.z = 1;
        
        _lineTensionLeft.SetPosition(0, _lineTensionLeft.transform.position);
        _lineTensionLeft.SetPosition(1, position);
        
        _lineTensionRight.SetPosition(0, _lineTensionRight.transform.position);
        _lineTensionRight.SetPosition(1, position);
    }
    
    public void HideTension()
    {
        _lineTensionLeft.enabled = false;
        _lineTensionRight.enabled = false;
    }

    public void ShowTrajectory(Vector3[] points)
    {
        HideTrajectory();
        
        if (points.Length > _trajectoryPoints.Count)
            IncreasePullPoints(points.Length - _trajectoryPoints.Count, _trajectoryPoints, _blackPointPrefab);

        ShowPoints(_trajectoryPoints, points);
    }

    public void ShowTrajectory(Vector3[][] points)
    {
        HideTrajectory();
        
        if (points[0].Length > _oneTrajectoryPointsMegaShot.Count)
            IncreasePullPoints(points[0].Length - _oneTrajectoryPointsMegaShot.Count, _oneTrajectoryPointsMegaShot, _redPointPrefab);
        if (points[1].Length > _twoTrajectoryPointsMegaShot.Count)
            IncreasePullPoints(points[1].Length - _twoTrajectoryPointsMegaShot.Count, _twoTrajectoryPointsMegaShot, _redPointPrefab);
        
        ShowPoints(_oneTrajectoryPointsMegaShot, points[0]);
        ShowPoints(_twoTrajectoryPointsMegaShot, points[1]);
    }
    
    public void HideTrajectory()
    {
        HideSimpleTragectory();
        HideMegaTragectory();
    }

    private void HideSimpleTragectory()
    {
        for (int i = 0; i < _trajectoryPoints.Count; i++)
            _trajectoryPoints[i].SetActive(false);
    }

    private void HideMegaTragectory()
    {
        for (int i = 0; i < _oneTrajectoryPointsMegaShot.Count; i++)
            _oneTrajectoryPointsMegaShot[i].SetActive(false);

        for (int i = 0; i < _twoTrajectoryPointsMegaShot.Count; i++)
            _twoTrajectoryPointsMegaShot[i].SetActive(false);
    }

    private void ShowPoints(List<GameObject> trajectoryPoints, Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            trajectoryPoints[i].SetActive(true);
            trajectoryPoints[i].transform.position = points[i];
        }
    }

    private void IncreasePullPoints(int size, List<GameObject> points, GameObject pointPrefab)
    {
        for (int i = 0; i < size; i++)
            points.Add(Instantiate(pointPrefab));
    }
}
