using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private GameObject scrollView, scrollbar, purchaseUI, fadeOut;

    [SerializeField]
    private GameObject[] Contents, Stages, Vehicles;

    private GameObject[] content;

    [SerializeField]
    private Text moneyText, cantBuyText;

    [SerializeField]
    private AudioSource audio;

    private float scroll_pos = 0, distance;
    private float[] pos;

    private int selectedMenuIndex, selectedIndex;
    private bool changeIndex = true, start = true;

    private void Start() {
        if(!PlayerPrefs.HasKey("Stage")) {
            PlayerPrefs.SetInt("Stage", 0);
            PlayerPrefs.SetInt("Vehicle", 1);
            PlayerPrefs.SetInt("Stage_Mars", 0);
            PlayerPrefs.SetInt("Stage_Cave", 0);
            PlayerPrefs.SetInt("Vehicle_Motorcycle", 0);
            PlayerPrefs.SetInt("Money", 500000);
        }
        LoadData();
        MenuChange(1);
        start = false;
    }

    private void LoadData() {
        /*Vehicles[1].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Vehicle_Motorcycle").Equals(0));
        Vehicles[1].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Vehicle_Motorcycle").Equals(0);
        Vehicles[2].transform.GetChild(1).gameObject.SetActive(true);
        Vehicles[3].transform.GetChild(1).gameObject.SetActive(true);*/

        moneyText.text = PlayerPrefs.GetInt("Money").ToString();
    }

    private void Update() {
        if(Input.GetMouseButton(0)) {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else {
            for(int i = 0; i < pos.Length; i++) {
                if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    selectedIndex = i;
                }
            }
            changeIndex = true;
        }

        for(int i = 0; i < pos.Length; i++) {
            if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) {
                content[i].transform.localScale = Vector2.Lerp(content[i].transform.localScale, new Vector2(1.2f, 1.2f), 0.1f);
                for(int j = 0; j < pos.Length; j++)
                    if(j != i)
                        content[j].transform.localScale = Vector2.Lerp(content[j].transform.localScale, new Vector2(0.8f, 0.8f), 0.1f);

                if(changeIndex) {
                    SaveSelectedData(i);
                    changeIndex = false;
                }
            }
        }
    }

    public void MenuChange(int index) {
        if(!CheckPurchased() && !start) {
            if(!(selectedMenuIndex == 0 && selectedIndex > 1 || selectedMenuIndex == 1 && selectedIndex > 1)) {
                purchaseUI.SetActive(true);
                return;
            }
        }    
        selectedMenuIndex = index;

        pos = new float[Contents[index].transform.childCount];
        distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++)
            pos[i] = distance * i;

        if(index.Equals(0)) {
            content = Stages;
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value = pos[PlayerPrefs.GetInt("Stage")];
        }
        else if(index.Equals(1)) {
            content = Vehicles;
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value = pos[PlayerPrefs.GetInt("Vehicle")];
        }

        foreach(var obj in Contents)
            obj.SetActive(false);
        Contents[index].SetActive(true);
        scrollView.GetComponent<ScrollRect>().content = Contents[index].GetComponent<RectTransform>();
    }

    private bool CheckPurchased() {
        if(selectedMenuIndex.Equals(0)) {
            if(selectedIndex != 0)
                return !Stages[selectedIndex].transform.GetChild(1).gameObject.activeSelf;
        }
        else { 
            if(selectedIndex != 0) 
                return !Vehicles[selectedIndex].transform.GetChild(1).gameObject.activeSelf;
        }
        return true;
    }

    private void SaveSelectedData(int index) {
        if(selectedMenuIndex.Equals(0)) {
            if(!CheckPurchased()) return; 
            PlayerPrefs.SetInt("Stage", index);
        }
        else {
            if(!CheckPurchased()) return; 
            PlayerPrefs.SetInt("Vehicle", index);
        }
    }

    public void GameStart() {
        if(!CheckPurchased()) {
            if(!(selectedMenuIndex == 0 && selectedIndex > 1 || selectedMenuIndex == 1 && selectedIndex > 1)) {
                purchaseUI.SetActive(true);
                return;
            }
        }
        fadeOut.GetComponent<Animator>().SetTrigger("FadeOut");
    }
}