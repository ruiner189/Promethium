using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Utility
{
    public class WeightedList<T>
    {
        private List<WeightedItem<T>> _items = new List<WeightedItem<T>>();
        private float _totalWeights = 0;
        private Random _random = new Random();

        public void Add(T item, float weight)
        {
            _items.Add(new WeightedItem<T>(item, weight));
        }

        public void Remove(T item)
        {
            _items.RemoveAll(obj => obj.Item.Equals(item));
        }

        private void CalculateWeights()
        {
            _totalWeights = 0;
            foreach(WeightedItem<T> item in _items)
            {
                item.Sum = _totalWeights;
                _totalWeights += item.Weight;
            }
        }

        public T GetRandomItem()
        {
            CalculateWeights();
            float value = _totalWeights * (float) _random.NextDouble();
            foreach(WeightedItem<T> item in _items)
            {
                if(item.Sum <= value && (item.Sum + item.Weight) > value)
                {
                    return item.Item;
                }
            }

            return default;
        }

        public T this[int i]
        {
            get { return _items[i].Item;}
        }
    }

}
