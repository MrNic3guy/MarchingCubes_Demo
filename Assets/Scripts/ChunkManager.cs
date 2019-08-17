using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    // Extend of the chunks in meter
    public const float EXTEND = 8;

    // Resolution of the chunk
    public const int VOXEL_RESOLUTION = 8;

    // The SIZE of a voxel
    public const float VOXEL_SIZE = EXTEND / VOXEL_RESOLUTION;

    [Range(0.01f, 1f)]
    public float PerlinScale = 0.1f;

    [Range(0f, 10f)]
    public float PerlinOffset = 0.1f;

    public GameObject ChunkPrefab;

    private Dictionary<Vector3, Chunk> _chunks = new Dictionary<Vector3, Chunk>();

    public static ChunkManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Queue<Chunk> chunksToBuild = new Queue<Chunk>();

        for (int x = 0; x < 150; x++)
        {
            for (int z = 0; z < 150; z++)
            {


                float perlin = Mathf.PerlinNoise(x * PerlinScale, z * PerlinScale);
                float curHeight = (int)(perlin * 15);
                curHeight = Mathf.Max(2, curHeight);
                for (int y = 0; y < curHeight; y++)
                {
                    Chunk curChunk = AddVoxel(new Vector3(x + 0.5f, y+0.5f, z + 0.5f), 1);

                    if (!chunksToBuild.Contains(curChunk))
                    {
                        chunksToBuild.Enqueue(curChunk);
                    }
                }


            }
        }

        while(chunksToBuild.Count > 0)
        {
            yield return new WaitForEndOfFrame();       
            Chunk chunk = chunksToBuild.Dequeue();
            chunk.Rebuild();
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public float GetVoxel(Vector3 worldPoint)
    {
        Chunk chunk;
        Vector3 chunkPos = new Vector3(Mathf.Round(worldPoint.x / EXTEND), Mathf.Round(worldPoint.y / EXTEND), Mathf.Round(worldPoint.z / EXTEND)) * EXTEND;
        if (!_chunks.TryGetValue(chunkPos, out chunk))
        {
            return 0;
        }

        float ret = chunk.GetVoxel(worldPoint);
        return ret;
    }

    /// <summary>
    /// Instantiates and initializes a new chunk
    /// </summary>
    /// <param name="chunkWorldPos"></param>
    /// <returns></returns>
    private Chunk InstantiateChunk(Vector3 chunkWorldPos){

        GameObject chunkObj = Instantiate(ChunkPrefab, transform);
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.Init(EXTEND, VOXEL_RESOLUTION);
        chunkObj.transform.localPosition = chunkWorldPos;
        _chunks.Add(chunkWorldPos, chunk);
        return chunk;
    }

    /// <summary>
    /// Adds a Voxel value to a chunk
    /// Instantiates a new chunk if target chunk does not exist
    /// </summary>
    /// <param name="worldPoint"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public Chunk AddVoxel(Vector3 worldPoint, float val)
    {
        Chunk chunk;
        Vector3 chunkPos = new Vector3(Mathf.Round(worldPoint.x/EXTEND), Mathf.Round(worldPoint.y / EXTEND), Mathf.Round(worldPoint.z / EXTEND)) * EXTEND;
        if(!_chunks.TryGetValue(chunkPos, out chunk))
        {
            chunk = InstantiateChunk(chunkPos);
        }

        chunk.AddVoxel(worldPoint, val);
        
        return chunk;
    }


    /// <summary>
    /// Perlin 3D
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static float Perlin3D(float x, float y, float z)
    {
        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);

        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        float ABC = AB + BC + AC + BA + CB + CA;

        return ABC / 6.0f;
    }
}
