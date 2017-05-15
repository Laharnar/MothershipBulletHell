using UnityEngine;

public class ProjectileSpawner :MonoBehaviour{

    public Transform projectile;
    public SpawnPoint spawnPoint;

    static GameObject rootOfAllProjectiles;

    GameObject projectilesOfThisSpawner;
    
    void Start(){
        spawnPoint = GetComponentInChildren<SpawnPoint>();

        // initialize root once per load
        if (rootOfAllProjectiles == null)
	    {
		     rootOfAllProjectiles = new GameObject();
             rootOfAllProjectiles.name = "Root empty for projectiles";
	    }

        // initialize this object
        projectilesOfThisSpawner = new GameObject();
        projectilesOfThisSpawner.transform.parent = rootOfAllProjectiles.transform;
        projectilesOfThisSpawner.name = "[ProjectileSpawner]"+transform.root.name + "/" + transform.name;


        
    }

    public Projectile Spawn() {
        Transform spawnedProjectile = Instantiate(projectile, spawnPoint.transform.position, spawnPoint.transform.rotation) as Transform;
        spawnedProjectile.parent = projectilesOfThisSpawner.transform;
        return spawnedProjectile.GetComponent<Projectile>();
    }


}
