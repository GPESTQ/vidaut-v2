using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BottomManager : MonoBehaviour
{
    private Gamemaneger gm;

    public Button btnFinalizar;
    public Button btnIniciar;
    public Button btnContinuar;

    public string CenaInicial;
    public string CenaJogo;

    private bool _deveIniciarNovo = false;
    private bool _deveContinuar = false;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene cena, LoadSceneMode modo)
    {
        ConectarBotoes();

        if (cena.name == CenaJogo)
        {
            gm = FindObjectOfType<Gamemaneger>();

            if (gm != null)
            {
                if (_deveIniciarNovo)
                {
                    gm.NovoJogo();
                    _deveIniciarNovo = false;
                }
                else if (_deveContinuar)
                {
                    gm.RestaurarSnapshot();
                    _deveContinuar = false;
                }
            }
        }
    }

    void ConectarBotoes()
    {
        if (btnFinalizar != null)
        {
            btnFinalizar.onClick.RemoveAllListeners();
            btnFinalizar.onClick.AddListener(CloseGame);
        }

        if (btnIniciar != null)
        {
            btnIniciar.onClick.RemoveAllListeners();
            btnIniciar.onClick.AddListener(IniciarNovoJogo);
        }

        if (btnContinuar != null)
        {
            btnContinuar.onClick.RemoveAllListeners();
            btnContinuar.onClick.AddListener(ContinuarJogo);
        }
    }

    void IniciarNovoJogo()
    {
        _deveIniciarNovo = true;
        _deveContinuar = false;
        SceneManager.LoadScene(CenaJogo);
    }

    void ContinuarJogo()
    {
        _deveContinuar = true;
        _deveIniciarNovo = false;
        SceneManager.LoadScene(CenaJogo);
    }

    void CloseGame()
    {
        SceneManager.LoadScene(CenaInicial);
    }
}
