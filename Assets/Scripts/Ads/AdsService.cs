using UnityEngine;

public class AdsService : MonoBehaviour
{
    public static AdsService Instance { get; private set; }


    [field: SerializeField] public AdsInitializer adsInitializer { get; private set; }
    [field: SerializeField] public InterstitialAds interstitialAds { get; private set; }

    //[field: SerializeField] public RewardedAds rewardedAds { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        interstitialAds.LoadAd();
        //rewardedAds.LoadAd();
    }
}
