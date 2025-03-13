using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private GameObject _notchPoint;
    [SerializeField] private float _spawnDelay = 1f;

    private XRGrabInteractable _bow;
    private XRPullInteractible _pullInteractable;
    private bool _arrowNotched = false;
    private GameObject _currentArrow = null;

    private void Start()
    {
        _bow = GetComponent<XRGrabInteractable>();
        _pullInteractable = GetComponentInChildren<XRPullInteractible>();

        if (_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased += NotchEmpty;
        }
    }

    private void OnDestroy()
    {
        if (_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased -= NotchEmpty;
        }
    }

    private void Update()
    {
        if (_bow.isSelected && !_arrowNotched)
        {
            _arrowNotched = true;
            StartCoroutine(DelayedSpawn());
        }

        if (!_bow.isSelected && _currentArrow != null)
        {
            Destroy(_currentArrow);
            NotchEmpty(1f);
        }
    }

    private void NotchEmpty(float value)
    {
        _arrowNotched = false;
        _currentArrow = null;
    }

    private IEnumerator DelayedSpawn()
    {
        Debug.Log("DelayedSpawn started!"); // Debug
        yield return new WaitForSeconds(_spawnDelay);

        Debug.Log("Instantiating arrow..."); // Debug
        _currentArrow = Instantiate(_arrowPrefab, _notchPoint.transform.position, _notchPoint.transform.rotation);
        _currentArrow.transform.SetParent(_notchPoint.transform);

        if (_currentArrow == null)
        {
            Debug.LogError("Arrow instantiation failed!");
            yield break;
        }

        ArrowLauncher launcher = _currentArrow.GetComponent<ArrowLauncher>();
        if (launcher != null && _pullInteractable != null)
        {
            launcher.Initialize(_pullInteractable);
        }
        else
        {
            Debug.LogWarning("Launcher or PullInteractable is null!");
        }
    }

}