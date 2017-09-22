namespace ToyArmyServer
{    public class EntityHealthSystem
    {
        public Entity entity;

        private float health;
        private float maxHealth;

        public EntityHealthSystem(Entity entity, float maxHealth)
        {
            this.entity = entity;
            this.maxHealth = maxHealth;
            this.health = maxHealth;
        }

        private void Respawn()
        {
            health = maxHealth;
        }

        public void Damage(float amount)
        {
            health -= amount;

            if(health <= 0)
            {
                // -- Die
                Program.clientManager.SendMessageToAllClients("/entitydeath/" + entity.id + "/");
                Respawn();
                return;
            }

            Program.clientManager.SendMessageToAllClients("/entitydamagehealth/" + entity.id + "/" + health + "/" + amount + "/");
        }

    }
}