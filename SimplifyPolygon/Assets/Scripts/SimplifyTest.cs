using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifyTest : MonoBehaviour
{
    private int m_RotateSize = 45;
    public SimplifyMesh simplifyMesh;
    private float m_VerScale = 1;
    private MeshFilter m_Meshfilter;
    private int m_OriginVerNum;
    private int m_Dir = -1;
    private float m_Speed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        //SkinnedMeshRenderer meshRenderer = GetComponent<SkinnedMeshRenderer>();
        m_Meshfilter = GetComponent<MeshFilter>();
        Mesh mesh = m_Meshfilter.mesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = Rabbit.vertices;
            mesh.triangles = Rabbit.triangles;
            mesh.RecalculateNormals();
        }

        m_OriginVerNum = Rabbit.vertices.Length;
        //Mesh mesh = meshRenderer.sharedMesh;

        simplifyMesh = new SimplifyMesh(mesh);
        Mesh yMesh = simplifyMesh.GetSimplifyMesh((int)(mesh.vertices.Length * m_VerScale));
        if(yMesh != null) m_Meshfilter.mesh = yMesh;
        //meshRenderer.sharedMesh = yMesh;
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.Rotate(Vector3.up * Time.deltaTime * m_RotateSize);

        if (m_VerScale > 1)
        {
            m_Dir = -1;
        }
        if (m_VerScale < 0)
        {
            m_Dir = 1;
        }

        simplifyMesh.InitMesh();
        m_VerScale += Time.deltaTime * m_Speed * m_Dir;
        Mesh yMesh = simplifyMesh.GetSimplifyMesh((int)(m_OriginVerNum * m_VerScale));
        if(yMesh != null)
        {
            m_Meshfilter.mesh = yMesh;
        }
    }
}
