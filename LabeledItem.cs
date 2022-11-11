using System;
namespace BiArcTutorial
{
    public static class LabeledItem
    {
        public static LabeledItem<R> Create<R>(String label, R item)
        {
            return new LabeledItem<R>(label, item);
        }
    }

    public class LabeledItem<T>
    {
        private readonly String label;
        private readonly T item;

        public LabeledItem(String label, T item)
        {
            this.label = label;
            this.item = item;
        }

        public T Item => item;

        public override string ToString()
        {
            return label;
        }
    }
}

