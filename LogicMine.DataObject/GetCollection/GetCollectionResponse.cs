namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionResponse<T> : Response
    {
        public T[] Objects { get; }

        public GetCollectionResponse()
        {    
        }
        
        public GetCollectionResponse(T[] objects)
        {
            Objects = objects;
        }

        public GetCollectionResponse(string error) : base(error)
        {
        }
    }
}