using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuScreen : MonoBehaviour {

    public void HideMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        this.gameObject.SetActive(true);
    }
}
