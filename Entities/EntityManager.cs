using System;
using System.Collections.Generic;

namespace ToyArmyServer
{
    public class EntityManager
    {
        private int currentId = 0;

        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        public List<NPCEntity> npcEntities = new List<NPCEntity>();

        /*
        public void CreateNewEntityWithAutoId(Entity entity)
        {
            entity.id = currentId;
            currentId++;
            entities.Add(entity.id, entity);
        }
        */

        public Entity CreateNewNPC(Vector2 position, List<NPCEntityWaypoint> waypoints)
        {
            NPCEntity newEntity = new NPCEntity(currentId, waypoints);
            currentId++;
            entities.Add(newEntity.id, newEntity);
            npcEntities.Add(newEntity);
            return newEntity;
        }

        public Entity CreateNewEntityWithAutoId(EntityType entityType)
        {
            Entity newEntity = CreateNewEntity(currentId, entityType);
            currentId++;
            return newEntity;
        }

        public Entity CreateNewEntity(int id, EntityType entityType)
        {
            Entity newEntity = new Entity(id, entityType);
            entities.Add(id, newEntity);
            return newEntity;
        }

        public Dictionary<string, IList<Entity>> chunkEntities = new Dictionary<string, IList<Entity>>();

        public IList<Entity> GetEntitiesInSurroundingChunks(Client client)
        {
            // TODO: Calculate entities in surrouding chunks, not just current
            /*
            string chunkString = client.Entity.currentChunk;
            if(chunkEntities.ContainsKey(chunkString))
                return chunkEntities[chunkString];
            return new List<Entity>();
            */
            IList<Entity> allEntities = new List<Entity>();
            foreach(IList<Entity> list in chunkEntities.Values)
            {
                foreach(Entity e in list)
                {
                    allEntities.Add(e);
                }
            }
            return allEntities;
        }

        public Entity GetEntityById(int id)
        {
            try
            {
                return entities[id];
            }
            catch (Exception e)
            {
                Console.WriteLine("Entity not found with id " + id + ", returning null.");
                return null;
            }
        }

        public List<Entity> GetEntities()
        {
            return new List<Entity>(entities.Values);
        }

        public bool RemoveEntity(int id)
        {
            Entity e = GetEntityById(id);
            if(!Program.gameManager.entityManager.chunkEntities.ContainsKey(e.currentChunk))
                Program.gameManager.entityManager.chunkEntities.Add(e.currentChunk, new List<Entity>());
            IList<Entity> currentChunkList = Program.gameManager.entityManager.chunkEntities[e.currentChunk];
            
            currentChunkList.Remove(e);

            if(e.EntityType == EntityType.NPC)
                npcEntities.Remove((NPCEntity) e);

            return entities.Remove(id);
        }
    }
}