using Newtonsoft.Json;

namespace ToyArmyServer
{
    public class Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)    
        {
            this.x = x;
            this.y = y;
        }

        public Vector2()
        {
            this.x = 0;
            this.y = 0;
        }

        public Vector2 FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Vector2>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}