using UnityEngine;
using System.Collections;
public class AbracadabraEffect : MonoBehaviour
{
	bool canDisable = false;
	void OnTriggerEnter(Collider other)
	{
        if (other.transform.name != "BirdBrown" && other.transform.name != "BirdWhite" && other.transform.name != "StorkTall")
        { tranformotherMethod(other);}
        else{ PlayHideEffectParticle(other.transform); }
    }
    private void tranformotherMethod(Collider other)
    {
        if (other.name == "BirdBody")
        { birdbodyifMethod(other); }
        else if (other.name == "ResetTriggerer" && other.tag == "Obstacles" && canDisable)
        { ResetThis(); }
    }
    private void birdbodyifMethod(Collider other)
    {
        if (!canDisable)
        { other.transform.parent.GetComponent<BirdTraffic>().TargetHit(true);}
    }
    void PlayHideEffectParticle(Transform parent)
    {
        playhideeffectpartimethod(parent);
        if (!canDisable)
        { ParticleSystem hideParticle = parent.Find("HideParticleEffect").gameObject.GetComponent("ParticleSystem") as ParticleSystem; hideParticle.Play(); }
    }
    private static void playhideeffectpartimethod(Transform parent)
    {
        parent.GetComponent<Renderer>().enabled = false;
        parent.GetComponent<Collider>().enabled = false;
    }
    void ResetThis()
	{
		canDisable = false;
		this.transform.localPosition = new Vector3(-70, 0, -5);
		this.gameObject.SetActive(false);
	}
	public void Disable()
	{
		canDisable = true;
	}
}
