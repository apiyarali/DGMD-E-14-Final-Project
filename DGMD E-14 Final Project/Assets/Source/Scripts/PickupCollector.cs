using UnityEngine;

public class PickupCollector : MonoBehaviour
{
    public GameObject SuccessParticles = null;

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collector collision detected.");
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
