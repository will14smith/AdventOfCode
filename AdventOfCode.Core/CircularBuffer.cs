// _heavily_ based on: https://github.com/joaoportela/CircullarBuffer-CSharp/blob/master/CircularBuffer/CircularBuffer.cs
// mostly changed to work better with Span, Memory, and Sequence

using System.Buffers;
using System.Collections;

namespace AdventOfCode.Core;

/// <inheritdoc/>
/// <summary>
/// Circular buffer.
/// 
/// When writing to a full buffer:
/// PushBack -> removes this[0] / Front()
/// PushFront -> removes this[Size-1] / Back()
/// 
/// this implementation is inspired by
/// http://www.boost.org/doc/libs/1_53_0/libs/circular_buffer/doc/circular_buffer.html
/// because I liked their interface.
/// </summary>
public class CircularBuffer<T> : IEnumerable<T>
{
    private readonly T[] _buffer;

    /// <summary>
    /// The _start. Index of the first element in buffer.
    /// </summary>
    private int _start;

    /// <summary>
    /// The _end. Index after the last element in the buffer.
    /// </summary>
    private int _end;

    public CircularBuffer(int capacity)
        : this(capacity, ReadOnlySequence<T>.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
    /// 
    /// </summary>
    /// <param name='capacity'>
    /// Buffer capacity. Must be positive.
    /// </param>
    /// <param name='items'>
    /// Items to fill buffer with. Items length must be less than capacity.
    /// Suggestion: use Skip(x).Take(y).ToArray() to build this argument from
    /// any enumerable.
    /// </param>
    public CircularBuffer(int capacity, ReadOnlySequence<T> items)
    {
        if (capacity < 1)
        {
            throw new ArgumentException(
                "Circular buffer cannot have negative or zero capacity.", nameof(capacity));
        }
        if (items.Length > capacity)
        {
            throw new ArgumentException(
                "Too many items to fit circular buffer", nameof(items));
        }

        _buffer = new T[capacity];
        items.CopyTo(_buffer.AsSpan());
            
        Count = (int)items.Length;

        _start = 0;
        _end = Count == capacity ? 0 : Count;
    }

    /// <summary>
    /// Maximum capacity of the buffer. Elements pushed into the buffer after
    /// maximum capacity is reached (IsFull = true), will remove an element.
    /// </summary>
    public int Capacity => _buffer.Length;

    public bool IsFull => Count == Capacity;

    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Current buffer size (the number of elements that the buffer has).
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Element at the front of the buffer - this[0].
    /// </summary>
    /// <returns>The value of the element of type T at the front of the buffer.</returns>
    public T Front()
    {
        ThrowIfEmpty();
        return _buffer[_start];
    }

    /// <summary>
    /// Element at the back of the buffer - this[Size - 1].
    /// </summary>
    /// <returns>The value of the element of type T at the back of the buffer.</returns>
    public T Back()
    {
        ThrowIfEmpty();
        return _buffer[(_end != 0 ? _end : Capacity) - 1];
    }

    public T this[int index]
    {
        get
        {
            if (IsEmpty)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
            }
            if (index >= Count)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Count}");
            }
            var actualIndex = InternalIndex(index);
            return _buffer[actualIndex];
        }
        set
        {
            if (IsEmpty)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
            }
            if (index >= Count)
            {
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Count}");
            }
            int actualIndex = InternalIndex(index);
            _buffer[actualIndex] = value;
        }
    }

    /// <summary>
    /// Pushes a new element to the back of the buffer. Back()/this[Size-1]
    /// will now return this element.
    /// 
    /// When the buffer is full, the element at Front()/this[0] will be 
    /// popped to allow for this new element to fit.
    /// </summary>
    /// <param name="item">Item to push to the back of the buffer</param>
    public void PushBack(T item)
    {
        if (IsFull)
        {
            _buffer[_end] = item;
            Increment(ref _end);
            _start = _end;
        }
        else
        {
            _buffer[_end] = item;
            Increment(ref _end);
            ++Count;
        }
    }

    /// <summary>
    /// Pushes a new element to the front of the buffer. Front()/this[0]
    /// will now return this element.
    /// 
    /// When the buffer is full, the element at Back()/this[Size-1] will be 
    /// popped to allow for this new element to fit.
    /// </summary>
    /// <param name="item">Item to push to the front of the buffer</param>
    public void PushFront(T item)
    {
        if (IsFull)
        {
            Decrement(ref _start);
            _end = _start;
            _buffer[_start] = item;
        }
        else
        {
            Decrement(ref _start);
            _buffer[_start] = item;
            ++Count;
        }
    }

    /// <summary>
    /// Removes the element at the back of the buffer. Decreasing the 
    /// Buffer size by 1.
    /// </summary>
    public T PopBack()
    {
        ThrowIfEmpty("Cannot take elements from an empty buffer.");
        Decrement(ref _end);
        var item = _buffer[_end];
        _buffer[_end] = default(T);
        --Count;
        return item;
    }

    /// <summary>
    /// Removes the element at the front of the buffer. Decreasing the 
    /// Buffer size by 1.
    /// </summary>
    public T PopFront()
    {
        ThrowIfEmpty("Cannot take elements from an empty buffer.");
        var item = _buffer[_start];
        _buffer[_start] = default(T);
        Increment(ref _start);
        --Count;
        return item;
    }

    /// <summary>
    /// Copies the buffer contents to an array, according to the logical
    /// contents of the buffer (i.e. independent of the internal 
    /// order/contents)
    /// </summary>
    /// <returns>A new array with a copy of the buffer contents.</returns>
    public T[] ToArray()
    {
        var newArray = new T[Count];

        var arrayOne = ArrayOne();
        arrayOne.CopyTo(newArray.AsMemory(0));
        ArrayTwo().CopyTo(newArray.AsMemory(arrayOne.Length));
            
        return newArray;
    }

    #region IEnumerable<T> implementation
    public IEnumerator<T> GetEnumerator()
    {
        var one = ArrayOne();
        for (var index = 0; index < one.Length; index++)
        {
            yield return one.Span[index];
        }    
            
        var two = ArrayOne();
        for (var index = 0; index < two.Length; index++)
        {
            yield return two.Span[index];
        }
    }
    #endregion
    #region IEnumerable implementation
    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }
    #endregion

    private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Increments the provided index variable by one, wrapping
    /// around if necessary.
    /// </summary>
    /// <param name="index"></param>
    private void Increment(ref int index)
    {
        if (++index == Capacity)
        {
            index = 0;
        }
    }

    /// <summary>
    /// Decrements the provided index variable by one, wrapping
    /// around if necessary.
    /// </summary>
    /// <param name="index"></param>
    private void Decrement(ref int index)
    {
        if (index == 0)
        {
            index = Capacity;
        }
        index--;
    }

    /// <summary>
    /// Converts the index in the argument to an index in <code>_buffer</code>
    /// </summary>
    /// <returns>
    /// The transformed index.
    /// </returns>
    /// <param name='index'>
    /// External index.
    /// </param>
    private int InternalIndex(int index)
    {
        return _start + (index < (Capacity - _start) ? index : index - Capacity);
    }

    // doing ArrayOne and ArrayTwo methods returning ArraySegment<T> as seen here: 
    // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1957cccdcb0c4ef7d80a34a990065818d
    // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1f5081a54afbc2dfc1a7fb20329df7d5b
    // should help a lot with the code.

    #region Array items easy access.
    // The array is composed by at most two non-contiguous segments, 
    // the next two methods allow easy access to those.

    private ReadOnlyMemory<T> ArrayOne()
    {
        if (IsEmpty)
        {
            return ReadOnlyMemory<T>.Empty;
        }

        var end = _start < _end ? _end : _buffer.Length;

        return _buffer.AsMemory(_start, end - _start);
    }

    private ReadOnlyMemory<T> ArrayTwo()
    {
        if (IsEmpty || _start < _end)
        {
            return ReadOnlyMemory<T>.Empty;
        }

        return _buffer.AsMemory(0, _end);
    }

    public ReadOnlySequence<T> Take(int count) => ToSequence().Slice(0, count);

    public ReadOnlySequence<T> ToSequence()
    {
        var a1 = ArrayOne();
        var a2 = ArrayTwo();
            
        if(a1.IsEmpty && a2.IsEmpty) return ReadOnlySequence<T>.Empty;
        if (a1.IsEmpty) return new ReadOnlySequence<T>(a2);
        if (a2.IsEmpty) return new ReadOnlySequence<T>(a1);

        var first = new MemorySegment(a1);
        var second = first.Append(a2);
            
        return new ReadOnlySequence<T>(first, 0, second, a2.Length);
    }
        
    #endregion
        
    private class MemorySegment : ReadOnlySequenceSegment<T>
    {
        public MemorySegment(ReadOnlyMemory<T> memory)
        {
            Memory = memory;
        }

        public MemorySegment Append(ReadOnlyMemory<T> memory)
        {
            var segment = new MemorySegment(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };

            Next = segment;

            return segment;
        }
    }

}