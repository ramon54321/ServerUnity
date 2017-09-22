using System;
using System.Collections;
using System.Collections.Generic;

namespace ToyArmyServer
{
    public enum EntityType
    {
        Player,
        PlayerOther,
        NPC
    }

    public class NPCEntityWaypoint
    {
        public Vector2 position;
        public int time;
    }

    public class NPCEntity : Entity
    {
        List<NPCEntityWaypoint> waypoints;
        private int currentWaypointIndex = 0;
        private int currentTimeCount = 0;

        public void Tick()
        {
            
        }

        public NPCEntity(int id, List<NPCEntityWaypoint> waypoints)
        {
            this.waypoints = waypoints;

            this.id = id;
            this.position = new Vector2(0,0);
            this.rotation = 0;

            this.EntityType = EntityType.NPC;

            this.entityHealthSystem = new EntityHealthSystem(this, 100);
        }
    }

    public class Entity
    {
        public int id;
        public Vector2 position;
        public float rotation;
        public Client clientData = null;

        public EntityType EntityType { get; set; }

        // -- Health system used for damage
        public EntityHealthSystem entityHealthSystem;

        public Entity(int id, EntityType entityType)
        {
            this.id = id;
            this.position = new Vector2(0,0);
            this.rotation = 0;

            this.EntityType = entityType;

            this.entityHealthSystem = new EntityHealthSystem(this, 100);
        }

        protected Entity()
        {

        }

        public string currentChunk = "";
        public void UpdateChunk()
        {
            string chunkString = GetChunkFromPosition(position);
            if(chunkString != currentChunk)
            {
                if(!Program.gameManager.entityManager.chunkEntities.ContainsKey(currentChunk))
                    Program.gameManager.entityManager.chunkEntities.Add(currentChunk, new List<Entity>());
                IList<Entity> currentChunkList = Program.gameManager.entityManager.chunkEntities[currentChunk];

                currentChunk = chunkString;

                if(!Program.gameManager.entityManager.chunkEntities.ContainsKey(currentChunk))
                    Program.gameManager.entityManager.chunkEntities.Add(currentChunk, new List<Entity>());
                IList<Entity> newChunkList = Program.gameManager.entityManager.chunkEntities[currentChunk];

                currentChunkList.Remove(this);
                newChunkList.Add(this);
            }
        }

        public string GetChunkFromPosition(Vector2 position)
        {
            int chunkX = (int)MathF.Floor(((position.x + 25) / 50));
            int chunkY = (int)MathF.Floor(((position.y + 25) / 50));
            return chunkX + "_" + chunkY;
        }

        public void SetPosition(float x, float y)
        {
            this.position.x = x;
            this.position.y = y;
            //UpdateChunk();
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
            //UpdateChunk();
        }
    }
}