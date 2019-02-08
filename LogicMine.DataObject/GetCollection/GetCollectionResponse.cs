namespace LogicMine.DataObject.GetCollection
{
    public class GetCollectionResponse<T> : Response
    {
        private T[] Objects { get; }

        public GetCollectionResponse(T[] objects)
        {
            Objects = objects;
        }

        public GetCollectionResponse(string error) : base(error)
        {
        }
    }
}