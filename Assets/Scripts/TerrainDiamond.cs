using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainDiamond {

	float roughness;
	int detail;
	float size;
	float max;
	List<Vector3> coords = new List<Vector3>();

	public TerrainDiamond() {
	}

	public TerrainDiamond(float roughness, int detail) {
		this.roughness = roughness;
		this.detail = detail;
		this.size = Mathf.Pow(detail, 2)-1;
		this.max = this.size - 1;

		generateFirstPoints();
	}

	~TerrainDiamond() {
	}

	public float getMax() {
		return this.max;
	}

	public void printCoords() {
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		int i;
		foreach (Vector3 aPart in this.coords)
		{
			Debug.Log(aPart);
			i++;
		}
		foreach (Vector3 aPart in this.coords)
		{
			Debug.Log(aPart);
			i++;
		}
		m.vertices = new Vector3[i] {
			this.coords();
		};
		m.RecalculateNormals();


//		Mesh CreateMesh(float width, float height)
//		{
//			Mesh m = new Mesh();
//			m.name = "ScriptedMesh";
//			m.vertices = new Vector3[] {
//				new Vector3(-width, -height, 0.01f),
//				new Vector3(width, -height, 0.01f),
//				new Vector3(width, height, 0.01f),
//				new Vector3(-width, height, 0.01f)
//			};
//			m.uv = new Vector2[] {
//				new Vector2 (0, 0),
//				new Vector2 (0, 1),
//				new Vector2(1, 1),
//				new Vector2 (1, 0)
//			};
//			m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
//			m.RecalculateNormals();
//			
//			return m;
//		}

	}

	public void generateFirstPoints() {
		this.coords.Add( new Vector3(0, 0, 0));
		this.coords.Add( new Vector3(0, this.max, 0));
		this.coords.Add( new Vector3(this.max, this.max, 0));
		this.coords.Add( new Vector3(this.max, 0, 0));
	}

	public void generate(float size) {
		float scale = this.roughness * size;
		float half = size / 2;

		if (half < 1) return;

		for (float y = half; y < this.max; y += size) {
			for (float x = half; x < this.max; x += size) {
				Square(x, y, half, Random.value * scale * 2 - scale);
			}
		}

		for (float y = 0; y <= this.max; y += half) {
			for (float x = (y + half) % size; x < this.max; x += size) {
				Diamond(x, y, half, Random.value * scale * 2 - scale);
			}
		}
		Debug.Log("here "+ size);

		generate(size / 2);
	}

	private void Square(float x, float y, float size, float offset) {
		Vector2[] vectors = new Vector2[4];

		vectors[0] = new Vector2(x - size, y - size);
		vectors[1] = new Vector2(x + size, y - size);
		vectors[2] = new Vector2(x + size, y + size);
		vectors[3] = new Vector2(x - size, y + size);

		offset += Avg(vectors);
		this.coords.Add( new Vector3(x, y, offset));
	}

	private void Diamond(float x, float y, float size, float offset) {
		Vector2[] vectors = new Vector2[4];
		
		vectors[0] = new Vector2(x, y - size);
		vectors[1] = new Vector2(x + size, y);
		vectors[2] = new Vector2(x, y + size);
		vectors[3] = new Vector2(x - size, y);
		
		offset += Avg(vectors);
		this.coords.Add( new Vector3(x, y, offset));
	}

	private float Avg(Vector2[] vectors) {
		if (vectors.Length == 0) return 0.0f;

		Vector2 average = Vector2.zero;
		
		for (int i = 0; i < vectors.Length-1; i++) {
			average += vectors[i];
		}

		return ((average.x/vectors.Length + average.y/vectors.Length) / 2);
	}


}