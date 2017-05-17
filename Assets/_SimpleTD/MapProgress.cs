#define execInEdit

using UnityEngine;
using System.Collections;

#if execInEdit
// load all childs
[ExecuteInEditMode]
#endif
public class MapProgress : MonoBehaviour {

    WorldMapChoice[] levels;


	// Use this for initialization
	void Start () {
        levels = GetComponentsInChildren<WorldMapChoice>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
