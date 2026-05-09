using UnityEngine;

/// <summary>
/// Панель показывается, пока прицел на объекте (управляет <see cref="Interactor"/>).
/// Прогрев Canvas при старте убирает лаг при первом наведении (перестроение UI в первый раз дорогое).
/// </summary>
public class PanelRevealInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject panel;

    [Tooltip("В начале сцены скрыть панель.")]
    [SerializeField] private bool hidePanelOnStart = true;

    [Tooltip("Один раз при старте «проиграть» включение панели и Canvas — убирает фриз при первом наведении.")]
    [SerializeField] private bool warmupCanvasOnStart = true;

    private bool _hover;

    private void Start()
    {
        if (panel == null)
            return;

        if (warmupCanvasOnStart)
            WarmUpPanelCanvas();

        if (hidePanelOnStart)
            panel.SetActive(false);
    }

    /// <summary>
    /// Включает панель на кадр: Unity создаёт батчи/лейаут не во время геймплея при первом показе.
    /// </summary>
    private void WarmUpPanelCanvas()
    {
        bool wasActive = panel.activeSelf;
        panel.SetActive(true);
        Canvas.ForceUpdateCanvases();
        panel.SetActive(false);
        if (wasActive)
            panel.SetActive(true);
    }

    /// <summary>
    /// Вызывается из Interactor каждый кадр: true — игрок смотрит на объект, false — отвернулся.
    /// </summary>
    public void SetHover(bool hovering)
    {
        _hover = hovering;
        if (panel != null)
            panel.SetActive(hovering);
    }

    /// <summary>
    /// Дополнительное действие по E (не обязательно для показа панели).
    /// </summary>
    public virtual void Interact()
    {
    }
}
