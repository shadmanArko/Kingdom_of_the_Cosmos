using System;
using UnityEngine;

public class TriangleModifier : MonoBehaviour
{
    [SerializeField] private int triangleBase;
    [SerializeField] private int triangleHeight;
    
    // [SerializeField] private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Transform p0; // Apex vertex
    [SerializeField] private Transform p1; // Base left
    [SerializeField] private Transform p2; // Base right

    [SerializeField] private MeshFilter triangleMeshFilter;
    
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        mesh = new Mesh();
        triangleMeshFilter.mesh = mesh;

        // Define vertices and triangle indices
        vertices = new Vector3[3]; // 3D for compatibility but flat on the Z axis
        triangles = new int[3] { 0, 1, 2 }; // Clockwise order for proper face rendering

        UpdateMesh();
    }

    void Update()
    {
        ConstructTriangle(triangleBase, triangleHeight);
        
        vertices[0] = new Vector3(p0.position.x, p0.position.y, 0); // Z = 0 to keep it flat
        vertices[1] = new Vector3(p1.position.x, p1.position.y, 0);
        vertices[2] = new Vector3(p2.position.x, p2.position.y, 0);

        UpdateMesh();
    }

    public void ConstructTriangle(float baseLength, float height)
    {
        if (p0 == null || p1 == null || p2 == null)
        {
            Debug.LogError("Please assign all vertex GameObjects!");
            return;
        }
    
        // Get P0's transform information
        Vector3 p0Position = p0.transform.position;
        Quaternion p0Rotation = p0.rotation;
    
        // Calculate base center position using P0's forward direction
        Vector3 baseCenter = p0Position + p0Rotation * Vector3.down * height;
    
        // Position base vertices using P0's right direction for the base
        Vector3 rightDirection = p0Rotation * Vector3.right;
        p1.transform.position = baseCenter - rightDirection * (baseLength / 2f);
        p2.transform.position = baseCenter + rightDirection * (baseLength / 2f);
    }
    
    private void UpdateMesh()
    {
        mesh.Clear(); // Clear previous data
        mesh.vertices = vertices; // Set updated vertices
        mesh.triangles = triangles; // Apply triangle indices

        // Optionally calculate normals for lighting if needed
        mesh.RecalculateNormals();
    }
    
}
