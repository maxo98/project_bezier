using UnityEngine;
public class Point : MonoBehaviour
{
    private Material _pointMaterial;
    
    private Color _pointDefaultColor;
    private Color _pointHoverColor;

    private void Awake()
    {
        _pointMaterial = GetComponent<MeshRenderer>().material;
        _pointDefaultColor = _pointMaterial.color;
        _pointHoverColor = Color.red;
    }
    
    private void OnMouseOver()
    {
        _pointMaterial.color = _pointHoverColor;
    }

    private void OnMouseExit()
    {
        _pointMaterial.color = _pointDefaultColor;
    }
}
