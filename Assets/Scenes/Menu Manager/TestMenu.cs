using System;
using Gamepangin;
using UnityEngine;

public class TestMenu : Menu<TestMenu>
{
    private void OnEnable()
    {
        onOpen.AddListener(OnOpen);
        onClose.AddListener(OnClose);
    }

    private void OnDisable()
    {
        onOpen.RemoveListener(OnOpen);
        onClose.RemoveListener(OnClose);
    }

    private void OnOpen()
    {
        Debug.Log("Opened");
    }

    private void OnClose()
    {
        Debug.Log("Closed");
    }
}
