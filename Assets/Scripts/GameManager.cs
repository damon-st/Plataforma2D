using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject vidasUI;
    public PlayerController player;
    public Text txtCoins;
    public int coins;
    public Text savedDepartureText;

    private bool loadingLevel;
    private int indiceNivelJugador;

    [Header("Panels")]
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelLoad;
    [SerializeField] private GameObject panelTransition;

    public CinemachineConfiner2D cinemachineConfiner;

    private const string KEY_COIN = "monedas";
    private const string KEY_COORDINES_X = "cordenadax";
    private const string KEY_COORDINES_Y = "cordenaday";
    private const string KEY_LIVES = "lives";
    private const string KEY_LEVEL = "leves";
    private const string KEY_INDICE_NIVEL_INICIO = "indiceNivel";

    private bool executeCorrutine = false;

    public bool avanandoNivel;
    public int nivelActual;
    public List<Transform> positionsAvance = new();
    public List<Transform> positionsRetroceder = new();
    public List<Collider2D> areasCamera = new();

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

    /**    if (PlayerPrefs.GetInt(KEY_LIVES) != 0)
        {
            LoadGame();
        }**/
           
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Nivel1")
        {
            nivelActual = PlayerPrefs.GetInt(KEY_INDICE_NIVEL_INICIO);
            indiceNivelJugador = nivelActual;
            PositionInicialJugador(indiceNivelJugador);
            cinemachineConfiner.m_BoundingShape2D = areasCamera[indiceNivelJugador];
        }
        else if(SceneManager.GetActiveScene().name == "LevelSelect")
        {
            PositionInicialJugador(0);
        }
    }

    public void ActivePanelTransition()
    {
        panelTransition.GetComponent<Animator>().SetTrigger("ocultar");
    }


    private void PositionInicialJugador(int indiceNivelIncio)
    {
        player.transform.position = positionsAvance[indiceNivelIncio].transform.position;
    }

    public void SetIndiceNivelInicio(int indiceNivelJugador)
    {
        this.indiceNivelJugador = indiceNivelJugador;
        PlayerPrefs.SetInt(KEY_INDICE_NIVEL_INICIO,indiceNivelJugador);
    }


    public void ChangePositionPlayer()
    {
        if (avanandoNivel)
        {
            if(nivelActual + 1 < positionsAvance.Count ) {
                player.transform.position = positionsAvance[nivelActual + 1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.ChangeAnimWalking(false);
                player.finishMap = false;
                cinemachineConfiner.m_BoundingShape2D = areasCamera[nivelActual + 1];
            }
        }
        else
        {
            if(positionsRetroceder.Count > nivelActual - 1)
            {
                player.transform.position = positionsAvance[nivelActual - 1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.ChangeAnimWalking(false);
                player.finishMap = false;
                cinemachineConfiner.m_BoundingShape2D = areasCamera[nivelActual - 1];
            }
        }
    }

    public void SaveGame()
    {
        float x, y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int lives = player.lives;

        PlayerPrefs.SetInt(KEY_COIN, coins);
        PlayerPrefs.SetFloat(KEY_COORDINES_X, x);
        PlayerPrefs.SetFloat(KEY_COORDINES_Y, y);
        PlayerPrefs.SetInt(KEY_LIVES, lives);
        PlayerPrefs.SetInt(KEY_LEVEL, nivelActual);
        PlayerPrefs.SetInt(KEY_INDICE_NIVEL_INICIO,indiceNivelJugador);
        if (!executeCorrutine)
        {
            StartCoroutine(ShowTextSaved());
        }
    }

    private IEnumerator ShowTextSaved()
    {
        executeCorrutine = true;
        savedDepartureText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        savedDepartureText.gameObject.SetActive(false);
        executeCorrutine = false;
    }

    public void LoadGame()
    {
       /* if (PlayerPrefs.GetString(KEY_SCENE) == string.Empty)
        {
            SceneManager.LoadScene("LevelSelect");

        }
        else
        {
            SceneManager.LoadScene(PlayerPrefs.GetString(KEY_SCENE));
        }*/
        coins = PlayerPrefs.GetInt(KEY_COIN);
        player.transform.position = new Vector2(PlayerPrefs.GetFloat(KEY_COORDINES_X),PlayerPrefs.GetFloat(KEY_COORDINES_Y));
        player.lives = PlayerPrefs.GetInt(KEY_LIVES);
        txtCoins.text = coins.ToString();
        nivelActual = PlayerPrefs.GetInt(KEY_LEVEL);
        indiceNivelJugador = PlayerPrefs.GetInt(KEY_INDICE_NIVEL_INICIO);
        cinemachineConfiner.m_BoundingShape2D = areasCamera[nivelActual];

        int livesDiscount = 3 - player.lives;
        player.ShowLivesUI();
        player.UpdateLivesUI(livesDiscount);
    }

    public void LoadLevel(string nameLevel)
    {
        SceneManager.LoadScene(nameLevel);

    }

    public  void UpdateCountCoins()
    {
        coins++;
        txtCoins.text = coins.ToString();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        panelPause.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        panelPause.SetActive(false);
    }

    public void OnBackMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    
    public void LoadSelector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadScene(int index)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    public void LoadScene(string sceneLoad) 
    {
        StartCoroutine(LoadSceneI(sceneLoad));
    }

    private IEnumerator LoadSceneI(string index)
    {
        loadingLevel = true;
        panelLoad.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
       // PositionInicialJugador(indiceNivelJugador);
        loadingLevel = false;

    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);
       
    }

    public void ContineGame()
    {
        if (PlayerPrefs.GetFloat(KEY_COORDINES_X) != 0.0f)
        {
            player.enabled = true;
            LoadGame();
            panelGameOver.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadSceneAsync(int index)
    {
        StartCoroutine(LoadSceneSelector(index));
    }

    private IEnumerator LoadSceneSelector(int index)
    {
        loadingLevel=true;
        panelLoad.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        loadingLevel = false;

    }
}
