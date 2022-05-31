using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject spleenPrefab;
    [SerializeField] private float incrementStepSpeed;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject buttonCurve;

    public Vector3 position;

    private static List<CastelJaun> _spleenList;
    private static CastelJaun _selectedSpleen;
    private bool _keydownStep;
    private Ray _ray;
    private readonly Vector3 _offset = new Vector3(0,0,0);

    private GameObject _pointSelected;
    private bool _isPointClicked;

    private void Start()
    {
        _spleenList = new List<CastelJaun>();
        InstantiateNewCastelJaun();
    }

    private void Update()
    {
        MousePosition();
        StepController();
        NewCurve();
        SelectCurve();
        DeleteCurve();
    }

    private void MousePosition()
    {
        _ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            ControlMouseClickedObject();
        }

        if (Input.GetMouseButton(0))
        {
            if (_isPointClicked)
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    foreach (var spleen in _spleenList.Where(spleen => spleen.RemovePoint(_pointSelected)))
                    {
                        break;
                    }

                    _pointSelected = null;
                }
                else if (Physics.Raycast(_ray, out var raycastHit, float.MaxValue, (1 << LayerMask.NameToLayer("Plane"))) && _pointSelected != null)
                {
                    position = _pointSelected.transform.position;
                    position = raycastHit.point + _offset;
                    position.z = 0.0f;
                    _pointSelected.transform.position = position;

                    foreach (var spleen in _spleenList.Where(spleen => spleen.UpdatePoint(_pointSelected)))
                    {
                        break;
                    }
                }
            }
        }
        
        if (!Input.GetMouseButton(0))
        {
            if (_pointSelected != null)
            {
                foreach (var spleen in _spleenList)
                {
                    if (spleen.HasPoint(_pointSelected))
                    {
                        if (spleen.IsLastOrFirst(_pointSelected))
                        {
                            foreach (var spleenBis in _spleenList)
                            {
                                if (spleenBis == spleen) continue;
                                GameObject pointBis = spleenBis.ComparePosition(_pointSelected);

                                if (pointBis == null) continue;
                                spleen.FusionBezier(spleenBis, _pointSelected, pointBis);
                                _spleenList.Remove(spleenBis);
    
                                // Destroy(_pointSelected.gameObject);
                                Destroy(spleenBis.gameObject);
                                Destroy(spleenBis);

                                break;
                            }
                        }

                        break;
                    }
                }
            }
        
            _isPointClicked = false;
            _pointSelected = null;
        }
    }
    
    private void ControlMouseClickedObject()
    {
        if (Physics.Raycast(_ray, out var raycastHit, float.MaxValue, (1 << LayerMask.NameToLayer("Point"))  | (1 << LayerMask.NameToLayer("Plane") | (1 << LayerMask.NameToLayer("UI") ))))
        {
            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Plane"))
            {
                position = raycastHit.point + _offset;
                _selectedSpleen.AddControlPoint(position);
            }

            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
            {
                _isPointClicked = true;
                _pointSelected = raycastHit.transform.gameObject;
            }
            Debug.Log(raycastHit.transform.gameObject.layer);
        }
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

    private void CreateButton(int _id)
    {
        GameObject _go = Instantiate(buttonCurve, content.transform);
        _go.GetComponent<Button_ID>().id = _id;
        TMP_Text _text = _go.GetComponentInChildren<TMP_Text>();
        _text.text = _go.name + " " + (_id);
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
        _spleenList.Add(Instantiate(spleenPrefab).GetComponent<CastelJaun>());
        _selectedSpleen = _spleenList[_spleenList.Count - 1];
        CreateButton(_spleenList.Count - 1);
    }
}
