using UnityEngine;
using System.Collections;
public class AbracadabraEffect : MonoBehaviour
{
	bool IscanDisable = false;
	void IsOnTriggerEnter(Collider other)
	{
        if (other.transform.name != "BirdBrown" && other.transform.name != "BirdWhite" && other.transform.name != "StorkTall")
        { IsTranforMotherMethod(other);}
        else{ PlayHideEffectParticle(other.transform); }
    }
    private void IsTranforMotherMethod(Collider other)
    {
        if (other.name == "BirdBody")
        { IsCanDisableCanHit(other); }
        else if (other.name == "ResetTriggerer" && other.tag == "Obstacles" && IscanDisable)
        { ResetThis(); }
    }
    private void IsCanDisableCanHit(Collider other)
    {
        if (!IscanDisable)
        { other.transform.parent.GetComponent<BirdTraffic>().TargetHit(true);}
    }
    void PlayHideEffectParticle(Transform parent)
    {
        PlayHideRenderCollider(parent);
        if (!IscanDisable)
        { ParticleSystem hideParticle = parent.Find("HideParticleEffect").gameObject.GetComponent("ParticleSystem") as ParticleSystem; hideParticle.Play(); }
    }
    private static void PlayHideRenderCollider(Transform parent)
    {
        parent.GetComponent<Renderer>().enabled = false;
        parent.GetComponent<Collider>().enabled = false;
    }
    void ResetThis()
	{
		IscanDisable = false;
		this.transform.localPosition = new Vector3(-70, 0, -5);
		this.gameObject.SetActive(false);
	}
	public void Disable()
	{
		IscanDisable = true;
	}
}
