using UnityEngine;

/// <summary>
/// A component which handles collection of picked up objects within the game.
/// </summary>
public class PickupCollector : MonoBehaviour
{
    /// <summary>
    /// A reference to the particle effects for successfully collected objects.
    /// </summary>
    public GameObject SuccessParticles = null;

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collider/rigidbody which collided with the entity this script is attached to.</param>
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            if(this.SuccessParticles != null)
            {
                GameObject particles = Instantiate(SuccessParticles);
                particles.transform.parent = this.transform;
                particles.transform.position = this.transform.position;
            }

            Destroy(collision.gameObject);
        }
    }
}
