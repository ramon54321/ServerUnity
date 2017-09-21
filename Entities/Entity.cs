using System;
using System.Collections;
using System.Collections.Generic;

namespace ToyArmyServer
{
    public class Entity
    {
        public int id;
        public Vector2 position;
        public float rotation;
        public Client clientData = null;

        // -- Health system used for damage
        public EntityHealthSystem entityHealthSystem;

        public Entity()
        {
            this.position = new Vector2(0,0);
            this.rotation = 0;

            this.entityHealthSystem = new EntityHealthSystem(this, 100);
        }

        public Entity(int id)
        {
            this.id = id;
            this.position = new Vector2(0,0);
            this.rotation = 0;

            this.entityHealthSystem = new EntityHealthSystem(this, 100);
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