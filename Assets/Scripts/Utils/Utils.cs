using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-19, -1), 1, -140);
    }

    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    
}
