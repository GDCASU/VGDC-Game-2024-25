using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
	public static bool IsPlaying(this Animator anim)
	{
		return anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f;
	}

	public static bool IsPlaying(this Animator anim, string s)
	{
		return anim.GetCurrentAnimatorStateInfo(0).IsName(s) && anim.IsPlaying();
	}

	public static IEnumerator PlayBlocking(this Animator self, string anim)
	{
		self.Play(anim, -1, 0f);

		yield return new WaitForEndOfFrame();

		while(self.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
			yield return null;
	}
}