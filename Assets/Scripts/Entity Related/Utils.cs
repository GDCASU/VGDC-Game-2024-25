// Builds the damage multiplier dict from values set in inspector
using System.Collections.Generic;

public static class Utils
{
	/// <summary>
	/// Returns a dictionary of damage multipliers built from an array of DamageMultipliers
	/// </summary>
	/// <param name="multipliers">An array of damage multipliers</param>
	/// <returns></returns>
	public static Dictionary<Elements, float> GetDamageMultipliers(DamageMultiplier[] multipliers)
	{
		Dictionary<Elements, float> damageMultiplierDict = new Dictionary<Elements, float>();
		// Create damage multiplier dict based on values set in the Unity inspector
		foreach(DamageMultiplier dm in multipliers)
		{
			damageMultiplierDict.Add(dm.element, dm.multiplier);
		}
		return damageMultiplierDict;
	}
}