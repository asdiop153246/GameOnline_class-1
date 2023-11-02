using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
public class RoomCodeScript : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private GameObject uiGameObject;
    private void Start()
    {
        if (codeText == null)
        {
            Debug.LogError("Code Text is not assigned.");
            return;
        }

        if (uiGameObject == null)
        {
            Debug.LogError("UI GameObject is not assigned.");
            return;
        }

        if (!IsLocalPlayer)
        {
            uiGameObject.SetActive(false);
            return;
        }

        if (RelayManagerScript.Instance != null)
        {
            string joinCode = RelayManagerScript.Instance.JoinCode;
            codeText.text = joinCode;
            Debug.Log("Join code in RoomCodeScript: " + joinCode);
        }
        else
        {
            Debug.LogError("RelayManagerScript instance is null.");
        }

        // Initially set the UI to be inactive
        uiGameObject.SetActive(false);
    }

    public void copyCodeTextToClip()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = codeText.text;
        textEditor.SelectAll();
        textEditor.Copy();
        Debug.Log("Text Copied");
    }

    private void Update()
    {
        // Only allow the local player to toggle the UI
        if (IsLocalPlayer && Input.GetKeyDown(KeyCode.O))
        {
            // Toggle the active state of the UI GameObject
            uiGameObject.SetActive(!uiGameObject.activeSelf);
        }
    }
}
