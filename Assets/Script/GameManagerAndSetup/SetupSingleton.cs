using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetupSingleton : MonoBehaviour
{
    #region Singleton
    public static SetupSingleton instance;
    #endregion
    public NavMeshSurface surface;
    private void Awake()
    {
        instance = this;
        surface = GetComponentInChildren<NavMeshSurface>();
    }
}
