using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifyTest : MonoBehaviour
{
    private int m_RotateSize = 45;
    // Start is called before the first frame update
    void Start()
    {
        //SkinnedMeshRenderer meshRenderer = GetComponent<SkinnedMeshRenderer>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        if (mesh == null) mesh = new Mesh();
        //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //mesh.vertices = Rabbit.vertices;
        //mesh.triangles = Rabbit.triangles;
        //mesh.RecalculateNormals();

        //Mesh mesh = meshRenderer.sharedMesh;

        SimplifyMesh simplifyMesh = new SimplifyMesh(mesh);
        Mesh yMesh = simplifyMesh.GetSimplifyMesh();
        meshFilter.mesh = yMesh;
        //meshRenderer.sharedMesh = yMesh;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up * Time.deltaTime * m_RotateSize);
    }
}
