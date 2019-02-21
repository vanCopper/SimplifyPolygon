using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifyTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SkinnedMeshRenderer meshRenderer = GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = meshRenderer.sharedMesh;

        SimplifyMesh simplifyMesh = new SimplifyMesh(mesh);
        simplifyMesh.GetSimplifyMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
