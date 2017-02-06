using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PushNotification.Monitors
{
    public class CircularBuffer<T>
    {
        T[] buffer;
        int position;
        int size;

        public CircularBuffer(int size)
        {
            this.size = size;
            buffer = new T[size];
            position = 0;
        }

        public void Write(T item)
        {
            buffer[position] = item;
            MoveNext();
        }

        private void MoveNext()
        {
            position++;
            if (position == size)
                position = 0;
        }

        public void Clear()
        {
            buffer = new T[size];
            position = 0;
        }

        public IReadOnlyCollection<T> GetValues()
        {
            return new ReadOnlyCollection<T>(buffer.Where(x => x.Equals(default(T)) == false).ToList());
        }
    }
}
