namespace Sample.LogicMine.Shop.Client.Service
{
    public class GetCollectionRequest<T> : ObjectRequest<T>
    {
        public string Filter { get; set; }
        public int? Max { get; set; }
        public int? Page { get; set; }
    }
}