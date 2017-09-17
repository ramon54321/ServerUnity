using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.IdGenerators;

namespace GameItems
{
    public class GameItemInstance_Weapon : GameItemInstance
    {
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float UniqueWear { get; set; } = 20;

        protected override void SetInstanceUniqueData(GameItem newItem)
        {
            GameItem_Weapon newItemTyped = (GameItem_Weapon) newItem;
            newItemTyped.UniqueWear = UniqueWear;
        }

        public override string GetItemJson()
        {
            return GetItemFromInstance<GameItem_Weapon>().ToJson();
        }
    }

    public class GameItem_Weapon : GameItem
    {
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ClassReloadTime { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
	    public float ClassRPS { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
	    public float ClassBarrelVelocityCoef { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
	    public float ClassUnaccuracy { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
	    public float ClassNoiseCoef { get; set; }
		[BsonRepresentation(BsonType.String)]
	    public List<MagazineType> ClassMagazineTypeList { get; set; }
        
        // -- Unique data (Must be set from instance)
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float UniqueWear { get; set; }

        protected override void SetInstanceUniqueData(GameItemInstance newItem)
        {
            GameItemInstance_Weapon newItemTyped = (GameItemInstance_Weapon) newItem;
            newItemTyped.UniqueWear = UniqueWear;
        }

        public override string GetItemInstanceJson()
        {
            return GetInstanceFromItem<GameItemInstance_Weapon>().ToJson();
        }
    }
}
