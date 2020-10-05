using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : MonoBehaviour
{
    /// <summary>
    /// Button callback
    /// </summary>
    public void OnBackToTitleClicked()
    {
        Game.Instance.GoToTitleScene();
    }
}
