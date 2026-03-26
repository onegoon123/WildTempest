// 풀링을 할 오브젝트는 Poolable을 상속받은 컴포넌트가 필요합니다
using System;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    public string spawnSFX = "";
    public string despawnSFX = "";
    protected Action<GameObject> returnToPool;
    public void Init(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    // 풀링에서 꺼낼때 실행
    public virtual void OnSpawn()
    {
        if (spawnSFX != string.Empty)
        {
            SoundManager.PlaySFX(spawnSFX);
        }
    }
    // 풀링으로 돌아갈 때 실행
    public virtual void OnDespawn()
    {
        if (despawnSFX != string.Empty)
        {
            SoundManager.PlaySFX(despawnSFX);
        }
    }

    // 풀링 오브젝트를 다시 풀에 돌리려면 Despawn을 사용합니다.
    public void Despawn()
    {
        returnToPool.Invoke(gameObject);
    }
}