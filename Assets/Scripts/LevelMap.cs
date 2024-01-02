using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelMap : MonoBehaviour
{
    public static LevelMap Instance { get; private set; }

    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private int levelsCount = 30;

    [SerializeField] private List<Button> levelButtons = new List<Button>();
    [SerializeField] private List<GameObject> stars = new List<GameObject>();
    [SerializeField] private List<GameObject> locks = new List<GameObject>();

    [SerializeField] private List<StarsAmount> allStars = new List<StarsAmount>();

    private readonly string Level_Reached_Key = "LevelReachedKey";
    private readonly string Stars_Count_Key = "StarsCountKey";

    private AsyncSceneLoader asyncSceneLoader;

    private List<int> reachedStarsCount = new List<int>();
    private List<string> starsCountKeys = new List<string>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        asyncSceneLoader = AsyncSceneLoader.Instance;

        //PlayerPrefs.DeleteAll();
        InstantiateLevels();
    }

    private void InstantiateLevels()
    {
        int levelReached = PlayerPrefs.GetInt(Level_Reached_Key, 1);
        

        for (int i = 0; i < levelsCount; i++)
        {
            var star_Count_key = Stars_Count_Key + i;
            starsCountKeys.Add(star_Count_key);
            int starsCount = PlayerPrefs.GetInt(starsCountKeys[i], 0);

            GameObject levelButtonObj = Instantiate(levelButtonPrefab, parentTransform, false);
            levelButtons.Add(levelButtonObj.GetComponent<Button>());
            GameObject lockIcon = levelButtonObj.transform.Find("Lock").gameObject;
            GameObject starIcon = levelButtonObj.transform.Find("Stars").gameObject;
            levelButtonObj.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            StarsAmount starsAmount = new StarsAmount();

            int sceneIndex = (i + 2);

            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LevelSelect(sceneIndex));

            if (i + 1 > levelReached)
            {
                starIcon.SetActive(false);
                lockIcon.SetActive(true); 
                levelButtons[i].interactable = false;
            }           

            stars.Add(starIcon);
            locks.Add(lockIcon);

            starsAmount.starsAmount[0] = stars[i].transform.GetChild(0).gameObject;
            starsAmount.starsAmount[1] = stars[i].transform.GetChild(1).gameObject;
            starsAmount.starsAmount[2] = stars[i].transform.GetChild(2).gameObject;

            reachedStarsCount.Add(starsCount);

            if (starsCount == 2)
            {
                starsAmount.starsAmount[2].gameObject.SetActive(false);
            }
            else if (starsCount == 1)
            {
                starsAmount.starsAmount[1].gameObject.SetActive(false);
                starsAmount.starsAmount[2].gameObject.SetActive(false);
            }
            else if (starsCount == 0)
            {
                starsAmount.starsAmount[0].gameObject.SetActive(false);
                starsAmount.starsAmount[1].gameObject.SetActive(false);
                starsAmount.starsAmount[2].gameObject.SetActive(false);
            }

            allStars.Add(starsAmount);
            
        }
    }

    private void LevelSelect(int sceneindex)
    {
        asyncSceneLoader.LoadSceneByIndex(sceneindex);
        Debug.Log($"try to load scene {sceneindex}");
    }

    public  List<int> GetReachedStarsCount()
    {
        return reachedStarsCount;
    }

    public List<string> GetReachedStarsCountKey()
    {
        return starsCountKeys;
    }
}

[System.Serializable]
public class StarsAmount
{
    public GameObject[] starsAmount = new GameObject[3];
}
