using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    _GameController gc;
    [SerializeField]
    private Text moneyText;
    [SerializeField]
    private Text stackCountText;
    [SerializeField]
    private Text sellPRiceText;
    [SerializeField]
    private Text priceText;

    // Start is called before the first frame update
    void Start()
    {
        gc = _GameController.gameController;
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "$ " + gc.money;
        stackCountText.text = gc.player.enemyStack.Count + "/" + gc.player.maxEnemyStack;
        sellPRiceText.text = gc.sellPrice.ToString();
        priceText.text = gc.upgradePrice.ToString();
    }
}
