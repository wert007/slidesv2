using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public class TimeWatcher
	{
		Stopwatch _watch;
		Dictionary<string, long> _entries;
		long _lastTime;
		List<TimeWatcher> _child;
		bool _useChild;

		public TimeWatcher()
		{
			_watch = new Stopwatch();
			_entries = new Dictionary<string, long>();
			_lastTime = 0;
			_child = new List<TimeWatcher>();
			_useChild = false;
		}

		public void Start()
		{
			_watch.Start();
			if (_useChild)
				_child.Last().Start();
		}

		public void Stop()
		{
			_watch.Stop();
			if (_useChild)
				_child.Last().Stop();
		}

		public long GetTotal()
		{
			Stop();
			return _watch.ElapsedMilliseconds;
		}

		public void Record(string entry)
		{
			if (_useChild)
			{
				_child.Last().Record(entry);
				return;
			}
			var time = _watch.ElapsedMilliseconds - _lastTime;
			_lastTime = _watch.ElapsedMilliseconds;
			if (!_entries.ContainsKey(entry))
				_entries.Add(entry, time);
			else
			{
				var i = 0;
				i++;
				while (_entries.ContainsKey(entry + i))
					i++;
				_entries.Add(entry + i, time);
			}
		}

		public void Push()
		{
			_child.Add(new TimeWatcher());
			_child.Last().Start();
			_useChild = true;
			_entries.Add(_child.Count.ToString(), -1);
		}

		public void Pop()
		{
			_useChild = false;
		}

		public IEnumerable<KeyValuePair<string, long>> GetEntries(int depth = -1)
		{
			int index = 0;
			foreach (var entry in _entries)
			{
				if (entry.Value == -1 && depth != 0)
				{
					foreach (var childEntry in _child[index].GetEntries(depth - 1))
						yield return childEntry;
					index++;
				}
				else
					yield return entry;
			}
		}
	}
}
