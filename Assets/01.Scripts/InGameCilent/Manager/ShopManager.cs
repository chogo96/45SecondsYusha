using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class ShopManager : SingletonMonoBase<ShopManager>
{
    //돈과 가루 프로퍼티 화
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
            Debug.Log("미 개봉 팩 존재함!" + PlayerPrefs.GetInt("UnOpenedPacks"));
            StartCoroutine(GivePacks(PlayerPrefs.GetInt("UnopenedPacks"), true));
        }

        LoadGoodsToPlayerPrefs();
    }


    /// <summary>
    /// 구매 할 때 터지는 함수 이때 가격 차감과 팩이 동시에 사진다
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
    /// 종료 시 플레이어의 재화를 저장함 
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
    /// 플레이어의 재화를 불러오는 함수임  awake에서 터짐
    /// </summary>
    public void LoadGoodsToPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("Dust"))
        {
            Dust = PlayerPrefs.GetInt("Dust");
        }
        else
        {
            Dust = StartingAmountOfDust; //처음 플레이어가 시작했을 때 가지고 있을 양의 가루양임
        }

        if (PlayerPrefs.HasKey("Gold"))
        {
            Gold = PlayerPrefs.GetInt("Gold");
        }
        //없는 경우임 처음 시작하는 것 외에는 값이 없을수가 없음
        else
        {
            Gold = StartingAmountOfGold;
        }
    }
}
