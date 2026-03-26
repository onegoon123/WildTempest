using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using DG.Tweening;
using GooglePlayGames;

/// <summary>
/// 메인 플레이어 컨트롤러 클래스
/// 플레이어 스탯, 이동, 인벤토리, 전투 시스템 처리
/// 
/// 관련 클래스:
/// PlayerMovement - 플레이어 이동 로직을 담당하는 클래스
/// PlayerInventory - 플레이어 아이템, 장비를 관리하는 클래스
/// </summary>
public class Player : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Slider hpSlider;

    public PlayerMovement Movement { get; private set; }
    public PlayerInventory Inventory { get; private set; }

    private int MaxHP = 100;            // 최대 체력
    private int currentHP = 100;        // 현재 체력
    public static int RegenHP = 0;      // 체력 회복 (1초마다 회복)

    public float invincibilityTime = 0.2f;     // 피해 후 무적 시간
    private float invincibilityTimer = 0;       // 현재 타이머

    private int level = 1;          // 현재 레벨
    private int currentEXP;         // 현재 경험치
    private int nextEXP;            // 경험치 필요량

    public bool isClear = false;    // 스테이지 클리어 체크 (게임오버 방지)

    // UI 참조
    public UIManager UI;

    private void Awake()
    {
        Movement = new PlayerMovement(this);
        Inventory = new PlayerInventory(this);
    }

    private void Start()
    {
        // ____ 캐릭터 스탯 설정 ____
        int currentHero = DataManager.data.currentHero;
        var heroData = Wild.Player.Hero.HeroMap[currentHero];
        MaxHP = heroData.HP;                            // 최대체력
        WeaponBase.DamagePercent = heroData.ATK;        // 공격력
        PlayerMovement.SpeedPercent = heroData.Speed;   // 이동속도
        Inventory.AddItem(heroData.Weapon);             // 시작 무기

        // ____ 캐릭터 애니메이션 컨트롤러 설정 ____
        animator.runtimeAnimatorController = AssetManager.Get<RuntimeAnimatorController>(heroData.Resource);

        // ____ 업그레이드 보너스 적용 ____
        var datamap = Wild.Player.UpgradeData.UpgradeDataMap;   // 업그레이드 데이터맵

        MaxHP += DataManager.data.upgrades[0] * (int)datamap[1001].Value;                       // 최대 체력
        currentHP = MaxHP;                                                                      // 현재 체력
        WeaponBase.DamagePercent += DataManager.data.upgrades[1] * datamap[1002].Value;         // 공격력
        ItemBase.cooldownReduction = 0.0f + DataManager.data.upgrades[2] * datamap[1003].Value; // 쿨다운
        PlayerMovement.SpeedPercent += DataManager.data.upgrades[3] * datamap[1004].Value;      // 이동속도
        RegenHP = 0 + DataManager.data.upgrades[4] * (int)datamap[1005].Value;                  // 체력 회복
        EXPPercent = 1.0f + DataManager.data.upgrades[5] * datamap[1006].Value;                 // 경험치
        GameManager.MoneyPercent = 1.0f + DataManager.data.upgrades[6] * datamap[1007].Value;   // 돈
        EXPOrb.ColRadius = .5f * (1.0f + DataManager.data.upgrades[7] * Wild.Player.UpgradeData.UpgradeDataMap[1008].Value);    // 자석

        level = 1;                                                  // 시작 레벨
        nextEXP = Wild.Player.EXPData.EXPDataMap[level].NextEXP;    // 경험치 필요량

        WeaponBase.ProjectileCount = 0; // 투사체 개수
        
        // 체력 회복 시스템 초기화
        InvokeRepeating(nameof(RegenerateHealth), 1f, 1f);
    }

    private void Update()
    {
        Movement.Tick();
        Inventory.Tick();

        invincibilityTimer -= Time.deltaTime;   // 무적 시간
    }

    private void FixedUpdate()
    {
        Movement.Tick_Fixed();
    }

    /// <summary>
    /// 플레이어가 받는 피해 처리
    /// </summary>
    /// <param name="damage">받을 피해량</param>
    public void TakeDamage(int damage)
    {
        if (0 < invincibilityTimer) return;     // 무적 시간 중 피해 X
        if (isClear) return;                    // 클리어 후 피해 X

        invincibilityTimer = invincibilityTime; // 무적 시간 설정
        currentHP -= damage;                    // 체력 감소

        if (currentHP <= 0)
        {
            // _____ 게임오버 처리 _____

            // 1. 죽음 애니메이션
            DG.Tweening.Sequence sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(Color.red, .1f).SetEase(Ease.OutQuad));
            sequence.Append(spriteRenderer.DOColor(Color.clear, .3f));
            sequence.Join(hpSlider.transform.GetChild(0).GetComponent<Image>().DOColor(Color.clear, .3f));
            sequence.Play();

            // 2. Rigidbody2D 및 컴포넌트 비활성화
            GetComponent<Rigidbody2D>().simulated = false;
            enabled = false;

            // 3. 게임오버 UI 트리거
            CustomEvent.Trigger(UI.gameObject, "GameOver");

            if (PlayGamesPlatform.Instance.IsAuthenticated())
                PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement);

            FindFirstObjectByType<SurvivalTimer>().StopTimer();
        }
        else
        {
            // 피해 애니메이션
            DG.Tweening.Sequence sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(Color.red, .1f).SetEase(Ease.OutQuad));
            sequence.Append(spriteRenderer.DOColor(Color.white, .1f));
            sequence.Play();
        }

        // 체력바 Slider 처리
        DOTween.Kill(hpSlider);
        float targetValue = (float)currentHP / MaxHP;
        float duration = Mathf.Abs(targetValue - hpSlider.value);
        hpSlider.DOValue(targetValue, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 체력 회복 처리
    /// </summary>
    /// <param name="value">회복할 체력량</param>
    public void TakeHeal(int value)
    {
        currentHP = Mathf.Min(MaxHP, currentHP + value);    // 체력 증가

        // 체력바 Slider 처리
        DOTween.Kill(hpSlider);
        float targetValue = (float)currentHP / MaxHP;
        float duration = Mathf.Abs(targetValue - hpSlider.value);
        hpSlider.DOValue(targetValue, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 자동 체력 회복 (InvokeRepeating용)
    /// </summary>
    private void RegenerateHealth()
    {
        if (RegenHP > 0)
            TakeHeal(RegenHP);
    }

    public static float EXPPercent = 1.0f;
    
    /// <summary>
    /// 경험치 획득 및 레벨업 처리
    /// </summary>
    /// <param name="value">획득할 경험치</param>
    public void AddEXP(int value)
    {
        currentEXP += (int)(value * EXPPercent);    // 경험치 획득
        if (nextEXP <= currentEXP)
        {
            // _____ 레벨업 처리 _____

            // 1. 경험치 필요량만큼 현재 경험치 차감
            currentEXP -= nextEXP;

            // 2. 경험치 필요량 업데이트
            if (Wild.Player.EXPData.EXPDataMap.ContainsKey(level+1))
                nextEXP = Wild.Player.EXPData.EXPDataMap[++level].NextEXP;
            else
                nextEXP = Wild.Player.EXPData.EXPDataMap[level].NextEXP;

            // 3. 레벨업 UI 트리거
            UI.LevelUp((float)currentEXP / nextEXP);
            return;
        }

        UI.SetEXPValue((float)currentEXP / nextEXP);    // 경험치 바 Slider 처리
    }

    public void AddMaxHP(int value) => SetMaxHP(MaxHP + value);

    /// <summary>
    /// 최대 체력 설정 및 현재 체력 조정
    /// </summary>
    /// <param name="value">새로운 최대 체력</param>
    public void SetMaxHP(int value)
    {
        currentHP += value - MaxHP; // 최대 체력 차이만큼 HP 증가
        MaxHP = value;              // 최대체력 설정

        // 체력바 Slider 처리
        DOTween.Kill(hpSlider);
        float targetValue = (float)currentHP / MaxHP;
        hpSlider.DOValue(targetValue, 1.0f).SetEase(Ease.OutQuad);
    }
}