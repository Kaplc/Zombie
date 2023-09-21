using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour
{
    private Button btn;
    private int id;
    private string money;

    private void Start()
    {
        btn.GetComponent<TowerBtn>();
        btn.onClick.AddListener(() =>
        {
            
        });
    }
}
