using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceTimeLine : MonoBehaviour
{
    public TextMeshProUGUI horloge;

    void Start()
    {
        horloge = GameObject.Find("Minuteur").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        horloge.text = GameManager.Instance.DefaultTime.Value.ToString("0.00");
        
    }
}
