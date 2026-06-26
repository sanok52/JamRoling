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
    [SerializeField] private Slider slider;

    [Space]
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject reloadButton;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject exitButton;

    private int click;
    private bool isMenu = false;
    public static float Snsitivity { get; private set; }

    public bool IsMenu => isMenu;

    private void Start()
    {
        slider.onValueChanged.AddListener((value) => ChangeSnsitivity(value));
        slider.value = 0.5f;
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

        yield return canvasGroup.DOFade(1f, 1f).SetUpdate(true).WaitForCompletion();
    }

    private IEnumerator ClosePauseRoutine()
    {
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

    private void ChangeSnsitivity(float value)
    {
        Snsitivity = value * 2f;
    }

    public void Quit()
    {
        Application.Quit();
    }

    private bool isPause;
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