using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class ShopManager : SingletonMonoBase<ShopManager>
{
    //���� ���� ������Ƽ ȭ
    private int _gold;
    public int Gold
    {
        get { return _gold; }
        set
        {
            _gold = value;
            MoneyText.text = _gold.ToString();
        }
    }
    private int _dust;
    public int Dust
    {
        get { return _dust; }
        set
        {
            _dust = value;
            DustText.text = _dust.ToString();
        }
    }

    public GameObject ScreenContent;
    public GameObject PackPrefab;
    public int PackPrice;
    public Transform PacksParent;
    public Transform InitialPackSpot;
    public float PosXRange = 4f;
    public float PosYRange = 8f;
    public float RotationRange = 10f;
    public Text MoneyText;
    public Text DustText;
    public GameObject GoldHUD;
    public GameObject DustHUD;
    public PackOpeningArea OpeningArea;

    public int StartingAmountOfDust = 1000;
    public int StartingAmountOfGold = 1000;

    public int PacksCreated { get; set; }
    private float _packPlacementOffset = -0.01f;

    private void Awake()
    {
        HideScreen();

        if (PlayerPrefs.HasKey("UnopenedPacks"))
        {
            Debug.Log("�� ���� �� ������!" + PlayerPrefs.GetInt("UnOpenedPacks"));
            StartCoroutine(GivePacks(PlayerPrefs.GetInt("UnopenedPacks"), true));
        }

        LoadGoodsToPlayerPrefs();
    }


    /// <summary>
    /// ���� �� �� ������ �Լ� �̶� ���� ������ ���� ���ÿ� ������
    /// </summary>
    public void BuyPack()
    {
        if(_gold >= PackPrice)
        {
            Gold -= PackPrice;
            StartCoroutine(GivePacks(1));
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="NumberOfPacks"></param>
    /// <param name="instant"></param>
    /// <returns></returns>
    public IEnumerator GivePacks(int NumberOfPacks, bool instant = false)
    {
        for (int i = 0; i < NumberOfPacks; i++)
        {
            GameObject newPack = Instantiate(PackPrefab, PacksParent);
            Vector3 localPositionForNewPack = new Vector3(Random.Range(-PosXRange, PosXRange), Random.Range(-PosYRange, PosYRange), PacksCreated * _packPlacementOffset);
            newPack.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-RotationRange, RotationRange));
            PacksCreated++;

            newPack.GetComponentInChildren<Canvas>().sortingOrder = PacksCreated;
            if (instant)
            {
                newPack.transform.localPosition = localPositionForNewPack;
            }
            else
            {
                newPack.transform.position = InitialPackSpot.position;
                newPack.transform.DOLocalMove(localPositionForNewPack, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield break;
    }

    private void OnApplicationQuit()
    {
        SaveGoodsToPlayerPrefs();

        PlayerPrefs.SetInt("UnopenedPacks", PacksCreated);
    }

    /// <summary>
    /// ���� �� �÷��̾��� ��ȭ�� ������ 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SaveGoodsToPlayerPrefs()
    {
        PlayerPrefs.SetInt("Dust", _dust);
        PlayerPrefs.SetInt("Gold", _gold);
    }

    public void HideScreen()
    {
        ScreenContent.SetActive(false);
        GoldHUD.SetActive(false);
    }
    public void ShowScreen()
    {
        ScreenContent.SetActive(true);
        GoldHUD.SetActive(true);
    }

    /// <summary>
    /// �÷��̾��� ��ȭ�� �ҷ����� �Լ���  awake���� ����
    /// </summary>
    public void LoadGoodsToPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("Dust"))
        {
            Dust = PlayerPrefs.GetInt("Dust");
        }
        else
        {
            Dust = StartingAmountOfDust; //ó�� �÷��̾ �������� �� ������ ���� ���� �������
        }

        if (PlayerPrefs.HasKey("Gold"))
        {
            Gold = PlayerPrefs.GetInt("Gold");
        }
        //���� ����� ó�� �����ϴ� �� �ܿ��� ���� �������� ����
        else
        {
            Gold = StartingAmountOfGold;
        }
    }
}
