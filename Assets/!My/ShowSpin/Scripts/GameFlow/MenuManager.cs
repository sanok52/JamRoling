using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroupGame;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Camera cameraMenu;
    [SerializeField] private Slider sliderSpin;
    [SerializeField] private Slider sliderLook;

    [Space]
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject reloadButton;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject exitButton;

    private int click;
    private bool isMenu = false;
    private bool isPause;

    public bool IsMenu => isMenu;
    public bool IsPause => isPause; 

    private void Start()
    {
        sliderSpin.SetValueWithoutNotify(SettingsManager.SensitivitySpin);
        sliderSpin.onValueChanged.AddListener((value) => SettingsManager.SensitivitySpin = value);
        sliderLook.SetValueWithoutNotify(SettingsManager.SensitivityLook);
        sliderLook.onValueChanged.AddListener((value) => SettingsManager.SensitivityLook = value);
        //slider.value = 0.5f;
    }

    public IEnumerator WaitPlayerAction()
    {
        yield return new WaitForEndOfFrame();

        SetMenuState(true);
        click = 0;
        isMenu = true;
        OpenPause();

        yield return OpenPauseRoutine();
        yield return new WaitWhile(() => click == 0);

        isMenu = false;
        SetMenuState(false);
        ClosePause();
    }

    private void ClosePause()
    {
        isPause = false;
        cameraMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        StartCoroutine(ClosePauseRoutine());

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OpenPause()
    {
        isPause = true;
        cameraMenu.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        StartCoroutine(OpenPauseRoutine());

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator OpenPauseRoutine()
    {
        canvasGroupGame.DOFade(0f, 0.5f).SetUpdate(true);
        G.spinGamePlay.gameObject.SetActive(false);
        if(!isMenu)
            G.MusicManager.SetPitch(0.25f);
        else
            G.MusicManager.SetVolume(0.2f);

        yield return canvasGroup.DOFade(1f, 1f).SetUpdate(true).WaitForCompletion();
    }

    private IEnumerator ClosePauseRoutine()
    {
        G.MusicManager.SetPitch(1f);
        G.MusicManager.SetVolume(0.25f);
        yield return canvasGroup.DOFade(0f, 1f).SetUpdate(true).WaitForCompletion();

        canvasGroupGame.DOFade(1f, 0.5f).SetUpdate(true);
        G.spinGamePlay.gameObject.SetActive(true);
    }

    public void SetMenuState(bool isMenu)
    {
#if UNITY_STANDALONE_WIN
        exitButton.SetActive(true);
#else
        exitButton.SetActive(false);
#endif
        this.isMenu = isMenu;
        startGameButton.SetActive(isMenu);
        returnButton.SetActive(!isMenu);
        reloadButton.SetActive(!isMenu);
        menuButton.SetActive(!isMenu);

        if (isMenu)
        {
            G.MusicManager.PlayMusic("Menu");
        }
    }

    public void StartGameClick()
    {
        click = 1;
    }

    public void ReturnClick()
    {
        ClosePause();
    }

    public void ReloadClick()
    {
        G.SpinGameFlow.Reload(true);
    }

    public void MenuClick()
    {
        G.SpinGameFlow.Reload(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SwitchPause()
    {
        if (isMenu)
            return;

        StopAllCoroutines();
        if (isPause)
            ClosePause();
        else
            OpenPause();
    }
}

public static class SettingsManager
{
    public static float SensitivityLook { get => sensitivityLook <= 0 ? 0.5f : sensitivityLook; set => sensitivityLook = value; }
    public static float SensitivitySpin { get => sensitivitySpin <= 0 ? 0.5f : sensitivitySpin; set => sensitivitySpin = value; }

    private static float sensitivityLook = -1f;
    private static float sensitivitySpin = -1f;

    private static Vector2 coefLookA = new Vector2(1f, 5f);
    private static Vector2 coefLookB = new Vector2(0.1f, 1f);
    private static Vector2 coefSpinA = new Vector2(1.1f, 3f);
    private static Vector2 coefSpinB = new Vector2(0.1f, 1.1f);

    private static float coefSpin => SensitivitySpin >= 0.5f ? Mathf.Lerp(coefSpinA.x, coefSpinA.y, (SensitivityLook - 0.5f) * 2f) :
        Mathf.Lerp(coefSpinB.x, coefSpinB.y, SensitivityLook * 2f);
    private static float coefLook => SensitivityLook >= 0.5f ? Mathf.Lerp(coefLookA.x, coefLookA.y, (SensitivityLook - 0.5f) * 2f) :
        Mathf.Lerp(coefLookB.x, coefLookB.y, SensitivityLook * 2f);

    public static float MouseXLook => Input.GetAxis("Mouse X") * SensitivityLook * coefLook;
    public static float MouseYLook => Input.GetAxis("Mouse Y") * SensitivityLook * coefLook * -1;

    public static float MouseXSpin => Input.GetAxis("Mouse X") * SensitivitySpin * coefSpin;
    public static float MouseYSpin => Input.GetAxis("Mouse Y") * SensitivitySpin * coefSpin;
}