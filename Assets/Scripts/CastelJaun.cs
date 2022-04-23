using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CastelJaun : MonoBehaviour
{
    [SerializeField] private Vector3[] controlPoints;
     private Vector3[][] temporaryPoints = new Vector3[10][];

     [SerializeField]
     private List<Vector3> bezierSpleen = new List<Vector3>();

    [SerializeField] 
    private int k;

    [SerializeField] 
    private LineRenderer lineRenderer;

    private bool _doOnce = true;

    private void Start()
    {
        temporaryPoints[0] = controlPoints;
        
    }

    private void Update()
    {
        if (!_doOnce) return;
        _doOnce = false;
        GetCastelJaun();
        
    }
*
    private void GetCastelJaun()
    {
        for (float t = 0; t < 1; t += 1/(float)k)
        {
            var max = 0;
            for (var j = 1; j < controlPoints.Length; j++)
            {
                temporaryPoints[j] = new Vector3[controlPoints.Length];
                for (var i = 0; i < controlPoints.Length - j; i++)
                {
                    temporaryPoints[j][i] = new Vector3((1 - t) * temporaryPoints[j-1][i].x + t * temporaryPoints[j-1][i].x,
                        (1 - t) * temporaryPoints[j-1][i].y + t * temporaryPoints[j-1][i].y, 0f);
                }
                max = j;
            }
            bezierSpleen.Add(temporaryPoints[max][0]);
            
        }

        foreach (var variable in bezierSpleen)
        {
            Debug.Log(variable);
            lineRenderer.SetPositions(bezierSpleen.ToArray());
        }
        
    }
    
}
