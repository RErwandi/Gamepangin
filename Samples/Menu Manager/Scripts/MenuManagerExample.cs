using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerExample : MonoBehaviour
{
    public void OpenExampleMenu()
    {
        TestMenu.Open();
    }

    public void OpenAnotherMenu()
    {
        AnotherMenu.Open();
    }
}
