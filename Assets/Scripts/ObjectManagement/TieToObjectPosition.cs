using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TieToObjectPosition : MonoBehaviour
{
    [SerializeField, Tooltip("Name in the editor for easier identification.")]
    private string editorName = "DefaultName";
    
    [SerializeField, Tooltip("Orientations for attaching objects.")]
    private Quaternion[] attachmentOrientations = new Quaternion[3];
    [SerializeField, Tooltip("Cycle through orientations or always use the first one.")]
    private bool cycleOrientations = true;
    [SerializeField, Tooltip("List of tool names that are accepted for attachment.")]
    private List<string> acceptableTools = new List<string>();

    [SerializeField, Tooltip("Toggle between accepting any tool or only those listed.")]
    private bool acceptOnlyListedTools = true;
    
    public string publicEditorName => editorName;
    
    private int _currentOrientationIndex = 0;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    
    private XRBaseInteractable _tiedObject;  
    private Transform _tiePosition; 
    [SerializeField, Tooltip("Time in seconds before the object can be retied")]
    private float regrabDelay = 2.0f;       // Time in seconds before the object can be retied

    public bool isTied { get; private set; }
    private bool _isLocked = false;
    private bool _isDelayActive = false;

    private void Awake()
    {
        isTied = false;
        InitializeOrientations();
    }


    void Update()
    {
        if (isTied && _tiedObject != null)
        {
            var transform1 = _tiedObject.transform;
            transform1.position = _tiePosition.position;
            transform1.rotation = _tiePosition.rotation;
        }
    }
    private void InitializeOrientations()
    {
        attachmentOrientations[0] = Quaternion.Euler(0, 45, 0);   // Orientation 1
        attachmentOrientations[1] = Quaternion.Euler(0, 0, 0);    // Orientation 2 (Correct)
        attachmentOrientations[2] = Quaternion.Euler(0, -45, 0);  // Orientation 3
    }
    private Quaternion GetCurrentOrientation()
    {
        return attachmentOrientations[_currentOrientationIndex];
    }
    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Tool") && !isTied && !_isDelayActive) &&
            (!acceptOnlyListedTools || acceptableTools.Contains(other.gameObject.name)))
        {
            Debug.Log("Tool detected: "+other.gameObject.name);
            XRBaseInteractable interactable = other.GetComponent<XRBaseInteractable>();
            if (interactable != null)
            {
                _tiedObject = interactable;
                var transform1 = _tiedObject.transform;
                _originalPosition = transform1.position;
                _originalRotation = transform1.rotation;

                // Subscribe to the select events
                _tiedObject.selectEntered.AddListener(HandleSelectEntered);
                _tiedObject.selectExited.AddListener(HandleSelectExited);
                
                var transform2 = transform;
                _tiedObject.transform.position = _tiePosition.position;
                _tiedObject.transform.rotation = _tiePosition.rotation * GetCurrentOrientation();
                isTied = true;
            }
        }
    }
    
    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (isTied && args.interactor is XRDirectInteractor && !_isLocked)  // Check the isLocked flag
        {
            Debug.Log("Object was grabbed by an XR Direct Interactor.");

            isTied = false;
            if (cycleOrientations)
            {
                _currentOrientationIndex = (_currentOrientationIndex + 1) % attachmentOrientations.Length;
            }
            else
            {
                _currentOrientationIndex = 1;  // Always reset to the correct orientation
            }
            // Unsubscribe to prevent memory leaks
            _tiedObject.selectEntered.RemoveListener(HandleSelectEntered);
            _tiedObject.selectExited.RemoveListener(HandleSelectExited);

            _tiedObject = null;
            // Start the re-grab delay 
            StartCoroutine(RegrabDelayCoroutine());
        }
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        if (args.interactor is XRDirectInteractor)
            Debug.Log("Object was released by an XR Direct Interactor.");
    }

    private IEnumerator RegrabDelayCoroutine()
    {
        _isDelayActive = true;
        yield return new WaitForSeconds(regrabDelay);
        _isDelayActive = false;
    }
    
    public bool IsObjectCorrectlyPositioned()
    {
        if (_tiedObject == null || !isTied)
        {
            return false;
        }

        // Return true only if the current orientation is the correct one (index 1)
        return _currentOrientationIndex == 1;
    }
    public void LockObject()
    {
        _isLocked = true;
    }

    public void UnlockObject()
    {
        _isLocked = false;
    }
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (_tiedObject != null)
        {
            _tiedObject.selectEntered.RemoveListener(HandleSelectEntered);
            _tiedObject.selectExited.RemoveListener(HandleSelectExited);
        }
    }
}