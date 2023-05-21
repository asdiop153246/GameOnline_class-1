using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject Menu;
    public void Play()
    {
        Menu.SetActive(false);
        LoginPanel.SetActive(true);
    }
}
