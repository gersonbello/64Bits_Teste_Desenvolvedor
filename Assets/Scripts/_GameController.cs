using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _GameController : MonoBehaviour
{
    private static _GameController _gameController;
    // Cria singleton para gerenciamento de dados e métodos
    public static _GameController gameController
    {
        get
        {
            if (_gameController == null) _gameController = FindObjectOfType<_GameController>();
            if (_gameController == null) _gameController = Instantiate(new GameObject("GameController")).AddComponent<_GameController>();
            return _gameController;
        }
    }

    private static PlayerBaseBehaviour _player;
    public PlayerBaseBehaviour player
    {
        get { if (_player == null) _player = FindObjectOfType<PlayerBaseBehaviour>(); return _player; }
    }

    private static Joystick _joystick;
    public Joystick joystick
    {
        get { if (_joystick == null) _joystick = FindObjectOfType<Joystick>(); return _joystick; }
    }

    private static HUD _hud;
    public HUD hud
    {
        get { if (_hud == null) _hud = FindObjectOfType<HUD>(); return _hud; }
    }

    public int money { get; private set; }

    public int upgradePrice { get; private set; } = 5;

    public int sellPrice { get; private set; } = 2;

    public void BuyUpgrade()
    {
        if(money >= upgradePrice)
        {
            AddMoney(-upgradePrice);
            player.LevelUP();
            upgradePrice *= 2;
            sellPrice += 2;
        }
    }

    public void AddMoney(int moneyToAdd)
    {
        money += moneyToAdd;
        if (moneyToAdd != 0)
        {
            hud.StopAllCoroutines();
            hud.StartCoroutine(hud.MoneyAdded(moneyToAdd));
        }
    }

    public IEnumerator MoveTransformPosition(List<Transform> transforms, Vector3 targetPosition, float moveSpeed)
    {
        while (transforms.Count > 0)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                transforms[i].position = Vector3.Lerp(transforms[i].position, targetPosition, moveSpeed * Time.deltaTime);
                if((Vector3.Distance(transforms[i].position, targetPosition) < 1))
                {
                    transforms[i].gameObject.SetActive(false);
                    transforms.Remove(transforms[i]);
                }
            }
            yield return null;
        }
    }
}
