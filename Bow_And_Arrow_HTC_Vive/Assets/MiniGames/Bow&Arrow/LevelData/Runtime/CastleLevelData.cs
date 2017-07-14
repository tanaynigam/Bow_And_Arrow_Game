using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class CastleLevelData
{
	[SerializeField]
	int level;
	public int Level { get {return level; } set { level = value;} }
	
	[SerializeField]
	int targethits;
	public int Targethits { get {return targethits; } set { targethits = value;} }
	
	[SerializeField]
	int balloonhits;
	public int Balloonhits { get {return balloonhits; } set { balloonhits = value;} }
	
	[SerializeField]
	bool enableironballs;
	public bool Enableironballs { get {return enableironballs; } set { enableironballs = value;} }
	
	[SerializeField]
	float targetsmovespeed;
	public float Targetsmovespeed { get {return targetsmovespeed; } set { targetsmovespeed = value;} }
	
	[SerializeField]
	float ironballmovespeed;
	public float Ironballmovespeed { get {return ironballmovespeed; } set { ironballmovespeed = value;} }
	
}