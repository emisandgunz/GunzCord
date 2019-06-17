using System.Collections.Generic;

namespace GunzSharp.Commands
{
	public class MCommandSNChecker
	{
		public const int DEFAULT_COMMAND_SNCHECKER_CAPICITY = 50;

		private int capacity;
		private Queue<int> snQueue;
		private SortedSet<int> snSet;

		public MCommandSNChecker()
		{
			capacity = DEFAULT_COMMAND_SNCHECKER_CAPICITY;

			snQueue = new Queue<int>(capacity);
			snSet = new SortedSet<int>();
		}

		public void InitCapacity(int capacity)
		{
			this.capacity = capacity;

			snQueue = new Queue<int>(capacity);
			snSet = new SortedSet<int>();
		}

		public bool CheckValidate(int serialNumber)
		{
			if (snSet.Contains(serialNumber))
			{
				return false;
			}

			if (snQueue.Count >= capacity)
			{
				int first = snQueue.Dequeue();
				snSet.Remove(first);
			}

			snSet.Add(serialNumber);
			snQueue.Enqueue(serialNumber);

			return true;
		}
	}
}
