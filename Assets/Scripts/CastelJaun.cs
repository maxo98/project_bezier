using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CastelJaun : MonoBehaviour
{
    [SerializeField] private List<Vector3> controlPoints; 
    private List<List<Vector3>> _temporaryPoints = new List<List<Vector3>>();

    [SerializeField]
    private List<Vector3> bezierSpleen = new List<Vector3>();

    [SerializeField] 
    private int k;

    [SerializeField] 
    private LineRenderer controlPointLineRenderer;
    [SerializeField]
    private LineRenderer bezierLineRenderer;

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

    private void renderCurves()
    {
        controlPointLineRenderer.positionCount = 0;
        bezierLineRenderer.positionCount = 0;
        for (var i = 0; i < controlPoints.Count; i++)
        {
            controlPointLineRenderer.positionCount = controlPoints.Count;
            controlPointLineRenderer.SetPosition(i, controlPoints[i]);
        }
        
        for (var i = 0; i < bezierSpleen.Count; i++)
        {
            bezierLineRenderer.positionCount = bezierSpleen.Count;
            bezierLineRenderer.SetPosition(i, bezierSpleen[i]);
        }
    }
    
    
    public void AddControlPoint(Vector3 point)
    {
        point.z = 0;
        controlPoints.Add(point);
        if (controlPoints.Count <= 2) return;
        GetCastelJaun();
        renderCurves();
    }

    public void ChangeStep(bool step)
    {
        k = step ? k + 1 : k - 1;
        if (controlPoints.Count <= 2) return;
        GetCastelJaun();
        renderCurves();
    }
}
