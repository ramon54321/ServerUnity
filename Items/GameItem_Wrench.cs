using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.IdGenerators;

namespace GameItems
{
    public class GameItemInstance_Wrench : GameItemInstance
    {
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float UniqueWear { get; set; } = 0;

        protected override void SetInstanceUniqueData(GameItem newItem)
        {
            GameItem_Wrench newItemTyped = (GameItem_Wrench) newItem;
            newItemTyped.UniqueWear = UniqueWear;
        }

        public override string GetItemJson()
        {
            return GetItemFromInstance<GameItem_Wrench>().ToJson();
        }
    }

    public class GameItem_Wrench : GameItem
    {        
        // -- Unique data (Must be set from instance)
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float UniqueWear { get; set; }

        protected override void SetInstanceUniqueData(GameItemInstance newItem)
        {
            GameItemInstance_Wrench newItemTyped = (GameItemInstance_Wrench) newItem;
            newItemTyped.UniqueWear = UniqueWear;
        }

        public override string GetItemInstanceJson()
        {
            return GetInstanceFromItem<GameItemInstance_Wrench>().ToJson();
        }
    }
}
