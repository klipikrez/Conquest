using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class vfxManager : MonoBehaviour
{
    public GameObject[] vfx;
    public static vfxManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public void Play(Vector3 position, int id)
    {
        StartCoroutine(PlayParticle(position, vfx[id]));
    }

    IEnumerator PlayParticle(Vector3 position, GameObject instance)
    {
        GameObject gameobj = Instantiate(instance, position, Quaternion.identity, gameObject.transform);
        VisualEffect vfx = gameobj.GetComponent<VisualEffect>();
        yield return new WaitForSeconds(vfx.GetFloat("length")); //cekaj
        Destroy(gameobj);
    }

}
