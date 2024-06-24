using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGhosting : MonoBehaviour
{
    public Material material;
    private List<Material> originalMaterial = new List<Material>();
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            meshRenderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
            originalMaterial.Add(meshRenderers[meshRenderers.Count].sharedMaterial);

            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                meshRenderers.Add(transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>());
                originalMaterial.Add(meshRenderers[meshRenderers.Count].sharedMaterial);
            }
        }
    }

    public void PlacingEnabled(bool enabled)
    {
        //no setbool in shader graph go with set int of setfloat.
        for(int i = 0;i < meshRenderers.Count;i++)
        {
            if (meshRenderers[i].sharedMaterial != material)
            {
                meshRenderers[i].sharedMaterial = material;
            }
        }
        
        material.SetInt("_EnabledBooleon", enabled ? 1 : 0);
    }

    public bool ReturnItemPlacementStatus()
    {
        int t = material.GetInt("_EnabledBooleon");
        bool status;
        switch (t)
        {
            default:
                status = false;
                return status;
            case 1:
                status = true;
                return status;
        }    
    }

    public void OriginalMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i].sharedMaterial != originalMaterial[i])
            {
                meshRenderers[i].sharedMaterial = originalMaterial[i];
            }
        }
    }
}
