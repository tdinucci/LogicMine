namespace Sample.LogicMine.Shop.Client.Service
{
    public class GetObjectRequest<T> : ObjectRequest<T>
    {
        public int Id { get; }

        public GetObjectRequest(int id)
        {
            Id = id;
        }
    }
}