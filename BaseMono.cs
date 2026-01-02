using UnityEngine;

/// ==================================================================================================================
/// BaseMono를 상속하면, 다음 어트리뷰트를 통해 공통 기능을 자동 처리할 수 있습니다.
/// 
/// [AutoCleanup] (자동)
/// 목적: OnDestroy() 시 컬렉션과 참조 타입을 자동 정리합니다.
/// (무시하고 싶은 필드는 [DoNotCleanup] 를 사용해 주세요)
/// 
/// [AutoKillTween] (자동)
/// 목적: OnDisable() 시 Tween을 자동으로 종료합니다.
/// (무시하고 싶은 필드는 [DoNotAutoKillTween] 를 사용해 주세요)
/// ==================================================================================================================

public abstract class BaseMono : MonoBehaviour
{
	protected virtual bool UseAutoCleanup => true;
	protected virtual bool UseAutoKillTween => true;

	protected virtual void Awake() { }
	protected virtual void Start() { }
	protected virtual void OnEnable() { }
	protected virtual void OnDisable()
	{
#if DOTWEEN
		if (UseAutoKillTween)
			TweenAutoCleanupUtility.KillAllTweens(this);
#endif
	}
	protected virtual void OnDestroy()
	{
#if DOTWEEN
		if (UseAutoKillTween)
			TweenAutoCleanupUtility.KillAllTweens(this);
#endif
		if (UseAutoCleanup)
			AutoCleanupUtility.Cleanup(this);
	}
}