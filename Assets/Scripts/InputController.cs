using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera formCamera;
    [SerializeField] private Camera freeMainCamera;
    [SerializeField] private Camera FreeFormCamera;
    
    [SerializeField] private GameObject spleenPrefab;
    [SerializeField] private float incrementStepSpeed;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject buttonCurve;

    [SerializeField] private LayerMask planeMask;
    [SerializeField] private LayerMask pointMask;

    [SerializeField] private LayerMask planeFormMask;
    [SerializeField] private LayerMask pointFormMask;

    private List<Button_ID> _buttonList;
    private static List<CastelJaun> _spleenList;
    private static CastelJaun _spleenForm;
    private static CastelJaun _selectedSpleen;
    private Camera _currentCamera;
    private bool _keydownStep;
    private Ray _ray;
    private Ray _rayForm;

    [SerializeField] private GameObject mesh;
    
    private GameObject _pointSelected;
    private bool _isPointClicked;
    private Vector3 _selectedPointPosition;

    private bool _isTranslating;
    private Vector3 _prevMousePos;

    private GameObject _meshRevolution = null;
    private GameObject _meshExtrusion = null;
    
    private void Start()
    {
        _spleenList = new List<CastelJaun>();
        _buttonList = new List<Button_ID>();
        InstantiateNewCastelJaun();
        _isTranslating = false;
        _prevMousePos = Vector3.zero;
        _currentCamera = formCamera;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            var newMesh = Instantiate(mesh, new Vector3(0, 0, 0), Quaternion.identity);
            newMesh.GetComponent<BezierMesh>().BuildMesh(_spleenList[_spleenList.Count - 1].gameObject, spleenPrefab);
            _meshRevolution = newMesh;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            var newMesh = Instantiate(mesh, new Vector3(0, 0, 0), Quaternion.identity);
            newMesh.GetComponent<BezierMesh>().BuildMeshAroundBezier(_spleenForm, _spleenList[_spleenList.Count - 1].gameObject, spleenPrefab);
            _meshExtrusion = newMesh;
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            C0();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            C1();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (mainCamera.gameObject.activeSelf)
            {
                mainCamera.gameObject.SetActive(false);
                formCamera.gameObject.SetActive(true);
                _currentCamera = formCamera;
            }
            else
            {
                formCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                _currentCamera = mainCamera;
            }
        }
        
        ControlMouseClickedObject();
        MousePosition();
        StepController();
        NewCurve();
        SelectCurve();
        DeleteCurve();
        
    }
 
    private void MousePosition()
    {
        _ray = _currentCamera.ScreenPointToRay(Input.mousePosition);
        
        if (_isPointClicked)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                foreach (var spleen in _spleenList.Where(spleen => spleen.RemovePoint(_pointSelected)))
                {
                    break;
                }

                _isPointClicked = false;
                _pointSelected = null;
            }
            else if (Input.GetMouseButton(1) &&
                     Physics.Raycast(_ray, out var raycastHit, float.MaxValue, planeMask) &&
                     _pointSelected != null)
            {
                _selectedPointPosition = _pointSelected.transform.position;
                _selectedPointPosition = raycastHit.point;
                _selectedPointPosition.z = 0.0f;
                _pointSelected.transform.position = _selectedPointPosition;

                _spleenList.ForEach(spleen => spleen.UpdatePoint(_pointSelected));
            } 
            else if (Input.GetMouseButton(1) &&
                     Physics.Raycast(_ray, out var raycastHitForm, float.MaxValue, planeFormMask) &&
                     _pointSelected != null)
            {
                _selectedPointPosition = _pointSelected.transform.position;
                _selectedPointPosition = raycastHitForm.point;
                _selectedPointPosition.x = 0.0f;
                _pointSelected.transform.position = _selectedPointPosition;

                _spleenForm.UpdatePoint(_pointSelected);
            }
        }
        
    }

    private void C0()
    {
        InstantiateNewCastelJaun();

        var points = _spleenList[_spleenList.Count - 2].GetPointsGameObjects();

        _selectedSpleen.AddPoint(points.Last());
    }

    private void C1()
    {
        InstantiateNewCastelJaun();
        _selectedSpleen.C1(_spleenList[_spleenList.Count - 2].GetPointsGameObjects());
    }
    
    public void NewDestroyPoint()
    {
        foreach (var spleen in _spleenList.Where(spleen => spleen.RemovePoint(_pointSelected)))
        {
            break;
        }

        _isPointClicked = false;
        _pointSelected = null;
    }

    private void ControlMouseClickedObject()
    {
        _ray = _currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _isTranslating = true;
                Physics.Raycast(_ray, out var raycastHit, float.MaxValue, planeMask);
                _prevMousePos = raycastHit.point;
            }
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.C))
        {
            if (Input.GetKeyUp(KeyCode.W) && _isTranslating)
            {
                _isTranslating = false;
                Physics.Raycast(_ray, out var raycastHit, float.MaxValue, planeMask);
                var curMousePos = raycastHit.point;
                var deltaPos = new Vector3(curMousePos.x - _prevMousePos.x,
                    curMousePos.y - _prevMousePos.y, 0);
                _selectedSpleen.Translate(deltaPos.x, deltaPos.y, deltaPos.z);
            }
        }
        else if(!_isTranslating)
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(_ray, out var raycastHit1, float.MaxValue, planeMask))
            {
                _selectedPointPosition = raycastHit1.point;
                _selectedPointPosition.z = 0;
                _selectedSpleen.AddControlPoint(_selectedPointPosition);
                if (_pointSelected != null)
                    _pointSelected.GetComponent<Point>().UnSelect();
                _isPointClicked = true;
                _pointSelected =
                    _selectedSpleen.GetPointsGameObjects()[_selectedSpleen.GetPointsGameObjects().Count - 1];
                _pointSelected.GetComponent<Point>().Select();

            } 
            else if (Input.GetMouseButtonDown(0) && Physics.Raycast(_ray, out var raycastHit2, float.MaxValue, planeFormMask))
            {
                _selectedPointPosition = raycastHit2.point;
                _selectedPointPosition.x = 0;
                _spleenForm.AddControlPoint(_selectedPointPosition);
                if (_pointSelected != null)
                    _pointSelected.GetComponent<Point>().UnSelect();
                _isPointClicked = true;
                _pointSelected =
                    _spleenForm.GetPointsGameObjects()[_spleenForm.GetPointsGameObjects().Count - 1];
                _pointSelected.GetComponent<Point>().Select();
            }
            else if (Input.GetMouseButtonDown(1) && Physics.Raycast(_ray, out var raycastHit3, float.MaxValue, pointMask))
            {
                if (_pointSelected != null)
                    _pointSelected.GetComponent<Point>().UnSelect();
                _isPointClicked = true;
                _pointSelected = raycastHit3.transform.gameObject;
                _pointSelected.GetComponent<Point>().Select();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (_pointSelected == null) return;
                foreach (var spleen in _spleenList.Where(spleen => spleen.HasPoint(_pointSelected)))
                {
                    if (spleen.IsLastOrFirst(_pointSelected))
                    {
                        foreach (var spleenBis in _spleenList)
                        {
                            if (spleenBis == spleen) continue;
                            var pointBis = spleenBis.ComparePosition(_pointSelected);

                        if (pointBis == null) continue;

                        if (pointBis == _pointSelected) break;
                        
                        spleen.FusionBezier(spleenBis, _pointSelected, pointBis);
                        RemoveButton(spleenBis);
                        _spleenList.Remove(spleenBis);

                        // Destroy(_pointSelected.gameObject);
                            Destroy(spleenBis.gameObject);
                            Destroy(spleenBis);
                            _isPointClicked = false;
                            _pointSelected.GetComponent<Point>().UnSelect();
                            _pointSelected = null;
                            break;
                        }
                    }

                    break;
                }
            }
        }
    }

    private void RemoveButton(CastelJaun cj)
    {
        int id = _spleenList.IndexOf(cj);
        Debug.Log(id);
        GameObject _go = _buttonList[_buttonList.Count - 1].gameObject;
        _buttonList.RemoveAt(_buttonList.Count - 1);
        Destroy(_go);
    }

    public void NewStepController(bool choice)
    {
        StartCoroutine(SetNewStep(choice));
    }

    private void StepController()
    {
        if (_keydownStep) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _keydownStep = true;
            StartCoroutine(SetNewStep(false));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _keydownStep = true;
            StartCoroutine(SetNewStep(true));
        }
    }

    private void NewCurve()
    {
        if (!Input.GetKeyDown(KeyCode.Return) || _spleenList.Count > 7) return;
        InstantiateNewCastelJaun();
    }

    public void NewCurveButton()
    {
        InstantiateNewCastelJaun();
    }

    private Button_ID CreateButton(int _id)
    {
        GameObject _go = Instantiate(buttonCurve, content.transform);
        _go.GetComponent<Button_ID>().id = _id;
        TMP_Text _text = _go.GetComponentInChildren<TMP_Text>();
        _text.text = _go.name + " " + (_id);
        return _go.GetComponent<Button_ID>();
    }

    public static void NewSelectCurve(int id)
    {
        _selectedSpleen = _spleenList[id];
    }

    private void SelectCurve()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && _spleenList.Count > 0)
        {
            _selectedSpleen = _spleenList[0];
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && _spleenList.Count > 1)
        {
            _selectedSpleen = _spleenList[1];
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && _spleenList.Count > 2)
        {
            _selectedSpleen = _spleenList[2];
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && _spleenList.Count > 3)
        {
            _selectedSpleen = _spleenList[3];
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && _spleenList.Count > 4)
        {
            _selectedSpleen = _spleenList[4];
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && _spleenList.Count > 5)
        {
            _selectedSpleen = _spleenList[5];
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && _spleenList.Count > 6)
        {
            _selectedSpleen = _spleenList[6];
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) && _spleenList.Count > 7)
        {
            _selectedSpleen = _spleenList[7];
        }
    }

    private void DeleteCurve()
    {
        if (_spleenList.Count == 1) return;
        if (!Input.GetKeyDown(KeyCode.Backspace) || _isPointClicked) return;
        var index = _spleenList.IndexOf(_selectedSpleen);

        _spleenList.Remove(_selectedSpleen);
        _selectedSpleen.RemovePoints();
        Destroy(_selectedSpleen.gameObject);

        if (index == _spleenList.Count)
        {
            index--;
        }
    }

    private IEnumerator SetNewStep(bool step)
    {
        while (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            _selectedSpleen.ChangeStep(step);
            yield return new WaitForSeconds(incrementStepSpeed);
        }

        _keydownStep = false;
    }

    private void InstantiateNewCastelJaun()
    {
        _spleenForm = Instantiate(Instantiate(spleenPrefab).GetComponent<CastelJaun>());
        _spleenList.Add(Instantiate(spleenPrefab).GetComponent<CastelJaun>());
        _buttonList.Add(CreateButton(_spleenList.Count - 1));
        _selectedSpleen = _spleenList[_spleenList.Count - 1];
    }

    public void DestroyMeshExtrusion()
    {
        if (_meshExtrusion != null)
        {
            Destroy(_meshExtrusion);
            _meshExtrusion = null;
        }        
    }
    
    public void DestroyMeshRevolution()
    {
        if (_meshRevolution != null)
        {
            Destroy(_meshRevolution);
            _meshRevolution = null;
        }
    }
    public void BuildNewMeshRevolution()
    {
        if (_meshRevolution != null)
        {
            Destroy(_meshRevolution);
        }
        var newMesh = Instantiate(mesh, new Vector3(0, 0, 0), Quaternion.identity);
        newMesh.GetComponent<BezierMesh>().BuildMesh(_spleenList[_spleenList.Count - 1].gameObject, spleenPrefab);
        _meshRevolution = newMesh;
    }
    
    public void BuildNewMeshExtrusion()
    {
        if (_meshExtrusion != null)
        {
            Destroy(_meshExtrusion);
        }
        var newMesh = Instantiate(mesh, new Vector3(0, 0, 0), Quaternion.identity);
        newMesh.GetComponent<BezierMesh>().BuildMeshAroundBezier(_spleenForm, _spleenList[_spleenList.Count - 1].gameObject, spleenPrefab);
        _meshExtrusion = newMesh;
    }

    public void SwitchCameraFreeForm()
    {
        mainCamera.gameObject.SetActive(false);
        formCamera.gameObject.SetActive(false);
        freeMainCamera.gameObject.SetActive(false);
        FreeFormCamera.gameObject.SetActive(true);
    }
    
    public void SwitchCameraFree()
    {
        mainCamera.gameObject.SetActive(false);
        formCamera.gameObject.SetActive(false);
        freeMainCamera.gameObject.SetActive(true);
        FreeFormCamera.gameObject.SetActive(false);
    }
    
    public void SwitchCameraForm()
    {        
        mainCamera.gameObject.SetActive(false);
        formCamera.gameObject.SetActive(true);
        freeMainCamera.gameObject.SetActive(false);
        FreeFormCamera.gameObject.SetActive(false);
        _currentCamera = formCamera;
    }
    
    public void SwitchCamera()
    {
        mainCamera.gameObject.SetActive(true);
        formCamera.gameObject.SetActive(false);
        freeMainCamera.gameObject.SetActive(false);
        FreeFormCamera.gameObject.SetActive(false);
        _currentCamera = mainCamera;
    }
}