// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
** Purpose: A circular-array implementation of a generic queue.
**
**
=============================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Util
{
	// A simple Queue of generic objects.  Internally it is implemented as a 
	// circular buffer, so Enqueue can be O(n).  Dequeue is O(1).
	[Serializable]
	public class MutableQueue<T> : IEnumerable<T>
	{
		private T[] _array;
		private int _head;       // The index from which to dequeue if the queue isn't empty.
		private int _tail;       // The index at which to enqueue if the queue isn't full.
		private int _size;       // Number of elements.
		private int _version;
		[NonSerialized]
		private object _syncRoot;

		private const int MinimumGrow = 4;
		private const int GrowFactor = 200;  // double each time

		// Creates a queue with room for capacity objects. The default initial
		// capacity and grow factor are used.
		public MutableQueue()
		{
			_array = new T[0];
		}

		// Creates a queue with room for capacity objects. The default grow factor
		// is used.
		public MutableQueue(int capacity)
		{
			if (capacity < 0)
				throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "ArgumentOutOfRange_NeedNonNegNum");
			_array = new T[capacity];
		}

		//// Fills a Queue with the elements of an ICollection.  Uses the enumerator
		//// to get each of the elements.
		//public Queue(IEnumerable<T> collection)
		//{
		//	if (collection == null)
		//		throw new ArgumentNullException(nameof(collection));

		//	_array = EnumerableHelpers.ToArray(collection, out _size);
		//	if (_size != _array.Length) _tail = _size;
		//}

		public int Count => _size;

		// Removes all Objects from the queue.
		public void Clear()
		{
			if (_size != 0)
			{
				if (_head < _tail)
				{
					Array.Clear(_array, _head, _size);
				}
				else
				{
					Array.Clear(_array, _head, _array.Length - _head);
					Array.Clear(_array, 0, _tail);
				}

				_size = 0;
			}

			_head = 0;
			_tail = 0;
			_version++;
		}

		// CopyTo copies a collection into an Array, starting at a particular
		// index into the array.
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "ArgumentOutOfRange_Index");
			}

			int arrayLen = array.Length;
			if (arrayLen - arrayIndex < _size)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}

			int numToCopy = _size;
			if (numToCopy == 0) return;

			int firstPart = Math.Min(_array.Length - _head, numToCopy);
			Array.Copy(_array, _head, array, arrayIndex, firstPart);
			numToCopy -= firstPart;
			if (numToCopy > 0)
			{
				Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
			}
		}

		// Adds item to the tail of the queue.
		public void Enqueue(T item)
		{
			if (_size == _array.Length)
			{
				int newcapacity = (int)((long)_array.Length * (long)GrowFactor / 100);
				if (newcapacity < _array.Length + MinimumGrow)
				{
					newcapacity = _array.Length + MinimumGrow;
				}
				SetCapacity(newcapacity);
			}

			_array[_tail] = item;
			MoveNext(ref _tail);
			_size++;
			_version++;
		}

		// GetEnumerator returns an IEnumerator over this Queue.  This
		// Enumerator will support removing.
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <internalonly/>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		// Removes the object at the head of the queue and returns it. If the queue
		// is empty, this method throws an 
		// InvalidOperationException.
		public T Dequeue()
		{
			if (_size == 0)
			{
				ThrowForEmptyQueue();
			}

			T removed = _array[_head];
			_array[_head] = default(T);
			MoveNext(ref _head);
			_size--;
			_version++;
			return removed;
		}

		public bool TryDequeue(out T result)
		{
			if (_size == 0)
			{
				result = default(T);
				return false;
			}

			result = _array[_head];
			_array[_head] = default(T);
			MoveNext(ref _head);
			_size--;
			_version++;
			return true;
		}

		// Returns the object at the head of the queue. The object remains in the
		// queue. If the queue is empty, this method throws an 
		// InvalidOperationException.
		public T Peek()
		{
			if (_size == 0)
			{
				ThrowForEmptyQueue();
			}

			return _array[_head];
		}

		public bool TryPeek(out T result)
		{
			if (_size == 0)
			{
				result = default(T);
				return false;
			}

			result = _array[_head];
			return true;
		}

		// Returns true if the queue contains at least one object equal to item.
		// Equality is determined using EqualityComparer<T>.Default.Equals().
		public bool Contains(T item, EqualityComparer<T> comparer)
		{
			if (_size == 0)
			{
				return false;
			}

			if (_head < _tail)
			{
				return Array.IndexOf(_array, item, _head, _size) >= 0;
			}

			// We've wrapped around. Check both partitions, the least recently enqueued first.
			return IndexOf(_array, item, _head, _array.Length - _head, comparer) >= 0 || IndexOf(_array, item, 0, _tail, comparer) >= 0;
		}

		public bool TryGetItem(T item, out T match, IEqualityComparer<T> comparer)
		{
			var index = IndexOf(_array, item, _head, _array.Length - _head, comparer);
			index = index >= 0 ? index : IndexOf(_array, item, 0, _tail, comparer);

			if (index >= 0)
			{
				match = _array[index];
				return true;
			}
			match = default(T);
			return false;
		}

		public bool TryReplaceItem(T item, IEqualityComparer<T> comparer)
		{
			var index = IndexOf(_array, item, _head, _array.Length - _head, comparer);
			index = index >= 0 ? index : IndexOf(_array, item, 0, _tail, comparer);

			if (index >= 0)
			{
				_array[index] = item;
				return true;
			}
			return false;
		}
		
		// Returns the index of the first occurrence of a given value in a range of
		// an array. The array is searched forwards, starting at index
		// startIndex and upto count elements. The
		// elements of the array are compared to the given value using the
		// Object.Equals method.
		public static int IndexOf(Array array, T value, int startIndex, int count, IEqualityComparer<T> comparer)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			if (array.Rank != 1)
				throw new RankException("Rank_MultiDimNotSupported");

			int lb = array.GetLowerBound(0);
			if (startIndex < lb || startIndex > array.Length + lb)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "ArgumentOutOfRange_Index");
			if (count < 0 || count > array.Length - startIndex + lb)
				throw new ArgumentOutOfRangeException(nameof(count), "ArgumentOutOfRange_Count");

			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}

			T[] objArray = array as T[];
			int endIndex = startIndex + count;
			if (objArray != null)
			{
				if (value == null)
				{
					for (int i = startIndex; i < endIndex; i++)
					{
						if (objArray[i] == null) return i;
					}
				}
				else
				{
					for (int i = startIndex; i < endIndex; i++)
					{
						T obj = objArray[i];
						if (obj != null && comparer.Equals(obj, value))
						{
							return i;
						}
					}
				}
			}
			else
			{
				for (int i = startIndex; i < endIndex; i++)
				{
					T obj = (T) array.GetValue(i);
					if (obj == null)
					{
						if (value == null) return i;
					}
					else
					{
						if (comparer.Equals(obj, value)) return i;
					}
				}
			}
			// Return one less than the lower bound of the array.  This way,
			// for arrays with a lower bound of -1 we will not return -1 when the
			// item was not found.  And for SZArrays (the vast majority), -1 still
			// works for them.
			return lb - 1;
		}

		// PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
		// must be >= _size.
		private void SetCapacity(int capacity)
		{
			T[] newarray = new T[capacity];
			if (_size > 0)
			{
				if (_head < _tail)
				{
					Array.Copy(_array, _head, newarray, 0, _size);
				}
				else
				{
					Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
					Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
				}
			}

			_array = newarray;
			_head = 0;
			_tail = (_size == capacity) ? 0 : _size;
			_version++;
		}

		// Increments the index wrapping it if necessary.
		private void MoveNext(ref int index)
		{
			// It is tempting to use the remainder operator here but it is actually much slower 
			// than a simple comparison and a rarely taken branch.   
			int tmp = index + 1;
			index = (tmp == _array.Length) ? 0 : tmp;
		}

		private void ThrowForEmptyQueue()
		{
			Debug.Assert(_size == 0);
			throw new InvalidOperationException("InvalidOperation_EmptyQueue");
		}

		public void TrimExcess()
		{
			int threshold = (int)(((double)_array.Length) * 0.9);
			if (_size < threshold)
			{
				SetCapacity(_size);
			}
		}

		public struct Enumerator : IEnumerator<T>,
			System.Collections.IEnumerator
		{
			private readonly MutableQueue<T> _q;
			private readonly int _version;
			private int _index;   // -1 = not started, -2 = ended/disposed
			private T _currentElement;

			internal Enumerator(MutableQueue<T> q)
			{
				_q = q;
				_version = q._version;
				_index = -1;
				_currentElement = default(T);
			}

			public void Dispose()
			{
				_index = -2;
				_currentElement = default(T);
			}

			public bool MoveNext()
			{
				if (_version != _q._version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");

				if (_index == -2)
					return false;

				_index++;

				if (_index == _q._size)
				{
					// We've run past the last element
					_index = -2;
					_currentElement = default(T);
					return false;
				}

				// Cache some fields in locals to decrease code size
				T[] array = _q._array;
				int capacity = array.Length;

				// _index represents the 0-based index into the queue, however the queue
				// doesn't have to start from 0 and it may not even be stored contiguously in memory.

				int arrayIndex = _q._head + _index; // this is the actual index into the queue's backing array
				if (arrayIndex >= capacity)
				{
					// NOTE: Originally we were using the modulo operator here, however
					// on Intel processors it has a very high instruction latency which
					// was slowing down the loop quite a bit.
					// Replacing it with simple comparison/subtraction operations sped up
					// the average foreach loop by 2x.

					arrayIndex -= capacity; // wrap around if needed
				}

				_currentElement = array[arrayIndex];
				return true;
			}

			public T Current
			{
				get
				{
					if (_index < 0)
						ThrowEnumerationNotStartedOrEnded();
					return _currentElement;
				}
			}

			private void ThrowEnumerationNotStartedOrEnded()
			{
				Debug.Assert(_index == -1 || _index == -2);
				throw new InvalidOperationException(_index == -1 ? "InvalidOperation_EnumNotStarted" : "InvalidOperation_EnumEnded");
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			void IEnumerator.Reset()
			{
				if (_version != _q._version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				_index = -1;
				_currentElement = default(T);
			}
		}
	}
}