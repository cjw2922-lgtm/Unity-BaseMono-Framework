using System;
using System.Reflection;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class AutoKillTweenAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class DoNotAutoKillTweenAttribute : PropertyAttribute { }

#if DOTWEEN
public static class TweenAutoCleanupUtility
{
	public static void KillAllTweens(object target)
	{
		if (target == null) return;

		var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

		foreach (var field in fields)
		{
			// DoNotAutoKillTween 어트리뷰트가 붙은 필드는 무시
			if (field.IsDefined(typeof(DoNotAutoKillTweenAttribute), inherit: true))
				continue;

			// Tween 필드만 처리
			if (field.FieldType == typeof(Tween))
			{
				Tween tween = field.GetValue(target) as Tween;
				if (tween != null && tween.IsActive())
				{
					tween.Kill();
				}

				field.SetValue(target, null); // null 처리까지
			}
		}
	}
}
#endif