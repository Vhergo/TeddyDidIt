#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public List<GameObject> plankPrefabs;
    public int numberOfPlanks = 10;
    public float spacing = 0.1f;
    private List<GameObject> instantiatedPlanks = new List<GameObject>();

    public void UpdatePlanks()
    {
        RemovePlanks();

        for (int i = 0; i < numberOfPlanks; i++) {
            GameObject prefab = plankPrefabs[Random.Range(0, plankPrefabs.Count)];
            var plank = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            float plankWidth = plank.transform.localScale.x;
            plank.transform.localPosition = new Vector3(i * (plankWidth + spacing), 0, 0);
            instantiatedPlanks.Add(plank);
        }
    }

    public void RemovePlanks()
    {
        foreach(GameObject plank in instantiatedPlanks) {
            DestroyImmediate(plank.gameObject);
        }
        instantiatedPlanks.Clear();
    }
}
#endif
