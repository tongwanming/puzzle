namespace Hyperbyte.BlockPuzzle.Scripts.Http.Model
{
    public class Properties : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string is_login_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string app_version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wifi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string device_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_agent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int screen_width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int screen_height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int product_id { get; set; }
        /// <summary>
        /// 苹果
        /// </summary>
        public string product_name { get; set; }
        /// <summary>
        /// 水果
        /// </summary>
        public string product_classify { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int product_price { get; set; }
        
        public int value { get; set; }
    }

    public class EventModel : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string distinct_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @event { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string project { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Properties properties { get; set; }
    }

}