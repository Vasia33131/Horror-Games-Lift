using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Луч из камеры: панели у <see cref="PanelRevealInteractable"/> при наведении.
/// RaycastNonAlloc — без аллокации массива каждый кадр (меньше микрофризов от GC).
/// </summary>
public class Interactor : MonoBehaviour
{
    [Header("Raycast")]
    public Transform InteractorSource;
    public float InteractRange = 3f;
    [SerializeField] private LayerMask raycastLayers = ~0;
    [SerializeField] private int hitBufferSize = 32;

    [Header("Игрок (опционально)")]
    [SerializeField] private Player player;
    [Tooltip("Не считать попадания в коллайдеры игрока.")]
    [SerializeField] private bool skipOwnColliders = true;

    [Header("Ввод")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private PanelRevealInteractable _hoveredPanel;
    private RaycastHit[] _hitBuffer;
    private static readonly DistanceComparer HitDistanceComparer = new DistanceComparer();

    private sealed class DistanceComparer : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
    }

    private void Awake()
    {
        _hitBuffer = new RaycastHit[Mathf.Max(8, hitBufferSize)];

        if (player == null)
            player = GetComponentInParent<Player>();

        if (InteractorSource == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam == null)
                cam = Camera.main;
            if (cam != null)
                InteractorSource = cam.transform;
        }
    }

    private void LateUpdate()
    {
        if (InteractorSource == null)
            return;

        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
        PanelRevealInteractable found = FindPanelRevealAlongRay(ray);

        if (found != _hoveredPanel)
        {
            _hoveredPanel?.SetHover(false);
            _hoveredPanel = found;
            _hoveredPanel?.SetHover(true);
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(interactKey))
            return;

        if (InteractorSource == null)
            return;

        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        if (!TryGetInteractableAlongRay(ray, out IInteractable interactable))
            return;

        interactable.Interact();
    }

    private int RaycastNonAllocSorted(Ray ray)
    {
        int count = Physics.RaycastNonAlloc(
            ray,
            _hitBuffer,
            InteractRange,
            raycastLayers,
            QueryTriggerInteraction.Collide);

        if (count > _hitBuffer.Length)
        {
            _hitBuffer = new RaycastHit[count + 8];
            count = Physics.RaycastNonAlloc(ray, _hitBuffer, InteractRange, raycastLayers, QueryTriggerInteraction.Collide);
        }

        if (count > 1)
            Array.Sort(_hitBuffer, 0, count, HitDistanceComparer);

        return count;
    }

    private bool ShouldSkipHit(RaycastHit hit)
    {
        if (!skipOwnColliders || player == null)
            return false;

        Transform t = hit.collider.transform;
        Transform root = player.transform;
        return t == root || t.IsChildOf(root);
    }

    private PanelRevealInteractable FindPanelRevealAlongRay(Ray ray)
    {
        int count = RaycastNonAllocSorted(ray);
        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = _hitBuffer[i];
            if (ShouldSkipHit(hit))
                continue;

            PanelRevealInteractable pr = hit.collider.GetComponentInParent<PanelRevealInteractable>();
            if (pr != null)
                return pr;
        }

        return null;
    }

    private bool TryGetInteractableAlongRay(Ray ray, out IInteractable interactable)
    {
        interactable = null;
        int count = RaycastNonAllocSorted(ray);

        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = _hitBuffer[i];
            if (ShouldSkipHit(hit))
                continue;

            foreach (MonoBehaviour mb in hit.collider.GetComponentsInParent<MonoBehaviour>(true))
            {
                if (mb is IInteractable candidate)
                {
                    interactable = candidate;
                    return true;
                }
            }
        }

        return false;
    }
}
