using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class EnemyUIManager : MonoBehaviour
{
    public TMP_Text requiredSwordText;
    public TMP_Text requiredMagicText;
    public TMP_Text requiredShieldText;
    public Transform swordImageParent;
    public Transform magicImageParent;
    public Transform shieldImageParent;
    public GameObject swordImagePrefab;
    public GameObject magicImagePrefab;
    public GameObject shieldImagePrefab;

    public void UpdateUI(int requiredSword, int requiredMagic, int requiredShield)
    {
        // �ؽ�Ʈ ������Ʈ
        requiredSwordText.text = requiredSword.ToString();
        requiredMagicText.text = requiredMagic.ToString();
        requiredShieldText.text = requiredShield.ToString();

        // �̹��� ���� ������Ʈ �� �ʱ� ���İ� ����
        UpdateImages(swordImageParent, swordImagePrefab, requiredSword);
        UpdateImages(magicImageParent, magicImagePrefab, requiredMagic);
        UpdateImages(shieldImageParent, shieldImagePrefab, requiredShield);
    }

    private void UpdateImages(Transform parent, GameObject prefab, int count)
    {
        // ���� �̹��� ����
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        if (BuffManager.instance.ConfusionDebuff)
        {
            Debug.Log("ȥ���������� ���� �䱸ġ�� ������ �ʽ��ϴ�!!!!");
            return;
        }
        // ���ο� �̹��� ���� �� �ʱ� ���İ� ����
        for (int i = 0; i < count; i++)
        {
            var instance = Instantiate(prefab, parent);
            var image = instance.GetComponent<Image>();
            if (image != null)
            {
                Color color = image.color;
                color.a = 100 / 255f; // �ʱ� ���İ� ����
                image.color = color;
            }
        }
    }

    public void ChangeAlphaForIncrement(int increment, Transform parent, int currentAmount, int requiredAmount)
    {
        Debug.Log($"ChangeAlphaForIncrement called with increment: {increment}, currentAmount: {currentAmount}, requiredAmount: {requiredAmount}");

        if (increment <= 0) return;

        int changedCount = 0;
        foreach (Transform child in parent)
        {
            if (changedCount >= increment || currentAmount >= requiredAmount)
                break;

            var image = child.GetComponent<Image>();
            if (image != null && image.color.a < 1f)
            {
                Color color = image.color;
                color.a = 1f; // ���İ��� 255�� ����
                image.color = color;
                changedCount++;
                Debug.Log($"Changed alpha for {parent.name} at index {changedCount}");
            }
        }

        Debug.Log($"ChangeAlphaForIncrement completed for {parent.name}, total changed: {changedCount}");
    }
}
