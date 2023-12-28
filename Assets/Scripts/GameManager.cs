using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private Image fuelGauge, captureImg;
    private Texture2D textureImg;
    private Sprite spriteImg;

    [SerializeField]
    private GameObject fuelWarning, fadeIn, pauseUI, gameOverUI;

    [SerializeField]
    private Text moneyText, moneyEarnedText, distanceText, totaldistanceText, gameStateText;

    [SerializeField]
    private AudioSource[] audio;

    private int totalMoney, moneyEarned = 0;

    public ObjectManager objectManager;
    public CameraController cameraController;
    private CarController carController;

    public bool GasBtnPressed { get; set; }
    public bool BrakeBtnPressed { get; set; }
    public bool isDie { get; set; }
    public bool ReachGoal { get; set; }

    private void Start() {
        Time.timeScale = 1f;
        isDie = false;
        ReachGoal = false;
        fadeIn.GetComponent<Animator>().SetTrigger("FadeIn");
        Initialize();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))  
            GamePause();

        if(!gameOverUI.activeSelf)
            distanceText.text = (int)(carController.transform.position.x - carController.StartPos.x) + "m / <color=yellow>1427m</color>";

        if(isDie && Input.GetMouseButtonDown(0) && gameOverUI.activeSelf) 
            LoadScene(0);

        if(GasBtnPressed || BrakeBtnPressed)
            PlaySound("engine");
    }

    private void Initialize() {
        string objName = "";
        int stageIndex = 0, vehicleIndex = 0;

        objName = "Country";
        objectManager.GetObject(objName);
        Camera.main.backgroundColor = new Color(0.5803922f, 0.8470589f, 0.937255f, 0);
        objName = "Motorcycle";
        CarController vehicle = objectManager.GetObject(objName).GetComponent<CarController>();
        carController = vehicle;

        cameraController.vehiclePos = vehicle.gameObject.transform;
        cameraController.SetUp();

        totalMoney = PlayerPrefs.GetInt("Money");
        moneyText.text = totalMoney.ToString();
    }

    public void FuelConsume() {
        fuelGauge.fillAmount = carController.Fuel;
        if(fuelGauge.fillAmount <= 0.6f) {
            fuelGauge.color = new Color(1, fuelGauge.fillAmount * 0.8f * 2f, 0, 1);
            
            if(fuelGauge.fillAmount <= 0.3f) {
                if(!isDie) fuelWarning.SetActive(true);
                if(fuelGauge.fillAmount == 0f)
                    StartGameOver();
            }
        }
        else {
            fuelGauge.color = new Color((1f - fuelGauge.fillAmount) * 2f, 1, 0, 1);  
            fuelWarning.SetActive(false);
        }
    }

    public void FuelCharge() {
        carController.Fuel = 1;
        fuelGauge.fillAmount = 1;
        PlaySound("refuel");
    }

    public void GetCoin(int price) {
        totalMoney += price;
        moneyEarned += price;
        moneyText.text = totalMoney.ToString();
        moneyText.GetComponent<Animator>().SetTrigger("EarnMoney");
        PlaySound("coin");
    }

    public void GasBtn(bool press) {
        GasBtnPressed = press;
    }

    public void BrakeBtn(bool press) {
        BrakeBtnPressed = press;
    }

    public void PlaySound(string audioName) {
        switch(audioName) {
            case "cameraShutter" :
                audio[0].Play();
                break;
            case "coin":
                audio[1].Play();
                break;
            case "crack":
                audio[2].Play();
                break;
            case "refuel":
                audio[3].Play();
                break;
            case "engine":
                audio[4].Play();
                break;
        }
    }
    public void GamePause() {
        pauseUI.SetActive(!pauseUI.activeSelf);
        
        if(pauseUI.activeSelf) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public void StartGameOver() {
        if(!isDie) {
            StartCoroutine(GameOver());
            isDie = true;
        }
    }

    private IEnumerator GameOver() {
        if(!ReachGoal) yield return new WaitForSeconds(4f);

        carController.moveStop = true;
        fuelWarning.SetActive(false);

        yield return new WaitForEndOfFrame();
        Texture2D text = new Texture2D(Screen.width / 5, Screen.height / 3, TextureFormat.RGB24, false);
        textureImg = new Texture2D(Screen.width / 5, Screen.height / 3);
        text.ReadPixels(new Rect(-Screen.width / 2, Screen.height / 3 + 15f, Screen.width, Screen.height), 0, 0);
        text.Apply();
        textureImg = text;
        spriteImg = Sprite.Create(textureImg, new Rect(0, 0, textureImg.width, textureImg.height), new Vector2(0, 0));
        captureImg.sprite = spriteImg;

        if(!ReachGoal) gameStateText.text = "<color=#FF4C4C>Game Over</color>";
        else gameStateText.text = "<color=#FFFF4C>Game Complete!!</color>";
        moneyEarnedText.text = "+" + moneyEarned.ToString() + " COINS";  //게임 동안 얻은 코인 수를 보여줌
        totaldistanceText.text = " Distance : " + (int)(carController.transform.position.x - carController.StartPos.x) + "m";
        gameOverUI.SetActive(true);
        
        PlaySound("cameraShutter");
    }

    public void LoadScene(int sceneIndex) {
        PlayerPrefs.SetInt("Money", totalMoney);
        SceneManager.LoadScene(sceneIndex); 
    }
}