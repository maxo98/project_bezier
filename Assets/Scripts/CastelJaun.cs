using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastelJaun : MonoBehaviour
{
    [SerializeField] private List<Vector3> controlPoints; 
    private List<List<Vector3>> _temporaryPoints = new List<List<Vector3>>();
    
    [SerializeField]
    private List<Vector3> bezierSpleen = new List<Vector3>();

    [SerializeField]
    private int k;

    // [SerializeField]
    public GameObject prefabPoint;
    
    private List<GameObject> _pointsGameObjects = new List<GameObject>();
    
    [SerializeField] 
    private LineRenderer controlPointLineRenderer;
    [SerializeField]
    private LineRenderer bezierLineRenderer;
    
    [SerializeField] private Matrix4x4 translate;
    [SerializeField] private Matrix4x4 rotate;
    [SerializeField] private Matrix4x4 scale;

    private double DELTA = 0.5;

    public void GetCastelJaun()
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
    
    public void C1(List<GameObject> points)
    {
        var lastPoint = points.Last();
        var secondLast = points[points.Count - 2];

        var newPoint = Instantiate(prefabPoint);

        var newPosition = new Vector3( 2 * lastPoint.transform.position.x - secondLast.transform.position.x , 2 * lastPoint.transform.position.y - secondLast.transform.position.y,0);
        
        newPoint.transform.position = newPosition;

        AddPoint(points.Last());
        AddPoint(newPoint);
    }

    public void RenderCastleJaun()
    {
        GetCastelJaun();
        RenderCurves();
    }

    public void SetNewPoints(List<GameObject> points)
    {
        foreach (var point in points)
        {
            var newPoint = Instantiate(prefabPoint, point.transform.position, Quaternion.identity);
            newPoint.transform.parent = transform;
            _pointsGameObjects.Add(newPoint);
        }

        foreach (var point in _pointsGameObjects)
        {
            controlPoints.Add(point.transform.position);
            point.SetActive(false);
        }
    }
    
    public void SetPoints(List<GameObject> points)
    {
        _pointsGameObjects = points;

        foreach (var point in points)
        {
            controlPoints.Add(point.transform.position);
        }
    }
    
    public void AddPoint(GameObject point)
    {
        _pointsGameObjects.Add(point);
        controlPoints.Add(point.transform.position);

        if (controlPoints.Count <= 2) return;
        if (_pointsGameObjects.Count <= 2) return;
        
        GetCastelJaun();
        RenderCurves();
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

    public List<Vector3> GetBezierSpleen()
    {
        return bezierSpleen;
    }
    public List<GameObject> GetPointsGameObjects()
    {
        return _pointsGameObjects;
    }

    public List<Vector3> GetControlPoints()
    {
        return controlPoints;
    }

    public void SetControlPoints(List<Vector3> points)
    {
        controlPoints = points;
    }
    
    private bool SamePosition(GameObject pointA, GameObject pointB)
    {
        return (pointA.transform.position - pointB.transform.position).sqrMagnitude < (DELTA * DELTA);
    }

    public void FusionBezier(CastelJaun bezier, GameObject pointA, GameObject pointB)
    {
        if (_pointsGameObjects.IndexOf(pointA) == 0)
        {
            Reverse();
        }
        
        if (bezier.Position(pointB) == bezier.GetLength() - 1)
        {
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

    public bool UpdatePointPosition(GameObject point)
    {
        var idx = _pointsGameObjects.IndexOf(point);

        if (idx <= -1) 
            return false;
        
        controlPoints[idx] = point.transform.position;

        return true;
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
        if (_pointsGameObjects == null)
        {
            _pointsGameObjects = new List<GameObject>();
        }

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
        k = k <= 1 ? 1 : k;
        if (controlPoints.Count <= 2) return;
        if (_pointsGameObjects.Count <= 2) return;
        
        GetCastelJaun();
        RenderCurves();
    }

    private GameObject CreateNewPoint(Vector3 position)
    {
        GameObject point = Instantiate(prefabPoint);
        point.transform.position = position;
        point.transform.parent = transform;
        return point;
    }

    public void Translate(float x, float y, float z)
    {
        var translateVec = new Vector3(x, y, z);
        translate = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1 , 0, 0),
            new Vector4(0, 0, 1, 0), new Vector4(x, y, z, 1));
        foreach (var controlPoint in _pointsGameObjects)
        {
            controlPoint.transform.position += translateVec;
            //controlPoint.transform.position = translate.MultiplyVector(controlPoint.transform.position);
        }

        for (var i = 0; i < controlPoints.Count; i++)
        {
            //controlPoints[i] = translate.MultiplyVector(controlPoints[i]);
            controlPoints[i] += translateVec;
        }
        
        GetCastelJaun();
        RenderCurves();
    }
    
    public void Rotate(float angle)
    {
        rotate = new Matrix4x4(new Vector4((float)Math.Cos(angle), (float)-Math.Sin(angle), 0, 0),
            new Vector4((float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0), 
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1));
    }
    
    public void Scale(float x, float y, float z)
    {
        scale = new Matrix4x4(new Vector4(x, 0, 0, 0), new Vector4(0, y, 0, 0), 
            new Vector4(0, 0, z, 0), new Vector4(0, 0, 0, 1));
    }
    
}
