using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.IdGenerators;

namespace GameItems
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(GameItemInstance_Weapon), typeof(GameItemInstance_Wrench), typeof(GameItemInstance_Magazine))]
    public class GameItemInstance
    {
        [BsonId]
        public ObjectId InstanceId { get; set; }
        public ObjectId BlueprintId { get; set; }

        public static Tinstance GetNewItemInstance<Tinstance, Titem>(string systemName) where Tinstance : GameItemInstance where Titem : GameItem
        {
            Tinstance newInstance = (Tinstance) Activator.CreateInstance(typeof(Tinstance));
            newInstance.InstanceId = ObjectId.GenerateNewId();
            newInstance.BlueprintId = ToyArmyServer.DatabaseManager.GetCollection<Titem>("ItemBlueprints").Find(item => item.ItemNameSystem == systemName).ToCursor().First().BlueprintId;
            return (Tinstance) newInstance;
        }

        public static BsonDocument GetNewItemInstance(string systemName)
        {
            BsonDocument blueprint = ToyArmyServer.DatabaseManager.GetCollection<BsonDocument>("ItemBlueprints").Find(new BsonDocument("ItemNameSystem", systemName)).ToCursor().First();
            
            BsonDocument newInstance = new BsonDocument { 
                { "_t",  blueprint["InstanceType"].AsBsonArray },
                { "_id", ObjectId.GenerateNewId() },
                { "BlueprintId", blueprint["_id"].AsObjectId }
            };

            return newInstance;
        }

        protected virtual void SetInstanceUniqueData(GameItem newItem)
        {

        }

        public virtual string GetItemJson()
        {
            return GetItemFromInstance<GameItem>().ToJson();
        }

        protected Titem GetItemFromInstance<Titem>() where Titem : GameItem
        {
            Titem newItem = (Titem) ToyArmyServer.DatabaseManager.itemBlueprintsCollection.Find(itm => itm.BlueprintId == BlueprintId).ToCursor().First();
            newItem.InstanceId = InstanceId;
            SetInstanceUniqueData(newItem);
            return newItem;
        }
    }

    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(GameItem_Weapon), typeof(GameItem_Wrench), typeof(GameItem_Magazine))]
    public class GameItem
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId BlueprintId { get; set; }
        public ObjectId InstanceId { get; set; }
        public string[] InstanceType { get; set; }
        public string ItemCode { get; set; }
        public string ItemVersion { get; set; }
        public string ItemNameSystem { get; set; }
        public string ItemNameLong { get; set; }
        public string ItemNameShort { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ItemValue { get; set; }
        public string ItemDescription { get; set; }
        public bool ItemIsCarryable { get; set; }
        public bool ItemIsStackable { get; set; }
        public int ItemStackMax { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ItemWeight { get; set; }
        public string ItemVolume { get; set; }

        public GameItem()
        {
            BlueprintId = ObjectId.GenerateNewId();
        }

        protected virtual void SetInstanceUniqueData(GameItemInstance newItemInstance)
        {

        }

        public virtual string GetItemInstanceJson()
        {
            return GetInstanceFromItem<GameItemInstance>().ToJson();
        }

        protected Tinstance GetInstanceFromItem<Tinstance>() where Tinstance : GameItemInstance
        {
            Tinstance newItemInstance = (Tinstance) Activator.CreateInstance(typeof(Tinstance));;
            newItemInstance.InstanceId = InstanceId;
            newItemInstance.BlueprintId = BlueprintId;
            SetInstanceUniqueData(newItemInstance);
            return newItemInstance;
        }
    }

    public enum MagazineType
	{
	    strip_5,
	    strip_8,
	    strip_12,
	    strip_box_20,
		strip_box_25
	}
}
