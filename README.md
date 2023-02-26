# Coroutine-Optimizer

최적화된 코루틴 라이브러리

===================================</br>
@@ 2. 코루틴 최적화... @@</br>
[ 참고 : https://ejonghyuck.github.io/blog/2016-12-12/unity-coroutine-optimization/ ]</br>
===================================<br>
1] Garbage<br>
===================================<br> - 코루틴은 가비지를 생성하는 요소를 가지고 있음.<br>
   - StartCoroutine<br>
   - YieldInstruction<br>

===================================<br>
2] StartCoroutine의 가비지 생성.<br>
=================================== <br>
- StartCoroutine을 호출. 
- 엔진 내부에서 인스턴스가 생성. 
- 해당 코루틴 관리 목적. 
- 이 타이밍에 가비지가 발생!!

유니티 엔진 내부의 코드<br>
함수 자체를 최적화하는 것은 불가능.

해결방법<br>
- 직접 코루틴 기능을 제작.<br>
- 호출 최소화.

===================================<br>
3] YieldInstruction<br>
===================================<br>

코루틴 내부에서 yield 구문에 사용되는 구문.<br>
    크게 3가지 구문사용.<br>
- WaitForEndOfFrame
- WaitForFixedUpdate
- WaitForSeconds

예)<br>
```cs
    yield return new WaitForEndOfFrame();
```

YieldInstruction을 new를 통해 인스턴스를 생성해서 사용.<br>
- 이때 가비지가 발생.<br>
- YieldInstruction의 인스턴스를 캐싱하면 가비지가 크게 줄것임.<br>
```cs
publics static class YieldInstructionCache
{
    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
}

// 사용예...
yield return YieldInstructionCache.WaitForEndOfFrame;
yield return YieldInstructionCache.WaitForFixedUpdate;
```
===================================<br>
4] WaitForSeconds<br>
===================================<br> 
- WaitForSeconds의 경우 시간 값에 따라 인스턴스가 생성됨. 
- 인게임 중 대기 시간을 얼만큼 설정할지는 예측하기 힘듬.<br>

다음과 같은 방법으로 보완.
```cs
//	IEqualityComparer
//	-	https://themangs.tistory.com/entry/unity-c%EC%97%90%EC%84%9C-enum-struct%EB%A5%BC-key%EB%A1%9C-%EC%82%AC%EC%9A%A9%ED%95%98%EB%8A%94-dictionary-%EB%B0%95%EC%8B%B1-%ED%94%BC%ED%95%98%EA%B8%B0?category=492504
class FloatComparer : IEqualityComparer<float>
{
    bool IEqualityComparer<float>.Equals (float x, float y) { return x == y; }
    int IEqualityComparer<float>.GetHashCode (float obj)	{ return obj.GetHashCode(); }
}

private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());

public static WaitForSeconds WaitForSeconds(float seconds)
{
    WaitForSeconds wfs;
    if (!_timeInterval.TryGetValue(seconds, out wfs))
    _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
    return wfs;
}

// 사용예...
yield return YieldInstructionCache.WaitForSeconds(0.1f);
yield return YieldInstructionCache.WaitForSeconds(seconds);
```
-	second( 초단위 시간 )값마다 WaitForSeconds 인스턴스를 생성하여 Dictionary에 캐싱하는 방법.
-	가비지가 아예 발생하지 않는것은 아니지만 일반적인 방법보다 ( yield return new WaitForSeconds(seconds); )<br>
    크게 가비지의 발생률을 줄일수 있음.

===================================<br>
사용법

```cs
using CoroutineOptimizer; // 라이브러리 임포트
...

IEnumerator Func()
{
yield return CoroutineOptimizer.WaitForSeconds((float)waiting time); // 멈출 시간
yield return CoroutineOptimizer.WaitForEndOfFrame; // 프레임이 끝날때 까지 대기
yield return CoroutineOptimizer.WaitForFixedUpdate; // FixedUpdate() 끝날때 까지 대기
}

```

===================================<br>
