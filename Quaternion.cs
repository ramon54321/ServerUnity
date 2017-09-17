using Newtonsoft.Json;

namespace ServerConsole
{
    public class Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float x, float y, float z, float w)    
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Quaternion>(json);
        }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}