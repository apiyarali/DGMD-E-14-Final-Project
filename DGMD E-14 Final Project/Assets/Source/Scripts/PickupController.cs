using UnityEngine;

public class PickupController : MonoBehaviour
{
    private GameObject heldObject = null;
    public float TimeBetweenPickups = 0.5f;
    private float timeSinceLastPickup = 0;
    public float PickupRange = 1.0f;
    private Rigidbody heldObjectRigidbody = null;
    private float heldObjectPreviousDrag = 0;
    private RigidbodyConstraints heldObjectPreviousRigidbodyConstraints = RigidbodyConstraints.None;

    private void Start()
    {
        this.heldObject = null;
    }

    void Update()
    {
        // Add delta time to sime since last pickup
        if (timeSinceLastPickup <= TimeBetweenPickups)
        {
            timeSinceLastPickup += Time.deltaTime;
        }

        // If the pickup button is pressed, shoot a ray to try to pickup an object:
        if (Input.GetButton("Fire1") && timeSinceLastPickup >= TimeBetweenPickups)
        {
            // Reset the pickup timer so that the timer can start to when next pickup is allowed:
            timeSinceLastPickup = 0;

            if (heldObject == null)
            {
                PickupObject();
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
            this.heldObject.transform.localPosition = new Vector3(0, this.heldObject.transform.localPosition.y + deltaY, 1.25f);
        }
    }

    private void MoveObject()
    {
        Vector3 cameraPosition = new Vector3(this.gameObject.transform.position.x, 1.375f + this.gameObject.transform.position.y, this.gameObject.transform.position.z + 2.0f);
        this.heldObject.transform.position = cameraPosition;/*
        if (Vector3.Distance(this.heldObject.transform.position, cameraPosition) > 0.1f)
        {
            Vector3 moveDirection = cameraPosition - this.heldObject.transform.position;
            Debug.Log(moveDirection);
            this.heldObjectRigidbody.AddForce(moveDirection * 150.0f); // TODO: pickup force
        }*/

        // Raise the object to eye level:
    }

    private void PickupObject()
    {
        if (this.heldObject != null)
        {
            return;
        }

        Debug.Log("Attemping to pickup object.");
        GameObject pickupableObject = null;
        RaycastHit raycastHit;
        if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.forward), out raycastHit, this.PickupRange)) {
            pickupableObject = raycastHit.transform.gameObject;
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
                Debug.Log("Object picked up.");
            } else
            {
                Debug.Log("Object did not have a rigidbody.");
            }
        } else
        {
            Debug.Log("Nothing to pick up.");
        }
    }

    private void DropObject()
    {
        if (heldObject == null)
        {
            return;
        }

        Debug.Log("Attemping to drop object.");
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
