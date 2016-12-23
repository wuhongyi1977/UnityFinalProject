using UnityEngine;
using System.Collections;

[System.Serializable]
public enum Element{Thunder,Wind,Water,Fire,Earth,Zero};
public enum Moving_way{Walk,Fly};

[System.Serializable]
public class RoleInfo
{
	public string name;
	public Element element;
	public Moving_way moving_way;
	public int basic_life;
	public int basic_damage;
	public int basic_move_range;
	public int basic_attack_range;

	static public Element findAgainstElement(Element e)
	{
		if (e == Element.Earth) {
			return Element.Thunder;
		} else if (e == Element.Fire) {
			return Element.Water;
		} else if (e == Element.Thunder) {
			return Element.Wind;
		} else if (e == Element.Water) {
			return Element.Earth;
		} else if (e == Element.Wind) {
			return Element.Fire;
		} else {
			return Element.Zero;
		}
	}
}
