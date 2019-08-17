using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LookUpTable;

/// <summary>
/// A Chunk of Voxels
/// Each voxel has its position with an offset of 0.5
/// </summary>
public class Chunk : MonoBehaviour
{

    public int VOXEL_RESOLUTION;
    public float EXTEND;
    public float VOXEL_SIZE;

    // The array which stores all voxel values of the chunk
    private float[] _voxel;

    /// <summary>
    /// Should be called to initialize the chunk
    /// </summary>
    /// <param name="extend"></param>
    /// <param name="voxelRes"></param>
    /// <param name="voxelGrid"></param>
    public void Init(float extend, int voxelRes, float[] voxelGrid = null)
    {
        VOXEL_RESOLUTION = voxelRes;
        EXTEND = extend;
        VOXEL_SIZE = EXTEND / ((float) VOXEL_RESOLUTION);

        if(voxelGrid == null)
        {
            _voxel = new float[VOXEL_RESOLUTION * VOXEL_RESOLUTION * VOXEL_RESOLUTION];
        }
        else
        {
            _voxel = voxelGrid;
        }
    }

    // Start is called before the first frame update
    public void Rebuild()
    {
        // Build Mesh
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < VOXEL_RESOLUTION; x++)
        {
            for (int y = 0; y < VOXEL_RESOLUTION; y++)
            {
                for (int z = 0; z < VOXEL_RESOLUTION; z++)
                {
                    Voxel[] voxel = new Voxel[8];
                    Vector3 pos = new Vector3(x, y, z) - Vector3.one * (EXTEND / 2);
                    if(x < VOXEL_RESOLUTION - 1 && y < VOXEL_RESOLUTION - 1 && z < VOXEL_RESOLUTION - 1)
                    {
                        int vIndex0 = (x) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y) * VOXEL_RESOLUTION + (z + 1);
                        int vIndex1 = (x + 1) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y) * VOXEL_RESOLUTION + (z + 1);
                        int vIndex2 = (x + 1) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y) * VOXEL_RESOLUTION + (z);
                        int vIndex3 = (x) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y) * VOXEL_RESOLUTION + (z);
                        int vIndex4 = (x) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y + 1) * VOXEL_RESOLUTION + (z + 1);
                        int vIndex5 = (x + 1) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y + 1) * VOXEL_RESOLUTION + (z + 1);
                        int vIndex6 = (x + 1) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y + 1) * VOXEL_RESOLUTION + (z);
                        int vIndex7 = (x) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y + 1) * VOXEL_RESOLUTION + (z);

                        // Build Voxel Array of size 8 to feed the lookuptable
                        // The order is important and is defined here: http://paulbourke.net/geometry/polygonise/
                        
                        
                        voxel[0] = new Voxel() { position = new Vector3(pos.x, pos.y, pos.z + 1) * VOXEL_SIZE, density = _voxel[vIndex0] };
                        voxel[1] = new Voxel() { position = new Vector3(pos.x + 1, pos.y, pos.z + 1) * VOXEL_SIZE, density = _voxel[vIndex1] };
                        voxel[2] = new Voxel() { position = new Vector3(pos.x + 1, pos.y, pos.z) * VOXEL_SIZE, density = _voxel[vIndex2] };
                        voxel[3] = new Voxel() { position = new Vector3(pos.x, pos.y, pos.z) * VOXEL_SIZE, density = _voxel[vIndex3] };
                        voxel[4] = new Voxel() { position = new Vector3(pos.x, pos.y + 1, pos.z + 1) * VOXEL_SIZE, density = _voxel[vIndex4] };
                        voxel[5] = new Voxel() { position = new Vector3(pos.x + 1, pos.y + 1, pos.z + 1) * VOXEL_SIZE, density = _voxel[vIndex5] };
                        voxel[6] = new Voxel() { position = new Vector3(pos.x + 1, pos.y + 1, pos.z) * VOXEL_SIZE, density = _voxel[vIndex6] };
                        voxel[7] = new Voxel() { position = new Vector3(pos.x, pos.y + 1, pos.z) * VOXEL_SIZE, density = _voxel[vIndex7] };




                    }
                    else
                    {

                        Vector3 worldPos = pos + Vector3.one * 0.5f;
                        float d0 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(0,0,1) * VOXEL_SIZE));
                        float d1 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(1,0,1) * VOXEL_SIZE));
                        float d2 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(1,0,0) * VOXEL_SIZE));
                        float d3 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(0,0,0) * VOXEL_SIZE));
                        float d4 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(0,1,1) * VOXEL_SIZE));
                        float d5 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(1,1,1) * VOXEL_SIZE));
                        float d6 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(1,1,0) * VOXEL_SIZE));
                        float d7 = ChunkManager.Instance.GetVoxel(transform.TransformPoint(worldPos + new Vector3(0,1,0) * VOXEL_SIZE));

                        voxel[0] = new Voxel() { position = new Vector3(pos.x, pos.y, pos.z + 1) * VOXEL_SIZE, density = d0 };
                        voxel[1] = new Voxel() { position = new Vector3(pos.x + 1, pos.y, pos.z + 1) * VOXEL_SIZE, density = d1 };
                        voxel[2] = new Voxel() { position = new Vector3(pos.x + 1, pos.y, pos.z) * VOXEL_SIZE, density = d2 };
                        voxel[3] = new Voxel() { position = new Vector3(pos.x, pos.y, pos.z) * VOXEL_SIZE, density = d3 };
                        voxel[4] = new Voxel() { position = new Vector3(pos.x, pos.y + 1, pos.z + 1) * VOXEL_SIZE, density = d4 };
                        voxel[5] = new Voxel() { position = new Vector3(pos.x + 1, pos.y + 1, pos.z + 1) * VOXEL_SIZE, density = d5 };
                        voxel[6] = new Voxel() { position = new Vector3(pos.x + 1, pos.y + 1, pos.z) * VOXEL_SIZE, density = d6 };
                        voxel[7] = new Voxel() { position = new Vector3(pos.x, pos.y + 1, pos.z) * VOXEL_SIZE, density = d7 };
                    }


                    List<Triangle> tList = LookUpTable.ConstructTriangles(voxel);

                    foreach (Triangle t in tList)
                    {
                        triangles.Add(vertices.Count);
                        vertices.Add(t.p1);
                        triangles.Add(vertices.Count);
                        vertices.Add(t.p2);
                        triangles.Add(vertices.Count);
                        vertices.Add(t.p3);
                    }

                }
            }
        }



        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;


    }

    public void AddVoxel(Vector3 worldPoint, float val)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPoint) - Vector3.one * 0.5f;
        localPos += Vector3.one * (EXTEND / 2);
        int x = Mathf.FloorToInt(localPos.x);
        int y = Mathf.FloorToInt(localPos.y);
        int z = Mathf.FloorToInt(localPos.z);
        int vIndex = (x) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y) * VOXEL_RESOLUTION + (z);
        try
        {
            _voxel[vIndex] = val;
        }
        catch (IndexOutOfRangeException e)
        {
            Vector3 chunkPos = transform.position;
            Debug.LogError(x + " " + y + " " + z);
        }
        
    }

    public float GetVoxel(Vector3 worldPoint)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPoint);
        localPos += Vector3.one * (EXTEND / 2);
        int x = Mathf.FloorToInt(localPos.x);
        int y = Mathf.FloorToInt(localPos.y);
        int z = Mathf.FloorToInt(localPos.z);
        int vIndex = (x) * VOXEL_RESOLUTION * VOXEL_RESOLUTION + (y) * VOXEL_RESOLUTION + (z);
        try
        {
            float ret = _voxel[vIndex];
            return ret;
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.LogError(x + " " + y + " " + z);
            return 0;
        }
        
    }

    /// <summary>
    /// Returns the one-dimension index of the _voxel array based on a local position
    /// </summary>
    /// <returns></returns>
    public int LocalPositionToIndex(Vector3 localPos)
    {


        return 0;
    }

    /// <summary>
    /// Returns the local position of a voxel based on a given index
    /// </summary>
    /// <returns></returns>
    public Vector3 IndexToLocalPosition()
    {

        return Vector3.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    public void BuildMesh()
    {

    }

}
