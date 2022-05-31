using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ID : MonoBehaviour
{
    public int id;

    public void Selected()
    {
        InputController.NewSelectCurve(id);
    }
}
