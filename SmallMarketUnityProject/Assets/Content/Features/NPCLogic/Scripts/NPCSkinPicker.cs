using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class SkinData
{
    public Mesh bodyMesh;
    public Mesh headMesh;
}

public class NPCSkinPicker : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer head;
    [SerializeField] private SkinnedMeshRenderer body;
    [SerializeField] private SkinData[] allSkins;

    private void Start()
    {
        SkinData selectedSkin = allSkins[Random.Range(0, allSkins.Length)];
        body.sharedMesh = selectedSkin.bodyMesh;
        head.sharedMesh = selectedSkin.headMesh;
    }
}
