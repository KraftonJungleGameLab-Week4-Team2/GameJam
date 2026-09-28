// OpenFracture 원본 절단 알고리즘. 출처와 변경 사항은 LICENSE.txt에 기록한다.
namespace SurfaceSystem.OpenFracture
{
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum SlicedMeshSubmesh
{
    Default = 0,
    CutFace = 1
}
public class FragmentData
{
    public List<MeshVertex> Vertices;
    public List<MeshVertex> CutVertices;
    public List<int>[] Triangles;
    public List<EdgeConstraint> Constraints;
    public int[] IndexMap;
    public Bounds Bounds;
    public int triangleCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < this.Triangles.Length; i++)
            {
                count += this.Triangles[i].Count;
            }
            return count;
        }
    }
    public int vertexCount
    {
        get
        {
            return this.Vertices.Count + this.CutVertices.Count;
        }
    }
    public FragmentData(int vertexCount, int triangleCount)
    {
        this.Vertices = new List<MeshVertex>(vertexCount);
        this.CutVertices = new List<MeshVertex>(vertexCount / 10);

        // Store triangles for each submesh separately
        this.Triangles = new List<int>[] {
            new List<int>(triangleCount),
            new List<int>(triangleCount / 10)
        };

        this.Constraints = new List<EdgeConstraint>();
        this.IndexMap = new int[vertexCount];
    }
    public FragmentData(Mesh mesh)
    {
        var positions = mesh.vertices;
        var normals = mesh.normals;
        var uv = mesh.uv;

        this.Vertices = new List<MeshVertex>(mesh.vertexCount);
        this.CutVertices = new List<MeshVertex>(mesh.vertexCount / 10);
        this.Constraints = new List<EdgeConstraint>();
        this.IndexMap = new int[positions.Length];

        // Add mesh vertices
        for (int i = 0; i < positions.Length; i++)
        {
            this.Vertices.Add(new MeshVertex(positions[i], normals[i], uv[i]));
        }

        // Only meshes with one submesh are currently supported
        this.Triangles = new List<int>[2];
        this.Triangles[0] = new List<int>(mesh.GetTriangles(0));
        
        if (mesh.subMeshCount >= 2)
        {
            this.Triangles[1] = new List<int>(mesh.GetTriangles(1));
        }
        else
        {
            this.Triangles[1] = new List<int>(mesh.triangles.Length / 10);
        }

        this.CalculateBounds();
    }
    public void AddCutFaceVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
        var vertex = new MeshVertex(position, normal, uv);

        // Add the vertex to both the normal mesh vertex data and the cut face vertex data
        // The vertex on the cut face will have different normal/uv coordinates which are
        // populated with the correct values later in the triangulation process.
        this.Vertices.Add(vertex);
        this.CutVertices.Add(vertex);
    }
    public void AddMappedVertex(MeshVertex vertex, int sourceIndex)
    {
        this.Vertices.Add(vertex);
        this.IndexMap[sourceIndex] = this.Vertices.Count - 1;
    }
    public void AddTriangle(int v1, int v2, int v3, SlicedMeshSubmesh subMesh)
    {
        this.Triangles[(int)subMesh].Add(v1);
        this.Triangles[(int)subMesh].Add(v2);
        this.Triangles[(int)subMesh].Add(v3);
    }
    public void AddMappedTriangle(int v1, int v2, int v3, SlicedMeshSubmesh subMesh)
    {
        this.Triangles[(int)subMesh].Add(IndexMap[v1]);
        this.Triangles[(int)subMesh].Add(IndexMap[v2]);
        this.Triangles[(int)subMesh].Add(IndexMap[v3]);
    }
    public void WeldCutFaceVertices()
    {
        // Temporary array containing the unique (welded) vertices
        // Initialize capacity to current number of cut vertices to prevent
        // unnecessary reallocations
        List<MeshVertex> weldedVerts = new List<MeshVertex>(CutVertices.Count);

        // We also keep track of the index mapping between the skipped vertices
        // and the index of the welded vertex so we can update the edges
        int[] indexMap = new int[CutVertices.Count];

        // Number of welded vertices in the temp array
        int k = 0;

        // Loop through each vertex, identifying duplicates. Must compare directly
        // because floating point inconsistencies cause a hash table to be unreliable
        // for vertices that are very close together but not directly coincident
        for(int i = 0; i < CutVertices.Count; i++)
        {
            bool duplicate = false;
            for(int j = 0; j < weldedVerts.Count; j++)
            {
                if (CutVertices[i].position == weldedVerts[j].position)
                {
                    indexMap[i] = j;
                    duplicate = true;
                    break;
                }
            }

            if (!duplicate)
            {
                weldedVerts.Add(CutVertices[i]);
                indexMap[i] = k;
                k++;
            }
        }

        // Update the edges
        for(int i = 0; i < Constraints.Count; i++)
        {
            var edge = Constraints[i];
            edge.v1 = indexMap[edge.v1];
            edge.v2 = indexMap[edge.v2];
        }

        weldedVerts.TrimExcess();

        // Update the cut vertices
        this.CutVertices = new List<MeshVertex>(weldedVerts);
    }
    public int[] GetTriangles(int subMeshIndex)
    {
        return this.Triangles[subMeshIndex].ToArray();
    }
    public void CalculateBounds()
    {
        float vertexCount = (float)Vertices.Count;
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        // The cut face does not modify the extents of the object, so we only need to
        // loop through the original vertices to determine the bounds
        foreach(MeshVertex vertex in Vertices)
        {
            if (vertex.position.x < min.x) min.x = vertex.position.x;
            if (vertex.position.y < min.y) min.y = vertex.position.y;
            if (vertex.position.z < min.z) min.z = vertex.position.z;
            if (vertex.position.x > max.x) max.x = vertex.position.x;
            if (vertex.position.y > max.y) max.y = vertex.position.y;
            if (vertex.position.z > max.z) max.z = vertex.position.z;
        }

        this.Bounds = new Bounds((max + min) / 2f, max - min);
    }
    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh();
        
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
        };

        mesh.SetIndexBufferParams(triangleCount, IndexFormat.UInt32);
        mesh.SetVertexBufferParams(vertexCount, layout);
        mesh.SetVertexBufferData(Vertices, 0, 0, Vertices.Count);
        mesh.SetVertexBufferData(CutVertices, 0, Vertices.Count, CutVertices.Count);

        mesh.subMeshCount = Triangles.Length;
        int indexStart = 0;
        for(int i = 0; i < Triangles.Length; i++)
        {
            var subMeshIndexBuffer = Triangles[i];
            mesh.SetIndexBufferData(subMeshIndexBuffer, 0, indexStart, subMeshIndexBuffer.Count);
            mesh.SetSubMesh(i, new SubMeshDescriptor(indexStart, subMeshIndexBuffer.Count));
            indexStart += subMeshIndexBuffer.Count;
        }
        
        mesh.RecalculateBounds();
        
        return mesh;
    }
}
}

