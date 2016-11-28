using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace org.apache.commons.cli
{
    internal static class ExtensionMethod
    {
        internal static bool isEmpty(this IList list)
        {
            return (list.Count == 0);
        }

        internal static bool isEmpty(this IList<string> list)
        {
            return (list.Count == 0);
        }

        internal static bool isEmpty(this IList<object> list)
        {
            return (list.Count == 0);
        }

        internal static bool isEmpty(this ICollection<Option> list)
        {
            return (list.Count == 0);
        }

        internal static bool isEmpty(this IDictionary<object, object> list)
        {
            return (list.Count == 0);
        }

        internal static bool containsAll(this IList<Option> list, IList<Option> subList)
        {
            foreach (Option opt in subList)
            {
                if (!list.Contains(opt))
                {
                    return false;
                }
            }
            return true;
        }

        internal static IListIterator listIterator(this IList list)
        {
            return new ListIterator(list);
        }

        internal static IListIterator iterator(this IList list)
        {
            return new ListIterator(list);
        }

        internal static IListIterator<string> listIterator(this IList<string> list)
        {
            return new ListIterator<string>(list);
        }

        internal static IListIterator<string> iterator(this IList<string> list)
        {
            return new ListIterator<string>(list);
        }

        internal static IIterator<Option> iterator(this ICollection<Option> list)
        {
            return new CollectionIterator<Option>(list);
        }

        internal static IIterator<string> iterator(this ICollection<string> list)
        {
            return new CollectionIterator<string>(list);
        }

        internal static IIterator<object> iterator(this ICollection<object> list)
        {
            return new CollectionIterator<object>(list);
        }

        internal static int size(this ICollection<string> list)
        {
            return list.Count;
        }

        internal static void clear(this IList<string> list)
        {
            list.Clear();
        }

        internal static string ToDumpString(this IDictionary<string, Option> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Option option in dict.Values)
            {
                sb.Append(option.ToString());
            }
            return sb.ToString();
        }

        internal static IList<OptionGroup> RemoveDuplicated(this IList<OptionGroup> groups)
        {
            List<OptionGroup> groupList = new List<OptionGroup>();

            foreach (var group in groups)
            {
                if (!groupList.Contains(group))
                {
                    groupList.Add(group);
                }
            }

            return groupList;
        }

    }

    public interface IIterator
    {
        bool hasNext();

        object next();
    }

    public interface IListIterator : IIterator
    {
        object previous();
    }

    public interface IIterator<T>
    {
        bool hasNext();

        T next();
    }

    public interface IListIterator<T> : IIterator<T>
    {
        T previous();
    }

    internal class ListIterator : IListIterator, IIterator
    {
        IList _list;
        int _current;

        internal ListIterator(IList list)
        {
            _list = list;
            _current = -1;
        }

        bool IIterator.hasNext()
        {
            return (_current + 1 < _list.Count);
        }

        object IIterator.next()
        {
            return _list[++_current];
        }

        object IListIterator.previous()
        {
            return _list[--_current];
        }
    }

    internal class ListIterator<T> : IListIterator<T>, IIterator<T>, IListIterator, IIterator
    {
        IList<T> _list;
        int _current;

        internal ListIterator(IList<T> list)
        {
            _list = list;
            _current = -1;
        }

        bool IIterator<T>.hasNext()
        {
            return (_current + 1 < _list.Count);
        }

        T IIterator<T>.next()
        {
            return _list[++_current];
        }

        T IListIterator<T>.previous()
        {
            return _list[--_current];
        }

        bool IIterator.hasNext()
        {
            return (_current + 1 < _list.Count);
        }

        object IIterator.next()
        {
            return _list[++_current];
        }

        object IListIterator.previous()
        {
            return _list[--_current];
        }
    }

    internal class CollectionIterator<T> : IIterator<T>, IIterator
    {
        ICollection<T> _list;
        IEnumerator<T> _enumerator;
        bool? _hasNext;

        internal CollectionIterator(ICollection<T> list)
        {
            _list = list;
            _enumerator = _list.GetEnumerator();
        }

        bool IIterator<T>.hasNext()
        {
            if (_hasNext == null)
            {
                _hasNext = _enumerator.MoveNext();
            }
            return _hasNext.Value;
        }

        T IIterator<T>.next()
        {
            if (_hasNext != null && _hasNext.Value)
            {
                T value = _enumerator.Current;
                _hasNext = _enumerator.MoveNext();
                return value;
            }
            else
            {
                return default(T);
            }
        }

        bool IIterator.hasNext()
        {
            return ((IIterator<T>)this).hasNext();
        }

        object IIterator.next()
        {
            return ((IIterator<T>)this).next();
        }
    }
}
