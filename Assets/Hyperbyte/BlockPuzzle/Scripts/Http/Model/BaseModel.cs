namespace Hyperbyte.BlockPuzzle.Scripts.Http.Model
{
    public class BaseModel
    {
        public override string ToString()
        {
            return JsonMapper.ToJson(this);
        }
    }
}