using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enable this if used for testing purposes
/// </summary>
public class TestController : MonoBehaviour
{
    public static bool testEnvironment = false;

    void Awake()
    {
        testEnvironment = true;
    }
}
