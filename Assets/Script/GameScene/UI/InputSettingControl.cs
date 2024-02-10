using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingControl : MonoBehaviour
{
    public Text txInfo;
    public Button btnChange;

    public void Awake()
    {
        btnChange = GetComponentInChildren<Button>();
        txInfo = transform.Find("TextInfo").GetComponent<Text>();
    }
    
    
}
