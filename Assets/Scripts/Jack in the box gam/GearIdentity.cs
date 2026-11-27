using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
public class GearIdentity : MonoBehaviour
{
    [SerializeField] private PlacingGear.GearPlace gearId;
    public PlacingGear.GearPlace GearId => gearId;

    private XRGrabInteractable grab;
    private Rigidbody rb;

    private readonly List<PlacingGear> socketsInRange = new List<PlacingGear>();
    private PlacingGear placedSocket;
    private Coroutine settleRoutine;

    public Transform HomeParent { get; private set; } // where the gear lives when it's not in a socket

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        HomeParent = transform.parent;
        grab.retainTransformParent = false; // we handle parenting ourselves
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }

    // Called by the sockets
    public void AddSocketInRange(PlacingGear socket)
    {
        if (!socketsInRange.Contains(socket))
            socketsInRange.Add(socket);

        if (grab.isSelected)
            socket.ShowHover(); // hint only while the gear is held
    }

    public void RemoveSocketInRange(PlacingGear socket)
    {
        if (socket == placedSocket)
            return; // a placed gear can wobble out of the trigger while turning

        socketsInRange.Remove(socket);
        socket.HideHover();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (settleRoutine != null)
        {
            StopCoroutine(settleRoutine);
            settleRoutine = null;
        }

        if (placedSocket != null)
        {
            placedSocket.Free();
            placedSocket = null;
            rb.isKinematic = false;
        }

        foreach (PlacingGear socket in socketsInRange)
            socket.ShowHover();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        foreach (PlacingGear socket in socketsInRange)
            socket.HideHover();

        settleRoutine = StartCoroutine(SettleAfterRelease());
    }

    private IEnumerator SettleAfterRelease()
    {
        // Wait one frame so XRGrabInteractable has finished restoring the Rigidbody
        yield return null;
        settleRoutine = null;

        if (grab.isSelected)
            yield break; // grabbed again in the meantime

        PlacingGear target = ClosestFreeSocket();

        if (target != null)
        {
            placedSocket = target;

            rb.isKinematic = true;
            rb.useGravity = false;

            Pose pose = target.SnapPose;
            transform.SetPositionAndRotation(pose.position, pose.rotation);

            target.Occupy(this);
        }
        else
        {
            // Dropped somewhere else: back to its normal parent, and make sure XRI didn't leave it frozen
            transform.SetParent(HomeParent, true);
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private PlacingGear ClosestFreeSocket()
    {
        PlacingGear best = null;
        float bestDistance = float.MaxValue;

        foreach (PlacingGear socket in socketsInRange)
        {
            if (socket.IsOccupied)
                continue;

            float d = (socket.transform.position - transform.position).sqrMagnitude;
            if (d < bestDistance)
            {
                bestDistance = d;
                best = socket;
            }
        }

        return best;
    }
}