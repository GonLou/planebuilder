using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TerrainDiamond t = new TerrainDiamond(0.7f, 9);
		Debug.Log(t.getMax());
		t.generate(t.getMax());
		t.printCoords();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
