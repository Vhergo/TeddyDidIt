using System.Collections.Generic;
using UnityEngine;

public class LegoColors : MonoBehaviour
{
    [SerializeField] private Material whiteLego;
    [SerializeField] private List<Material> legoMaterials = new List<Material>();

    [SerializeField] private List<MeshRenderer> legoArchive = new List<MeshRenderer>();

    public void SetRandomLegoColors()
    {
        if (legoArchive.Count == 0) 
            legoArchive = GetAllLegoRenderers();

        foreach (MeshRenderer lego in legoArchive) {
            Material randomMaterial = legoMaterials[Random.Range(0, legoMaterials.Count)];
            lego.material = randomMaterial;
        }
    }

    private List<MeshRenderer> GetAllLegoRenderers()
    {
        List<MeshRenderer> allLegoRenderers = new List<MeshRenderer>();
        Lego[] allLegos = FindObjectsOfType<Lego>();

        foreach (Lego lego in allLegos) {
            MeshRenderer renderer = lego.GetComponent<MeshRenderer>();
            if (renderer != null)
                allLegoRenderers.Add(renderer);
        }

        return allLegoRenderers;
    }

    public void RemoveAllColors()
    {
        if (legoArchive.Count == 0)
            legoArchive = GetAllLegoRenderers();

        foreach (MeshRenderer lego in legoArchive) {
            lego.material = whiteLego;
        }
    }
}
