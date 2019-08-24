using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class ProceduralGeometry : MonoBehaviour 
{
    [SerializeField, Range(0, 180)] float alpha;
    [SerializeField, Range(0, 180)] float beta;
    [SerializeField, Range(0, 20)] float radius;
    [SerializeField] int xResolution;
    [SerializeField] int yResolution;
    [SerializeField] Material mat;
    [SerializeField] Transform projectionPlane;
    [SerializeField] Transform projectionCamera;

    Mesh m_Mesh;

    private void OnEnable()
    {
        UpdateGeometry();
        UpdateMaterial();
    }

    private void OnDisable()
    {
        DestroyImmediate(m_Mesh);
    }

    private void OnValidate()
    {
        xResolution = Mathf.Max(1, xResolution);
        yResolution = Mathf.Max(1, yResolution);
        UpdateGeometry();
        UpdateMaterial();
    }

    void Update()
    {
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        // TODO: optimization, do not manipulate uniform string names
        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_AlphaRad", alpha * Mathf.Deg2Rad);
        mat.SetFloat("_BetaRad", beta * Mathf.Deg2Rad);
        mat.SetVector("_PlanePoint", projectionPlane.position);
        mat.SetVector("_PlaneNormal", projectionPlane.up);
        mat.SetMatrix("_WorldToPlane", projectionPlane.worldToLocalMatrix);
        mat.SetVector("_EyePosition", projectionCamera.position);
    }

    Mesh GetOrCreateMesh()
    {
        if (m_Mesh != null)
            return m_Mesh;
        
        m_Mesh = new Mesh();
        m_Mesh.hideFlags = HideFlags.HideAndDontSave;
        var filter = GetComponent<MeshFilter>();
        filter.mesh = m_Mesh;
        return m_Mesh;
    }

    void UpdateGeometry()
    {
        var numVerts = (xResolution + 1) * (yResolution + 1);
        var dx = 1.0f / (float)xResolution;
        var dy = 1.0f / (float)yResolution;
        var numTris = xResolution  * yResolution * 2;
        var vertices = new Vector3[numVerts];
        var indices = new int[numTris * 3];

        for (var y = 0; y != yResolution + 1; ++y)
        {
            for (var x = 0; x != xResolution + 1; ++x)
            {
                vertices[y * (xResolution + 1) + x] = Quaternion.Euler(
                    (y * dy - 0.5f) * beta, 
                    (x * dx - 0.5f) * alpha, 0) * Vector3.forward * radius;
            }
        }

        var index = 0;
        var xResPlusOne = xResolution + 1;
        for (var x = 0; x != xResolution; ++x)
        {
            for (var y = 0; y != yResolution; ++y)
            {
                indices[index++] = y * xResPlusOne + x;
                indices[index++] = y * xResPlusOne + x + 1;
                indices[index++] = (y + 1) * xResPlusOne + x + 1;

                indices[index++] = (y + 1) * xResPlusOne + x + 1;
                indices[index++] = (y + 1) * xResPlusOne + x;
                indices[index++] = y * xResPlusOne + x;
            }
        }

        var mesh = GetOrCreateMesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
    }
}
