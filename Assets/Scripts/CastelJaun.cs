using System.Collections.Generic;
using UnityEngine;

public class CastelJaun : MonoBehaviour
{
    [SerializeField] private List<Vector3> controlPoints; 
    private List<List<Vector3>> _temporaryPoints = new List<List<Vector3>>();
    
    [SerializeField]
    private List<Vector3> bezierSpleen = new List<Vector3>();

    [SerializeField]
    private int k;

    [SerializeField] 
    private GameObject prefabPoint;
    
    private List<GameObject> _pointsGameObjects;
    
    [SerializeField] 
    private LineRenderer controlPointLineRenderer;
    [SerializeField]
    private LineRenderer bezierLineRenderer;

    private double DELTA = 0.5;
    
    private void Start()
    {
        _pointsGameObjects = new List<GameObject>();
    }
    
    private void GetCastelJaun()
    {
        bezierSpleen.Clear();
        
        for (float t = 0; t < 1 ; t += 1/(float)k)
        {
            _temporaryPoints.Clear();
            _temporaryPoints.Add(controlPoints);
            
            for (var j = 1; j < controlPoints.Count; j++)
            {
                var tmp = new List<Vector3>();
                
                for (var i = 0; i < controlPoints.Count - j; i++)
                {
                    tmp.Add((1 - t) * _temporaryPoints[j - 1][i] + t * _temporaryPoints[j - 1][i + 1]);
                }
                
                _temporaryPoints.Add(new List<Vector3>(tmp));
                tmp.Clear();
            }
            
            bezierSpleen.Add(_temporaryPoints[controlPoints.Count - 1][_temporaryPoints[controlPoints.Count - 1].Count - 1]);
        }
        
        bezierSpleen.Add(controlPoints[controlPoints.Count - 1]);
    }

    private void RenderCurves()
    {
        controlPointLineRenderer.positionCount = 0;
        bezierLineRenderer.positionCount = 0;
        
        for (var i = 0; i < _pointsGameObjects.Count; i++)
        {
            controlPointLineRenderer.positionCount = _pointsGameObjects.Count;
            controlPointLineRenderer.SetPosition(i, _pointsGameObjects[i].transform.position);
        }
        
        for (var i = 0; i < bezierSpleen.Count; i++)
        {
            bezierLineRenderer.positionCount = bezierSpleen.Count;
            bezierLineRenderer.SetPosition(i, bezierSpleen[i]);
        }
    }

    public void RemovePoints()
    {
        foreach (var point in _pointsGameObjects)
        {
            Destroy(point.gameObject);
        }
    }
    
    public bool RemovePoint(GameObject point)
    {
        var idx = _pointsGameObjects.IndexOf(point);

        if (idx <= -1) 
            return false;

        _pointsGameObjects.Remove(point);
        controlPoints.Remove(point.transform.position);
        
        Destroy(point);
        Destroy(point.gameObject);
        
        GetCastelJaun();
        RenderCurves();
        
        return true;
    }

    public int Position(GameObject point)
    {
        return _pointsGameObjects.IndexOf(point);
    }

    public int GetLength()
    {
        return _pointsGameObjects.Count;
    }

    public void Reverse()
    {
        _pointsGameObjects.Reverse();
        controlPoints.Reverse();
    }

    public List<GameObject> GetPointsGameObjects()
    {
        return _pointsGameObjects;
    }

    public List<Vector3> GetControlPoints()
    {
        return controlPoints;
    }
    
    private bool SamePosition(GameObject pointA, GameObject pointB)
    {
        if ((pointA.transform.position - pointB.transform.position).sqrMagnitude < (DELTA * DELTA))
        {
            return true;
        }
        return false;
    }

    public void FusionBezier(CastelJaun bezier, GameObject pointA, GameObject pointB)
    {
        if (_pointsGameObjects.IndexOf(pointA) == 0)
        {
            Debug.Log("a");
            Debug.Log($"{_pointsGameObjects.IndexOf(pointA)} <==> {_pointsGameObjects.Count - 1}");
            Reverse();
        }
        
        if (bezier.Position(pointB) == bezier.GetLength() - 1)
        {
            Debug.Log("b");
            Debug.Log($"{bezier.Position(pointB)} <==> {bezier.GetLength() - 1}");
            bezier.Reverse();
        }
        
        bezier.GetPointsGameObjects().ForEach(item => _pointsGameObjects.Add(item));
        bezier.GetControlPoints().ForEach(item => controlPoints.Add(item));

        GetCastelJaun();
        RenderCurves();
    }
    
    public GameObject ComparePosition(GameObject point)
    {
        foreach (var _point in _pointsGameObjects)
        {
            if(SamePosition(point, _point) && IsLastOrFirst(_point))
            {
                return _point;
            }
        }

        return null;
    }
    public bool IsLastOrFirst(GameObject point)
    {
        return _pointsGameObjects.IndexOf(point) == 0 || _pointsGameObjects.IndexOf(point) == _pointsGameObjects.Count - 1;
    }
    public bool HasPoint(GameObject point)
    {
        return _pointsGameObjects.IndexOf(point) > -1;
    }
    public bool UpdatePoint(GameObject point)
    {
        var idx = _pointsGameObjects.IndexOf(point);

        if (idx <= -1) 
            return false;

        _pointsGameObjects[idx] = point;
        controlPoints[idx] = point.transform.position;
        
        GetCastelJaun();
        RenderCurves();

        return true;
    }
    
    public void AddControlPoint(Vector3 point)
    {
        Debug.Log("here");
        point.z = 0;
        _pointsGameObjects.Add(CreateNewPoint(point));
        controlPoints.Add(point);
        
        if (controlPoints.Count <= 2) return;
        if (_pointsGameObjects.Count <= 2) return;
        
        GetCastelJaun();
        RenderCurves();
    }

    public void ChangeStep(bool step)
    {
        k = step ? k + 1 : k - 1;
        
        if (controlPoints.Count <= 2) return;
        if (_pointsGameObjects.Count <= 2) return;
        
        GetCastelJaun();
        RenderCurves();
    }

    private GameObject CreateNewPoint(Vector3 position)
    {
        GameObject point = Instantiate(prefabPoint);
        point.transform.position = position;
        return point;
    }
}
