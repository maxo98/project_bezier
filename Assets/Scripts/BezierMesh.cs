using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierMesh : MonoBehaviour
{
    [SerializeField]
    private int templatesIterator = 30;
    
    [SerializeField]
    private List<GameObject> _vertices;

    public void BuildMesh(GameObject spleen, GameObject prefabSpleen)
    {
        _vertices = new List<GameObject>();
        InitVertices(spleen, prefabSpleen);
        PositionVertices();
        DrawCastelJaun();
        BuildMesh();
    }
    
    public void BuildMeshAroundBezier(CastelJaun spleenPath, GameObject spleen, GameObject prefabSpleen)
    {
        _vertices = new List<GameObject>();
        InitVerticesAroundBezier(spleenPath, spleen, prefabSpleen);
        PositionVerticesAroundBezier(spleen);
        DrawCastelJaun();
        BuildBezierMesh(spleenPath);
    }

    private void InitVerticesAroundBezier(CastelJaun spleenPath, GameObject initSpleen, GameObject prefabSpleen)
    {
        var pointsPath = spleenPath.GetBezierSpleen();
        var lengthPath = pointsPath.Count;

        for (var i = 0; i < lengthPath; i++)
        {
            _vertices.Add(Instantiate(prefabSpleen, initSpleen.transform.position, Quaternion.identity));
            _vertices[i].GetComponent<CastelJaun>().SetNewPoints(new List<GameObject>(initSpleen.GetComponent<CastelJaun>().GetPointsGameObjects()));
            _vertices[i].transform.position = pointsPath[i];
        }
    }
    
    private void InitVertices(GameObject initSpleen, GameObject prefabSpleen)
    {
        _vertices.Add(initSpleen);
        
        for (var i = 1; i < templatesIterator; i++)
        {
            _vertices.Add(Instantiate(prefabSpleen, initSpleen.transform.position, Quaternion.identity));
            _vertices[i].GetComponent<CastelJaun>().SetPoints(new List<GameObject>(_vertices[0].GetComponent<CastelJaun>().GetPointsGameObjects()));
        }
    }

    private void PositionVerticesAroundBezier(GameObject initSpleen)
    {
        List<Vector3> pointPosition = new List<Vector3>();

        var listControlPoints = initSpleen.GetComponent<CastelJaun>().GetControlPoints();

        foreach (var controlPoint in listControlPoints)
        {
            var initSpleenPosition = initSpleen.transform.position;
            pointPosition.Add(new Vector3(controlPoint.x - initSpleenPosition.x, controlPoint.y - initSpleenPosition.y, controlPoint.z - initSpleenPosition.z));
        }

        for (var i = 0; i < _vertices.Count; i++)
        {
            List<GameObject> points = _vertices[i].GetComponent<CastelJaun>().GetPointsGameObjects();
            
            for (var j = 0; j < points.Count; j++)
            {
                var currentPoint = points[j];
                currentPoint.transform.position.Set(pointPosition[j].x + _vertices[i].transform.position.x, pointPosition[j].y + _vertices[i].transform.position.y, pointPosition[j].z + _vertices[i].transform.position.z);
                    
                _vertices[i].GetComponent<CastelJaun>().UpdatePointPosition(currentPoint);
                points[j] = currentPoint;
            }
        }
    }
    private void PositionVertices()
    {
        var angleRotation = 360 / templatesIterator;

        for (var i = 0; i < templatesIterator; i++)
        {
            List<GameObject> points = _vertices[i].GetComponent<CastelJaun>().GetPointsGameObjects();
            foreach (var point in points)
            {
                point.transform.RotateAround(new Vector3(10, 0, 0), Vector3.up, angleRotation);
                _vertices[i].GetComponent<CastelJaun>().UpdatePointPosition(point);
            }
        }
    }
    
    private void DrawCastelJaun()
    {
        foreach (var vertice in _vertices)
        {
            vertice.GetComponent<CastelJaun>().GetCastelJaun();
        }
    }

    private void BuildMesh()
    {
        var mesh = new Mesh();
        mesh.name = "mon magnifique bézier";
        GetComponent<MeshFilter>().mesh = mesh;

        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < _vertices.Count; i++)
        {
            
            var currentSpleen = _vertices[i].GetComponent<CastelJaun>().GetBezierSpleen();
            var nextSpleen = _vertices[(i + 1) % templatesIterator].GetComponent<CastelJaun>().GetBezierSpleen();

            for (int j = 0; j < currentSpleen.Count - 1; j++)
            {
                CreateMeshVertices(vertices, currentSpleen[j], nextSpleen[j], currentSpleen[j + 1], nextSpleen[j + 1]);
                CreateMeshTriangles(triangles, vertices);
            }
        }

        DrawMesh(mesh, vertices.ToArray(), triangles.ToArray());
    }

    private void BuildBezierMesh(CastelJaun spleenPath)
    {
        var mesh = new Mesh();
        mesh.name = "mon magnifique bézier";
        GetComponent<MeshFilter>().mesh = mesh;
        
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        var length = spleenPath.GetControlPoints().Count;
            
        for (int i = 0; i < _vertices.Count - 1; i++)
        {
            var currentSpleen = _vertices[i].GetComponent<CastelJaun>().GetBezierSpleen();
            Debug.Log(_vertices.Count % (i + 1));
            var nextSpleen = _vertices[i + 1].GetComponent<CastelJaun>().GetBezierSpleen();

            for (int j = 0; j < currentSpleen.Count - 1; j++)
            {
                CreateMeshVertices(vertices, currentSpleen[j], nextSpleen[j], currentSpleen[j + 1], nextSpleen[j + 1]);
                CreateMeshTriangles(triangles, vertices);
            }
        }

        DrawMesh(mesh, vertices.ToArray(), triangles.ToArray());
    }

    private void CreateMeshVertices(List<Vector3> vertices, Vector3 posA,Vector3 posB,Vector3 posC,Vector3 posD)
    {
        vertices.Add(posA);
        vertices.Add(posB);
        vertices.Add(posC);
        vertices.Add(posD);
    }

    private void CreateMeshTriangles(List<int> triangles, List<Vector3> vertices)
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
    }
    private void DrawMesh(Mesh mesh, Vector3[] vertices, int[] triangles)
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
    }
}
