namespace Promethium.Utility
{
    public class WeightedItem<T>
    {
        public T Item;
        public float Sum;
        public float Weight;

        public WeightedItem(T item, float weight)
        {
            Item = item;
            Weight = weight;
        }
    }
}
