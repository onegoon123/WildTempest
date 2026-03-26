using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 웨이브 기반 진행으로 적 스폰 시스템 관리
/// 스테이지 기반 웨이브와 무한 생존 모드 모두 처리
/// 깔끔한 코드 구조를 위한 상태 머신 패턴 사용
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    private int waveStart;
    private int waveEnd;
    private int currentWave;

    public GameObject enemyPrefab;
    
    private Transform player;

    private float spawnInterval = 2f;
    private float spawnDistanceMin = 8f;
    private float spawnDistanceMax = 12f;

    private float waveTime;
    private float Timer;
    private List<int> enemies;
    private List<int> enemyCount;
    private int spawnCount;

    /// <summary>
    /// 스포너 동작을 위한 상태 머신
    /// </summary>
    enum SpawnerState
    {
        None,       // 스폰 없음
        Wait,       // 다음 웨이브 대기
        Spawn,      // 현재 적 스폰 중
        Infinity,   // 무한 생존 모드
    }
    SpawnerState curState = SpawnerState.None;

    /// <summary>
    /// 스포너 상태를 변경하고 적절한 초기화를 트리거
    /// </summary>
    void ChangeState(SpawnerState state)
    {
        curState = state;
        switch (curState)
        {
            case SpawnerState.None:
                break;
            case SpawnerState.Wait:
                WaitStart();
                break;
            case SpawnerState.Spawn:
                SpawnStart();
                break;
            case SpawnerState.Infinity:
                InfinityStart();
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        player = GameManager.Instance.player.transform;
        Timer = 0;

        int stage = GameManager.Instance.currentStage;

        // 스테이지 3은 무한 생존 모드
        if (stage == 3)
        {
            ChangeState(SpawnerState.Infinity);
            return;
        }

        // 현재 스테이지의 웨이브 데이터 초기화
        waveStart = Wild.Enemy.Stage.StageMap[stage].WaveStart;
        waveEnd = Wild.Enemy.Stage.StageMap[stage].WaveEnd;

        currentWave = waveStart - 1;
        ChangeState(SpawnerState.Wait);
    }

    void Update()
    {
        Timer += Time.deltaTime;
        switch (curState)
        {
            case SpawnerState.None:
                break;
            case SpawnerState.Wait:
                WaitUpdate();
                break;
            case SpawnerState.Spawn:
                SpawnUpdate();
                break;
            case SpawnerState.Infinity:
                InfinityUpdate();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 다음 웨이브 데이터 초기화
    /// </summary>
    void WaitStart()
    {
        currentWave++;
        if (Wild.Enemy.Wave.WaveMap.ContainsKey(currentWave) == false || waveEnd < currentWave)
        {
            ChangeState(SpawnerState.None);
            Debug.Log("All waves completed");
            return;
        }

        var waveData = Wild.Enemy.Wave.WaveMap[currentWave];
        waveTime = waveData.Time;
        enemies = waveData.Enemies;
        enemyCount = waveData.Counts;
        spawnInterval = waveData.SpawnInterval;
        spawnDistanceMin = waveData.SpawnDistanceMin;
        spawnDistanceMax = waveData.SpawnDistanceMax;
    }

    /// <summary>
    /// 스폰하기 전에 웨이브 시간이 완료될 때까지 대기
    /// </summary>
    void WaitUpdate()
    {
        if (Timer >= waveTime)
        {
            ChangeState(SpawnerState.Spawn);
        }
    }

    /// <summary>
    /// 현재 웨이브의 적 스폰 시작
    /// </summary>
    void SpawnStart()
    {
        spawnCount = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Wild.Enemy.Data.DataMap.ContainsKey(enemies[i]) == false) continue;
            StartCoroutine(SpawnEnemy(enemies[i], enemyCount[i]));
        }
    }

    /// <summary>
    /// 현재 웨이브의 모든 적이 스폰되었는지 확인
    /// </summary>
    void SpawnUpdate()
    {
        if (spawnCount == enemies.Count)
        {
            ChangeState(SpawnerState.Wait);
        }
    }

    /// <summary>
    /// 간격을 두고 적을 스폰하는 코루틴
    /// </summary>
    /// <param name="enemy">적 타입 ID</param>
    /// <param name="count">스폰할 적의 수</param>
    IEnumerator SpawnEnemy(int enemy, int count)
    {
        int spawned = 0;
        while (spawned < count)
        {
            // 플레이어 주변의 랜덤 스폰 위치 계산
            Vector2 spawnDir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(spawnDistanceMin, spawnDistanceMax);
            Vector2 spawnPos = (Vector2)player.position + spawnDir * dist;

            // 객체 풀에서 적 가져오기
            Enemy newEnemy = ObjectPoolManager.Instance.Get("Enemy").GetComponent<Enemy>();
            newEnemy.transform.position = spawnPos;

            // 특정 데이터로 적 초기화
            newEnemy.EnemyInit(enemy);
            spawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
        spawnCount++;
        yield return null;
    }

    /// <summary>
    /// 랜덤 위치에 단일 적 스폰
    /// 무한 모드에서 사용
    /// </summary>
    /// <param name="enemy">적 타입 ID</param>
    void Spawn(int enemy)
    {
        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(spawnDistanceMin, spawnDistanceMax);
        Vector2 spawnPos = (Vector2)player.position + spawnDir * dist;

        Enemy newEnemy = ObjectPoolManager.Instance.Get("Enemy").GetComponent<Enemy>();
        newEnemy.transform.position = spawnPos;

        newEnemy.EnemyInit(enemy);
    }

    /// <summary>
    /// 무한 생존 모드 초기화
    /// </summary>
    void InfinityStart()
    {
        currentWave = 0;
        waveTime = 0;
    }

    /// <summary>
    /// 무한 생존 모드 업데이트
    /// 점진적으로 난이도를 증가시키고 랜덤 적을 스폰
    /// </summary>
    void InfinityUpdate()
    {
        // 다음 난이도 레벨로 진행
        if (Wild.Enemy.InfiStage.InfiStageList[currentWave+1].Time < Timer)
        {
            currentWave++;
        }

        waveTime -= Time.deltaTime;
        if (waveTime < 0)
        {
            // 현재 난이도 레벨에서 랜덤 적 스폰
            int count = Wild.Enemy.InfiStage.InfiStageList[currentWave].Enemies.Count;
            int enemy = Wild.Enemy.InfiStage.InfiStageList[currentWave].Enemies[Random.Range(0, count)];
            Spawn(enemy);
            waveTime = Wild.Enemy.InfiStage.InfiStageList[currentWave].SpawnInterval;
        }
    }
}