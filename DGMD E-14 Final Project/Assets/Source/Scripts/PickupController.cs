using System;
using UnityEngine;

/// <summary>
/// This script component is used to control picking up and dropping object's with rigidbody components attached to them.
/// </summary>
public class PickupController : MonoBehaviour
{
    [Header("Pickup Object Layer")]
    [Tooltip("Which layer mask should this controller interact with?")]
    public LayerMask PickupLayer;

    [Header("Time Between Pickups")]
    [Tooltip("How long of a delay should there be between picking up or dropping?")]
    public float TimeBetweenPickups = 0.5f;

    [Header("Pickup range")]
    [Tooltip("How far should this controller be able to reach in picking up objects?")]
    public float PickupRange = 1.0f;

    /// <summary>
    /// The time, in seconds, since the last pickup action occcurred.
    /// </summary>
    private float timeInSecondsSinceLastPickupAction = 0;

    /// <summary>
    /// A reference to the currently held object, if any.
    /// </summary>
    private GameObject heldObject = null;

    /// <summary>
    /// A reference to the currently held object's rigidbody, if any.
    /// </summary>
    private Rigidbody heldObjectRigidbody = null;

    /// <summary>
    /// Stores the currently held object's rigidbody drag, if any, so it can be restored when the object is dropped.
    /// </summary>
    private float heldObjectPreviousDrag = 0;

    /// <summary>
    /// Stores the currently held object's rigidbody constraints, if any, so they can be restored when the object is dropped.
    /// </summary>
    private RigidbodyConstraints heldObjectPreviousRigidbodyConstraints = RigidbodyConstraints.None;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        this.heldObject = null;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Add delta time to sime since last pickup
        if (timeInSecondsSinceLastPickupAction <= TimeBetweenPickups)
        {
            timeInSecondsSinceLastPickupAction += Time.deltaTime;
        }

        // If the pickup button is pressed, shoot a ray to try to pickup an object:
        if (Input.GetButton("Fire1") && timeInSecondsSinceLastPickupAction >= TimeBetweenPickups)
        {
            // Reset the pickup timer so that the timer can start to when next pickup is allowed:
            timeInSecondsSinceLastPickupAction = 0;

            if (heldObject == null)
            {
                PickUpNearedGameObjectWithRigidbody();
            }
            else
            {
                DropObject();
            }
        }

        
        if(heldObject != null)
        {
            // TODO: make this constant a public parameter
            float pickupSpeed = 1.0f;
            this.heldObject.transform.parent = this.transform;
            float deltaY = 0;
            float deltaZ = 0;
            if(this.heldObject.transform.localPosition.y < 1.375f)
            {
                deltaY = pickupSpeed * Time.deltaTime;
            }
            if (this.heldObject.transform.localPosition.z < 1.25f)
            {
                deltaZ = pickupSpeed * Time.deltaTime;
            }
            this.heldObject.transform.localPosition = new Vector3(0, this.heldObject.transform.localPosition.y + deltaY, 1.25f);
        }
    }

    /// <summary>
    /// Shoots a forward ray based on configured settings and picks up the nearest object which the ray collides with.
    /// </summary>
    private void PickUpNearedGameObjectWithRigidbody()
    {
        if (this.heldObject != null)
        {
            return;
        }

        GameObject pickupableObject = null;

        RaycastHit[] raycastHits;
        Ray pickupRay = new Ray(transform.position, transform.forward);
        raycastHits = Physics.RaycastAll(pickupRay, this.PickupRange, this.PickupLayer);
        Array.Sort(raycastHits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if(raycastHits[i].transform.gameObject.GetComponent<Rigidbody>() != null)
            {
                pickupableObject = raycastHits[i].transform.gameObject;
                break;
            }
        }

        if (pickupableObject != null)
        {
            Rigidbody pickupableRigidbody = pickupableObject.GetComponent<Rigidbody>();
            if (pickupableRigidbody != null)
            {
                heldObjectPreviousDrag = pickupableRigidbody.drag;
                heldObjectPreviousRigidbodyConstraints = pickupableRigidbody.constraints;
                this.heldObject = pickupableObject;
                this.heldObjectRigidbody = this.heldObject.GetComponent<Rigidbody>();
                heldObjectRigidbody.useGravity = false;
                heldObjectRigidbody.drag = 10;
                heldObjectRigidbody.constraints = RigidbodyConstraints.FreezePosition;
                heldObjectRigidbody.transform.parent = this.gameObject.transform;
            }
        }
    }

    /// <summary>
    /// Drops the currently held object, if any.
    /// </summary>
    private void DropObject()
    {
        if (heldObject == null)
        {
            return;
        }

        Rigidbody heldRigidbody = this.heldObject.GetComponent<Rigidbody>();
        if (this.heldObject != null)
        {
            heldRigidbody.useGravity = true;
            heldRigidbody.drag = this.heldObjectPreviousDrag;
            this.heldObjectPreviousDrag = 0;
            heldRigidbody.constraints = heldObjectPreviousRigidbodyConstraints;
            heldRigidbody.transform.parent = null;
            heldObject = null;
        }
    }
}
