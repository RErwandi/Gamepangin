using System;
using Gamepangin;
using UnityEngine;

public class TestMenu : Menu<TestMenu>
{
    protected override void OnOpen()
    {
        Debug.Log("Opened");
    }

    protected override void OnClose()
    {
        Debug.Log("Closed");
    }
}
