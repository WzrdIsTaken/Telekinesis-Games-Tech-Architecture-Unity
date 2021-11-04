// Interface that scripts can use to add special functionality to projectile collisions
public interface IProjectileInteraction
{
    void ProjectileCollision(int damage); 
}