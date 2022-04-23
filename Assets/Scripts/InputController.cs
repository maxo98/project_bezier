using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject spleenPrefab;
    [SerializeField] private float incrementStepSpeed;

    public Vector3 position;

    private List<CastelJaun> _spleenList;
    private CastelJaun _selectedSpleen;
    private bool _keydownStep;
    private Ray _ray;
    private readonly Vector3 _offset = new Vector3(0,0,0);

    private void Start()
    {
        _spleenList = new List<CastelJaun> { Instantiate(spleenPrefab).GetComponent<CastelJaun>() };
        _selectedSpleen = _spleenList[0];
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
        if (!Physics.Raycast(_ray, out var raycastHit, float.MaxValue, layerMask)) return;
        if (!Input.GetMouseButtonDown(0)) return;
        position = raycastHit.point + _offset;
        _selectedSpleen.AddControlPoint(position);
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
        _spleenList.Add(Instantiate(spleenPrefab).GetComponent<CastelJaun>());
        _selectedSpleen = _spleenList[_spleenList.Count - 1];
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
        if (!Input.GetKeyDown(KeyCode.Backspace)) return;
        var index = _spleenList.IndexOf(_selectedSpleen);
        _spleenList.Remove(_selectedSpleen);
        Destroy(_selectedSpleen.gameObject);
        if (index == _spleenList.Count)
        {
            index--;
        }
        _selectedSpleen = _spleenList[index];
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
}
