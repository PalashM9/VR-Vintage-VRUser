using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRGrabFullDebug : MonoBehaviour
{
    [Header("Opt")]
    public InputActionReference rightSelectAction;   

    public InputActionReference leftSelectAction;    

    public Key keyboardTestKey = Key.G;              

    [Header("Logging")]
    public bool logEveryFrameWhileHovered = false;
    public bool logEveryFrameActionValues = false;

    private XRGrabInteractable grab;
    private XRInteractionManager manager;

    private bool isHovered;
    private IXRInteractor lastHoverInteractor;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (!grab)
        {
            Debug.LogError($"[XRGrabFullDebug] No XRGrabInteractable found on {name}.", this);
            enabled = false;
            return;
        }

        manager = grab.interactionManager;
        if (!manager)
        {

            manager = FindFirstObjectByType<XRInteractionManager>();
        }

        grab.hoverEntered.AddListener(OnHoverEntered);
        grab.hoverExited.AddListener(OnHoverExited);
        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
        grab.activated.AddListener(OnActivated);
        grab.deactivated.AddListener(OnDeactivated);

        DumpInteractableConfig("AWAKE");
        DumpActionSetup("AWAKE");

        SafeEnable(rightSelectAction, "RightSelect");
        SafeEnable(leftSelectAction, "LeftSelect");
    }

    void OnEnable()
    {
        DumpInteractableConfig("ON_ENABLE");
        DumpActionSetup("ON_ENABLE");
    }

    void Update()
    {

        if (Keyboard.current != null)
        {
            var keyCtrl = Keyboard.current[keyboardTestKey];
            if (keyCtrl != null && keyCtrl.wasPressedThisFrame)
                Debug.Log($"[XRGrabFullDebug] KEY PRESSED: {keyboardTestKey} (frame {Time.frameCount})", this);

            if (keyCtrl != null && keyCtrl.wasReleasedThisFrame)
                Debug.Log($"[XRGrabFullDebug] KEY RELEASED: {keyboardTestKey} (frame {Time.frameCount})", this);
        }
        else
        {

            Debug.LogWarning("[XRGrabFullDebug] Keyboard.current is null. Input System may not be active.", this);
        }

        if (logEveryFrameActionValues)
            DumpActionValues("UPDATE");

        if (logEveryFrameWhileHovered && isHovered)
            DumpHoverState("UPDATE_WHILE_HOVERED");
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovered = true;
        lastHoverInteractor = args.interactorObject;

        Debug.Log(
            $"[XRGrabFullDebug] HOVER ENTER | Interactable={Fmt(grab)} | Interactor={Fmt(args.interactorObject)} | " +
            $"Manager={(manager ? manager.name : "NULL")} | frame={Time.frameCount}",
            this);

        DumpInteractorDetails(args.interactorObject, "HOVER_ENTER_INTERACTOR");
        DumpInteractableConfig("HOVER_ENTER_INTERACTABLE");
        DumpActionValues("HOVER_ENTER_ACTIONS");
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovered = false;

        Debug.Log(
            $"[XRGrabFullDebug] HOVER EXIT  | Interactable={Fmt(grab)} | Interactor={Fmt(args.interactorObject)} | frame={Time.frameCount}",
            this);

        DumpActionValues("HOVER_EXIT_ACTIONS");
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log(
            $"[XRGrabFullDebug] SELECT ENTER | Interactable={Fmt(grab)} | Interactor={Fmt(args.interactorObject)} | " +
            $"isSelected={grab.isSelected} | frame={Time.frameCount}",
            this);

        DumpInteractorDetails(args.interactorObject, "SELECT_ENTER_INTERACTOR");
        DumpActionValues("SELECT_ENTER_ACTIONS");
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log(
            $"[XRGrabFullDebug] SELECT EXIT  | Interactable={Fmt(grab)} | Interactor={Fmt(args.interactorObject)} | frame={Time.frameCount}",
            this);

        DumpActionValues("SELECT_EXIT_ACTIONS");
    }

    private void OnActivated(ActivateEventArgs args)
    {
        Debug.Log($"[XRGrabFullDebug] ACTIVATE | Interactable={Fmt(grab)} | Interactor={Fmt(args.interactorObject)}", this);
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        Debug.Log($"[XRGrabFullDebug] DEACTIVATE | Interactable={Fmt(grab)} | Interactor={Fmt(args.interactorObject)}", this);
    }

    private void DumpInteractableConfig(string tag)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[XRGrabFullDebug] --- {tag} INTERACTABLE CONFIG ---");
        sb.AppendLine($"Object: {name} (activeInHierarchy={gameObject.activeInHierarchy})");
        sb.AppendLine($"Grab: enabled={grab.enabled}, isHovered={grab.isHovered}, isSelected={grab.isSelected}");
        sb.AppendLine($"InteractionManager: {(grab.interactionManager ? grab.interactionManager.name : "NULL")} (fallback={(manager ? manager.name : "NULL")})");
        sb.AppendLine($"InteractionLayerMask: {grab.interactionLayers.value}");
        sb.AppendLine($"CollidersCount: {grab.colliders.Count}");
        for (int i = 0; i < grab.colliders.Count; i++)
        {
            var c = grab.colliders[i];
            sb.AppendLine($"  [{i}] {c?.GetType().Name} name={c?.name} enabled={c?.enabled} isTrigger={(c is Collider col ? col.isTrigger : false)}");
        }

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            sb.AppendLine($"Rigidbody: isKinematic={rb.isKinematic}, useGravity={rb.useGravity}, constraints={rb.constraints}, collisionDetection={rb.collisionDetectionMode}");
        }
        else
        {
            sb.AppendLine("Rigidbody: NULL");
        }

        Debug.Log(sb.ToString(), this);
    }

    private void DumpInteractorDetails(IXRInteractor interactor, string tag)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[XRGrabFullDebug] --- {tag} ---");
        if (interactor == null)
        {
            sb.AppendLine("Interactor: NULL");
            Debug.Log(sb.ToString(), this);
            return;
        }

        sb.AppendLine($"Interactor: {Fmt(interactor)}");

        if (interactor is XRBaseInteractor baseInteractor)
        {
            sb.AppendLine($"Type: {baseInteractor.GetType().Name}");
            sb.AppendLine($"enabled={baseInteractor.enabled}, allowHover={baseInteractor.allowHover}, allowSelect={baseInteractor.allowSelect}");
            sb.AppendLine($"InteractionManager={(baseInteractor.interactionManager ? baseInteractor.interactionManager.name : "NULL")}");
            sb.AppendLine($"InteractionLayerMask={baseInteractor.interactionLayers.value}");
            sb.AppendLine($"AttachTransform={(baseInteractor.attachTransform ? baseInteractor.attachTransform.name : "NULL")}");

            if (baseInteractor is XRDirectInteractor direct)
            {
                var sc = direct.GetComponent<SphereCollider>();
                sb.AppendLine($"DirectInteractor SphereCollider: {(sc ? $"radius={sc.radius}, isTrigger={sc.isTrigger}, enabled={sc.enabled}" : "NULL")}");
            }

            if (baseInteractor is XRRayInteractor ray)
            {
                sb.AppendLine($"RayInteractor: maxDistance={ray.maxRaycastDistance}, rayMask={ray.raycastMask.value}");
            }
        }
        else
        {
            sb.AppendLine($"Type: {interactor.GetType().Name} (not XRBaseInteractor)");
        }

        Debug.Log(sb.ToString(), this);
    }

    private void DumpActionSetup(string tag)
    {
        Debug.Log($"[XRGrabFullDebug] --- {tag} ACTION SETUP ---\n" +
                  $"RightSelectAction={(rightSelectAction ? rightSelectAction.action.name : "NULL")} enabled={(rightSelectAction ? rightSelectAction.action.enabled : false)}\n" +
                  $"LeftSelectAction={(leftSelectAction ? leftSelectAction.action.name : "NULL")} enabled={(leftSelectAction ? leftSelectAction.action.enabled : false)}",
                  this);
    }

    private void DumpActionValues(string tag)
    {
        string rs = rightSelectAction ? $"{rightSelectAction.action.ReadValue<float>():0.###} (enabled={rightSelectAction.action.enabled})" : "NULL";
        string ls = leftSelectAction ? $"{leftSelectAction.action.ReadValue<float>():0.###} (enabled={leftSelectAction.action.enabled})" : "NULL";
        Debug.Log($"[XRGrabFullDebug] {tag} ACTION VALUES | RightSelect={rs} | LeftSelect={ls} | frame={Time.frameCount}", this);
    }

    private void DumpHoverState(string tag)
    {
        Debug.Log($"[XRGrabFullDebug] {tag} | isHovered={isHovered} | lastInteractor={Fmt(lastHoverInteractor)} | frame={Time.frameCount}", this);
    }

    private static void SafeEnable(InputActionReference a, string label)
    {
        if (a == null) return;
        try
        {
            if (!a.action.enabled)
                a.action.Enable();
        }
        catch (System.SystemException e)
        {
            Debug.LogWarning($"[XRGrabFullDebug] Could not Enable action '{label}': {e.Message}");
        }
    }

    private static string Fmt(object o)
    {
        if (o == null) return "NULL";
        if (o is Component c) return $"{c.name}";
        return o.ToString();
    }
}

