using UnityEngine;
using System.Collections;

public class WaterCannon : Projectile
{
    public bool selfDamage;
    public float aoeX;
    public float aoeY;
    //
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        //Only collision check your own projectiles
        if (PhotonNetwork.player.ID == this.Owner)
        {
            GameObject collidedObject = col.gameObject;

            if ((collidedObject.tag == tag_Player && collidedObject.GetComponent<PhotonView>().owner.ID != this.Owner) || col.tag == GameController.instance.tag_Ground || col.tag == GameController.instance.tag_NormalWall)
            {
                // Check aoe for manually detected collisions and apply damage to those objects.
                Vector3 aPos = this.GetComponent<Transform>().position;
                foreach (GameObject activeHero in GameController.instance.activePlayerHeroes)
                {
                    Vector3 hPos = activeHero.GetComponent<Transform>().position;
                    if (
                        aPos.x + aoeX >= hPos.x
                        &&
                        aPos.x - aoeX <= hPos.x
                        &&
                        aPos.y + aoeY >= hPos.y
                        &&
                        aPos.y - aoeY <= hPos.y
                       )
                    {
                        Debug.Log("A hit has happened to resource:" + activeHero.name);
                        if (selfDamage)
                        {
                            object[] paramsForRPC = new object[4];
                            paramsForRPC[0] = this.damage;
                            paramsForRPC[1] = activeHero.gameObject.GetComponent<PhotonView>().owner.ID;
                            paramsForRPC[2] = transform.position;
                            paramsForRPC[3] = this.Owner;

                            activeHero.gameObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);
                        }
                        else if (Owner != activeHero.GetComponent<PhotonView>().owner.ID)
                        {
                            object[] paramsForRPC = new object[4];
                            paramsForRPC[0] = this.damage;
                            paramsForRPC[1] = activeHero.gameObject.GetComponent<PhotonView>().owner.ID;
                            paramsForRPC[2] = transform.position;
                            paramsForRPC[3] = this.Owner;

                            activeHero.gameObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);
                        }
                    }
                }
                // Projectile has made proper collision for explosion, detect possible objects to damage around this area, and apply damage.
                this.GetComponent<PhotonView>().RPC("RemoveProjectileFromGame", PhotonTargets.All, null);
            }
        }
    }
}
