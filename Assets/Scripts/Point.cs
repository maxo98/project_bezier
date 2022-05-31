using UnityEngine;
public class Point : MonoBehaviour
{
    [SerializeField] private MeshRenderer pointMesh;
    
    private Color _pointDefaultColor;
    private Color _pointHoverColor;

    private void Awake()
    {
        _pointDefaultColor = pointMesh.material.color;
        _pointHoverColor = Color.green;
    }
    
    public void Select()
    {
        pointMesh.material.color = _pointHoverColor;
    }

    public void UnSelect()
    {
        pointMesh.material.color = _pointDefaultColor;
    }
}
