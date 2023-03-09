using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyAndSell : MonoBehaviour
{
    _GameController gc;
    delegate void ButtonDelegate();
    ButtonDelegate buttonDesiredFunction;

    [SerializeField]
    private float sellMoveSpeed;
    [SerializeField]
    private float maxSellMoveSpeed;

    private void Start()
    {
        gc = _GameController.gameController;
    }
    #region Sell
    public void Sell()
    {
        buttonDesiredFunction = SellMoveStack;
        buttonDesiredFunction += () => { Camera.main.GetComponent<CameraFollow>().SetNewOffset(0); };
        Image maskImage = GetComponentInChildren<Mask>().GetComponent<Image>();
        StartCoroutine(AnimateButton(maskImage, transform, 1, new Vector3(1.1f, 1.1f, 1.1f), buttonDesiredFunction));
        
    }

    void SellMoveStack()
    {
        List<Transform> sellList = gc.player.GetAndClearStackList();
        gc.AddMoney(sellList.Count);
        gc.StartCoroutine(gc.MoveTransformPosition(sellList, transform.position, Mathf.Min(sellMoveSpeed * sellList.Count, maxSellMoveSpeed)));
    }
    #endregion

    public void Buy()
    {
        buttonDesiredFunction = BuyLevelUp;
        Image maskImage = GetComponentInChildren<Mask>().GetComponent<Image>();
        StartCoroutine(AnimateButton(maskImage, transform, 1, new Vector3(1.1f, 1.1f, 1.1f), buttonDesiredFunction));

    }

    void BuyLevelUp()
    {
        gc.BuyUpgrade();
    }

    private IEnumerator AnimateButton(Image mask, Transform buttonTransform, float desiredFill, Vector3 newScale, ButtonDelegate function)
    {
        while (true)
        {
            // Anima o botão
            while (mask.fillAmount != desiredFill)
        {
            buttonTransform.transform.localScale = Vector3.Lerp(buttonTransform.transform.localScale, newScale, 2 * Time.deltaTime);
            mask.fillAmount = Mathf.MoveTowards(mask.fillAmount, desiredFill, 1.25f * Time.deltaTime);
            yield return null;
        }
        // Executa ação customizável enquanto estiver ativo depois de animar o botão
            function();
            yield return new WaitForSeconds(.25f);
            mask.fillAmount = 0;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ResetButton()
    {
        StopAllCoroutines();
        transform.localScale = new Vector3(1, 1, 1);
        GetComponentInChildren<Mask>().GetComponent<Image>().fillAmount = 0;
    }
}
