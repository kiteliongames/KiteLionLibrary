using UnityEngine;
using System.Collections;
using KiteLion.Common;
using KiteLion.Debugging;

public abstract class PlayerController : MonoBehaviour {

    /// <summary>
    /// Nothing to do. You stay invisible and immobile
    /// </summary>
    public abstract void Die();

    public abstract void Respawn(Vector3 spawnPoint);

    public void Initialize () {

    }

}
