using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class Scanner : XRGrabInteractable
{
    [Header("Scanner Data")]
    public Animator animator;
    public LineRenderer laserRenderer;

    public TextMeshProUGUI targetName;
    public TextMeshProUGUI targetPosition;

    [Header("Audio Files")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip laserSound;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        ScannerActivated(false);

        targetName.SetText("Ready for Scanning!");
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (laserRenderer.gameObject.activeSelf)
            ScanForObjects();
    }

    private void ScannerActivated(bool isActivated)
    {
        laserRenderer.gameObject.SetActive(isActivated);
        targetPosition.gameObject.SetActive(isActivated);
    }

    private void ScanForObjects()
    {
        RaycastHit hit;

        Vector3 worldHit = laserRenderer.transform.position + laserRenderer.transform.forward * 1000.0f;

        if (Physics.Raycast(laserRenderer.transform.position, laserRenderer.transform.forward, out hit))
        {
            worldHit = hit.point;

            targetName.SetText(hit.collider.name);
            // targetPosition.SetText(hit.collider.transform.position.ToString());

            targetPosition.SetText(Vector3.Distance(worldHit, transform.position).ToString("F2") + "m");
        }

        laserRenderer.SetPosition(1, laserRenderer.transform.InverseTransformPoint(worldHit));
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args); // add this line
        animator.SetBool("Opened", true);
        audioSource.PlayOneShot(openSound);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args); // add this line 
        animator.SetBool("Opened", false);
        audioSource.PlayOneShot(closeSound);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        audioSource.loop = true;
        audioSource.clip = laserSound;
        audioSource.Play();
        ScannerActivated(true);
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        audioSource.loop = false;
        audioSource.Stop();
        ScannerActivated(false);

        targetName.SetText("Ready for Scanning!");
    }
}
