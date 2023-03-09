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

    [SerializeField]
    private RectTransform priceTextAddAnimation;

    float currentMoneyTextValue;
    float currentVelovity;

    void Start()
    {
        gc = _GameController.gameController;
        priceTextAddAnimation.gameObject.SetActive(false);
    }

    void Update()
    {
        currentMoneyTextValue = Mathf.SmoothDamp(currentMoneyTextValue, gc.money, ref currentVelovity, .1f);

        moneyText.text = "$ " + Mathf.RoundToInt(currentMoneyTextValue);
        stackCountText.text = gc.player.enemyStack.Count + "/" + gc.player.maxEnemyStack;
        sellPRiceText.text = gc.sellPrice.ToString();
        priceText.text = gc.upgradePrice.ToString();
    }

    public IEnumerator MoneyAdded(int moneyAdded)
    {
        priceTextAddAnimation.gameObject.SetActive(true);
        priceTextAddAnimation.localPosition = new Vector2(priceTextAddAnimation.localPosition.x, 0);

        Text priceAddedText = priceTextAddAnimation.GetComponent<Text>();
        priceAddedText.text = "";
        Color alpha = priceAddedText.color;
        alpha.a = 1;
        if (moneyAdded > 0)
        {
            alpha = Color.yellow;
            priceAddedText.text = "+";
        }
        else alpha = Color.red;
        priceAddedText.text += moneyAdded.ToString();

        float time = 0;

        Vector3 newPosition = new Vector3(0, .1f, 0);

        while (time <= 2)
        {
            time += Time.deltaTime;
            priceAddedText.color = alpha;
            alpha.a = Mathf.Lerp(alpha.a, 0, .02f);
            priceTextAddAnimation.position += newPosition * 15;
            yield return null;
        }
        priceTextAddAnimation.gameObject.SetActive(false);
    }
}
