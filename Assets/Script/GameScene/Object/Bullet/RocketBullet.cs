using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public TrailRenderer tr;

    private Coroutine coroutine;

    private void OnEnable()
    {
        if (particleSystems.Count > 0)
        {
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Play();
            }
            tr.Clear();
        }
        
        // 缓存池保存
        coroutine = StartCoroutine(PushCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Buildings"))
        {
            //创建打击特效
            GameObject hitEff = PoolManager.Instance.GetObject("Prefabs/Bullet/RocketBulletHit");
            if (hitEff)
            {
                hitEff.transform.position = transform.position;
                hitEff.transform.rotation = transform.rotation;
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/RocketBulletHit", resHitEff =>
                {
                    hitEff = Instantiate(resHitEff);
                    hitEff.name = "Prefabs/Bullet/RocketBulletHit";
                    hitEff.transform.position = transform.position;
                    hitEff.transform.rotation = transform.rotation;
                });
            }
            
            // 范围检测
            Collider[] enemies =
                Physics.OverlapSphere(transform.position, 4f, 1 << LayerMask.NameToLayer("Enemy"));
            foreach (Collider enemy in enemies)
            {
                enemy.GetComponent<Zombie>().Wound(GameManger.Instance.player.GetComponent<Player>().weaponBag.weapon.atk + GameManger.Instance.player.GetComponent<Player>().baseAtk);
            }
            
            StopCoroutine(coroutine);
            PushBullet();
        }
    }

    private void PushBullet()
    {
        PoolManager.Instance.PushObject(gameObject);
    }

    private IEnumerator PushCoroutine()
    {
        yield return new WaitForSeconds(3f);
        PushBullet();
    }
}
