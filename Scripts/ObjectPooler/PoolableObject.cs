using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PoolableObject : ScriptableObject
{
    public string poolableName = "name";
    public GameObject poolablePrefab;
    public int pooledAmount;

    private void OnValidate()
    {
        //Ensure the pooled amount never goes below 1
        pooledAmount = Mathf.Clamp(pooledAmount, 1, int.MaxValue);
    }
}
