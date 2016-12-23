using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public enum Shape{Vertical,Horizontal,Cross,Intersect,Square,Diamond};
public enum Target{Single,AOE};

[System.Serializable]
public class SkillInfo 
{
	public string name;
	public string explane;
	public float basic_damage;
	public int basic_range;
	public int basic_cd;
	public Shape shape;
	public Target target;

	//public List<Cost> cost=new List<Cost>();


	[System.Serializable]
	public class Cost
	{
		public Element Element;
		public int num;
	}

}



