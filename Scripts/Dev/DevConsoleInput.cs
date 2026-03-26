using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevConsoleInput : MonoBehaviour
{
    private bool showConsole = false;
    private string input = "";
    private bool focusInputField = false;

    private InputAction toggleConsoleAction;
    private InputAction submitCommandAction;
    void Awake()
    {
        // `~` 키로 콘솔 열기
        toggleConsoleAction = new InputAction(binding: "<Keyboard>/backquote");
        toggleConsoleAction.performed += ctx => ToggleConsole();
        toggleConsoleAction.Enable();

        // `Enter` 키로 명령어 실행
        submitCommandAction = new InputAction(binding: "<Keyboard>/enter");
        submitCommandAction.performed += ctx => TrySubmitCommand();
        submitCommandAction.Enable();
    }

    void ToggleConsole()
    {
        showConsole = !showConsole;
        focusInputField = showConsole;
    }

    void TrySubmitCommand()
    {
        if (!showConsole) return;

        ExecuteCommand(input);
        input = "";
        focusInputField = true; // 다시 포커스
    }

    void ExecuteCommand(string cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd)) return;

        string[] parts = cmd.Trim().Split(' ');

        if (parts.Length == 2 && parts[0].ToLower() == "item")
        {
            if (int.TryParse(parts[1], out int itemCode))
            {
                GameManager.Instance.player.Inventory.AddItem(itemCode);
            }
            else
            {
                Debug.LogWarning("숫자를 입력해야 합니다. 예: item 101");
            }
        }
        else if (parts.Length == 2 && parts[0].ToLower() == "money")
        {
            if (int.TryParse(parts[1], out int money))
            {
                DataManager.data.Money += money;
            }
            else
            {
                Debug.LogWarning("숫자를 입력해야 합니다. 예: money 101");
            }
        }
        else if (parts.Length == 3 && parts[0].ToLower() == "item")
        {
            if (int.TryParse(parts[1], out int itemCode) && int.TryParse(parts[2], out int count))
            {
                for (int i = 0; i < count; i++)
                {
                    GameManager.Instance.player.Inventory.AddItem(itemCode);
                }
            }
        }
        else if (parts.Length == 2 && parts[0].ToLower() == "exp")
        {
            if (int.TryParse(parts[1], out int exp))
            {
                GameManager.Instance.player.AddEXP(exp);
            }
        }
        else if (cmd == "god")
        {
            GameManager.Instance.player.SetMaxHP(2100000000);
        }
        else if (cmd == "gameover")
        {
            CustomEvent.Trigger(GameObject.Find("Overlay Canvas"), "GameOver");
        }
        else if (cmd == "clear")
        {
            CustomEvent.Trigger(GameObject.Find("Overlay Canvas"), "Clear");
        }
        else if (cmd == "lu")
        {
            GameManager.Instance.UIManager.LevelUp(0f);
        }
        else if (parts.Length == 2 && parts[0].ToLower() == "time")
        {
            if (int.TryParse(parts[1], out int scale))
            {
                Time.timeScale = scale;
            }
        }
        else if (cmd == "tr1")
        {
            TreasureManager.Instance.TreasureOpen(1);
        }
        else if (cmd == "tr")
        {
            TreasureManager.Instance.TreasureOpen(3);
        }
        else if (cmd == "fullitem")
        {
            for (int i = 0; i < 5; i++)
            {
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Fireball);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Sword);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.TripleLightning);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Meteor);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Axe);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.AttackRing);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.SpeedBoots);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Acce);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Robe);
                GameManager.Instance.player.Inventory.AddItem(ItemCode.Quiver);
            }
            GameManager.Instance.player.Inventory.AddItem(ItemCode.FireEx);
            GameManager.Instance.player.Inventory.AddItem(ItemCode.HeroSword);
            GameManager.Instance.player.Inventory.AddItem(ItemCode.Lightning);
            GameManager.Instance.player.Inventory.AddItem(ItemCode.FireMeteor);
            GameManager.Instance.player.Inventory.AddItem(ItemCode.HeroAxe);
        }
        else
        {
            Debug.LogWarning($"알 수 없는 명령어: {cmd}");
        }
    }

    void OnGUI()
    {
        if (!showConsole) return;

        GUI.Box(new Rect(10, 10, 400, 100), "Console");

        GUI.SetNextControlName("ConsoleInputField");
        input = GUI.TextField(new Rect(20, 40, 380, 20), input);

        // 콘솔을 연 직후 자동 포커스
        if (focusInputField)
        {
            GUI.FocusControl("ConsoleInputField");
            focusInputField = false;
        }
    }

    void OnDestroy()
    {
        toggleConsoleAction.Dispose();
        submitCommandAction.Dispose();
    }
}
