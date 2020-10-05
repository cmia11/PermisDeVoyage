using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceTimeLine : MonoBehaviour
{
    public TextMeshProUGUI horloge;

    // Update is called once per frame
    void Update()
    {
        horloge.text = Level.Instance.DefaultTime.Value.ToString("0.00");
    }
}
