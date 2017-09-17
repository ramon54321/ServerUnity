using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.IdGenerators;

namespace GameItems
{
    public class GameItemInstance_Magazine : GameItemInstance
    {
        public int UniqueAmmoCount { get; set; } = 1;

        protected override void SetInstanceUniqueData(GameItem newItem)
        {
            GameItem_Magazine newItemTyped = (GameItem_Magazine) newItem;
            newItemTyped.UniqueAmmoCount = UniqueAmmoCount;
        }

        public override string GetItemJson()
        {
            return GetItemFromInstance<GameItem_Magazine>().ToJson();
        }
    }

    public class GameItem_Magazine : GameItem
    {        
        [BsonRepresentation(BsonType.String)]
        public MagazineType ClassMagazineType { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ClassDamage { get; set; }

        // -- Unique data (Must be set from instance)
        public int UniqueAmmoCount { get; set; }

        protected override void SetInstanceUniqueData(GameItemInstance newItem)
        {
            GameItemInstance_Magazine newItemTyped = (GameItemInstance_Magazine) newItem;
            newItemTyped.UniqueAmmoCount = UniqueAmmoCount;
        }

        public override string GetItemInstanceJson()
        {
            return GetInstanceFromItem<GameItemInstance_Magazine>().ToJson();
        }
    }
}
