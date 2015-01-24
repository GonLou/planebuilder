/// <summary>
/// Procedural terrain.
/// </summary>

using UnityEngine;
using System.Collections;

public class ProceduralTerrain : MonoBehaviour {
	
	private float widthLength = 30.0f;
	private int meshSegmentCount = 100;	
	public float noiseScale = 0.1f;
	public float noiseLayersCount = 4.3f;

	
	private const int noiseWidth = 256;
	private const int noiseHeight = 256;
	
	public float[,] noiseValues;


	protected virtual void Start()
	{
		MeshBuilder meshBuilder = new MeshBuilder();
		
		float segmentSize = widthLength / meshSegmentCount;

		generateNoise();
		
		for (int i = 0; i <= meshSegmentCount; i++)
		{
			float z = segmentSize * i;
			float v = (1.0f / meshSegmentCount) * i;
			
			for (int j = 0; j <= meshSegmentCount; j++)
			{
				float x = segmentSize * j;
				float u = (1.0f / meshSegmentCount) * j;

				Vector3 offset = new Vector3(x, CreateY(x, z), z);
				
				Vector2 uv = new Vector2(u, v);
				bool buildTriangles = i > 0 && j > 0;
				
				BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, meshSegmentCount + 1);
			}
		}

		// Creates the mesh
		Mesh mesh = meshBuilder.CreateMesh();
		// Recalculates the normals of the mesh from the triangles and vertices
		mesh.RecalculateNormals();
		
		// Search for a MeshFilter component attached to this GameObject
		MeshFilter filter = GetComponent<MeshFilter>();
		// Render the Mesh created
		if (filter != null)	filter.sharedMesh = mesh;

	}

	/// <summary>
	/// Calculate the value of Y (heigth).
	/// </summary>
	/// <returns>The y value.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public float CreateY(float x, float z)
	{
		float theta = Random.Range(0,2*Mathf.PI);
		float distance = widthLength/2;

		Debug.Log(x+" - "+Mathf.Cos(x)+"\n");
		Debug.Log(z+" - "+Mathf.Sin(z)+"\n");
		
		if (Mathf.Cos(x) != 0 && Mathf.Sin(z) != 0)
	    {

			if (noiseLayersCount <= 1) // simple noise
			{
				float perlinX = x * noiseScale;
				float perlinZ = z * noiseScale;
				return MyPerlinNoise(perlinX, perlinZ) * widthLength;
				//return Mathf.PerlinNoise(perlinX, perlinZ) * widthLength;
			}
			else // noise layers
			{
				float mul = 1.0f;
				float y = 0.0f;
				float totalPossibleSum = 0.0f;
				
				for (int i = 0; i < noiseLayersCount; i++)
				{
					float perlinX = x * noiseScale / mul;
					float perlinZ = z * noiseScale / mul;
					float noise = MyPerlinNoise(perlinX, perlinZ);
					//float noise = Mathf.PerlinNoise(perlinX, perlinZ);
					y += noise * mul;
					
					totalPossibleSum += mul;
					mul *= 0.5f;
				}
				
				return (y / totalPossibleSum) * widthLength/2;
			}
		 }

		return 0.0f;
	}

	/// <summary>
	/// Calculates the Perlin Noise
	/// </summary>
	/// <returns>The perlin noise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public float MyPerlinNoise(float x, float y)
	{
		// Determine grid cell coordinates
		int x0 = (x > 0.0 ? (int)x : (int)x - 1);
		int y0 = (y > 0.0 ? (int)y : (int)y - 1);
		
		// Determine interpolation weights
		float sx = x - x0;
		float sy = y - y0;
		
		// wrap around
		int x1 = (int)(x + noiseWidth % noiseWidth);
		int y1 = (int)(y + noiseHeight % noiseHeight);
		
		// neighbour values
		int x2 = (x1 + noiseWidth - 1) % noiseWidth;
		int y2 = (y1 + noiseHeight - 1) % noiseHeight;
		
		// smooth the noise with bilinear interpolation
		float value = 0.0f;
		value += sx       * sy       * noiseValues[x1, y1];
		value += sx       * (1 - sy) * noiseValues[x1, y2];
		value += (1 - sx) * sy       * noiseValues[x2, y1];
		value += (1 - sx) * (1 - sy) * noiseValues[x2, y2];
		
		return value;
	}

	/// <summary>
	/// Generate a noise grid
	/// </summary>
	public void generateNoise()
	{
		noiseValues = new float[noiseWidth,noiseHeight];
		
		for (int x = 0; x < noiseWidth; x++)
			for (int y = 0; y < noiseHeight; y++) noiseValues[x, y] = Random.Range(-1.0f, 1.0f);
	}

	/// <summary>
	/// </summary>
	/// <param name="meshBuilder">Mesh builder.</param>
	/// <param name="position">Position.</param>
	/// <param name="uv">Uv.</param>
	/// <param name="buildTriangles">If set to <c>true</c> build triangles.</param>
	/// <param name="vertsPerRow">Verts per row.</param>
	private void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);
		
		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;
			
			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;
			
			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}
}

