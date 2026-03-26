using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 게임 내 모든 아이템의 고유 코드를 정의하는 열거형입니다.
/// 무기, 진화 아이템, 액세서리, 특수 아이템으로 분류됩니다.
/// </summary>
public enum ItemCode
{
    WeaponStart = 100,
    Fireball = 101,
    Sword,
    TripleLightning,
    Meteor,
    Axe,
    Staff,
    Grimoire,
    Frost,
    WeaponEnd,
    UpStart = 150,
    FireEx = 151,
    HeroSword,
    Lightning,
    FireMeteor,
    HeroAxe,
    MagicStaff,
    GrimoireEX,
    FrostEX,
    UpEnd,
    AcceStart = 200,
    AttackRing,
    SpeedBoots,
    Acce,
    Robe,
    Quiver,
    Book,
    DarkRing,
    Hourglass,
    AcceEnd,
    Potion = 301,
    Money,
}

/// <summary>
/// 게임 내 모든 아이템(무기, 액세서리)의 추상 기본 클래스입니다.
/// 
/// 주요 기능:
/// - 쿨다운 기반 자동 활성화 시스템
/// - 레벨업을 통한 스탯 증가
/// - 장착 시 초기화 처리
/// 
/// 파생 클래스에서 구현해야 할 메서드:
/// - Activate: 아이템의 고유 기능 실행
/// - OnEquip: 장착 시 초기화 로직
/// - LevelUp: 레벨업 시 스탯 증가 로직
/// </summary>
public abstract class ItemBase
{
    public ItemCode itemCode { get; private set; }
    protected Player player;
    protected int maxLevel = 0;
    public int level { get; protected set; }
    protected float cooldown;
    protected float cooldownTimer = 0;
    public static float cooldownReduction = 0.0f;
    
    /// <summary>
    /// 아이템을 초기화합니다.
    /// 데이터 테이블에서 최대 레벨을 로드하고 첫 레벨업을 수행합니다.
    /// </summary>
    /// <param name="code">아이템의 고유 코드</param>
    /// <param name="player">아이템을 소유할 플레이어</param>
    public void Init(ItemCode code, Player player)
    {
        itemCode = code;
        this.player = player;
        
        // 데이터 테이블에서 최대 레벨 로드
        maxLevel = Wild.Item.Data.DataMap[(int)code].MaxLevel;
        level = 0;
        LevelUp();

        // 초기 쿨다운 설정
        cooldownTimer = cooldown * (1.0f - cooldownReduction);
        OnEquip();
    }

    /// <summary>
    /// PlayerInventory에서 호출되는 메인 업데이트 루프입니다.
    /// 쿨다운 타이머를 관리하고 자동으로 아이템을 활성화합니다.
    /// </summary>
    public virtual void Tick()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer < 0)
        {
            // 아이템 활성화
            if (Activate())
            {
                // 활성화 성공 시 쿨다운 재설정
                cooldownTimer = cooldown * (1.0f - cooldownReduction);
            }
        }
    }

    /// <summary>
    /// 아이템의 고유 기능을 활성화합니다.
    /// 파생 클래스에서 구현해야 합니다.
    /// </summary>
    /// <returns>활성화가 성공했으면 true, 아니면 false</returns>
    public virtual bool Activate() { return false; }
    
    /// <summary>
    /// 아이템이 장착될 때 호출되는 초기화 메서드입니다.
    /// 파생 클래스에서 구현해야 합니다.
    /// </summary>
    public abstract void OnEquip();
    
    /// <summary>
    /// 아이템의 레벨업을 처리합니다.
    /// 파생 클래스에서 구현해야 합니다.
    /// </summary>
    public abstract void LevelUp();
    
    /// <summary>
    /// 아이템이 최대 레벨인지 확인합니다.
    /// </summary>
    /// <returns>최대 레벨이면 true, 아니면 false</returns>
    public bool IsMaxLevel() => level == maxLevel;
}