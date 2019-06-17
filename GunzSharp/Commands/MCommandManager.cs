using GunzSharp.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunzSharp.Commands
{
	public class MCommandManager
	{
		protected SortedDictionary<string, string> CommandAlias { get; set; }

		protected SortedDictionary<int, MCommandDesc> CommandDescs { get; set; }
		protected LinkedList<MCommand> CommandQueue { get; set; }

		public MCommandManager()
		{
			InitializeCommandDesc();

			CommandAlias = new SortedDictionary<string, string>();
			CommandQueue = new LinkedList<MCommand>();
		}

		protected void InitializeCommandDesc()
		{
			CommandDescs = new SortedDictionary<int, MCommandDesc>();
		}
		public void AddAlias(string name, string text)
		{
			CommandAlias.Add(name, text);
		}

		public void AddCommandDesc(MCommandDesc cd)
		{
			CommandDescs.Add(cd.ID, cd);
		}

		public void AssignDescs(MCommandManager otherCM)
		{
			foreach (var commandDesc in CommandDescs)
			{
				otherCM.AddCommandDesc(commandDesc.Value.Clone());
			}
		}

		public MCommand GetCommand()
		{
			if (CommandQueue.Count == 0)
			{
				return null;
			}

			MCommand cmd = CommandQueue.First.Value;
			CommandQueue.RemoveFirst();

			return cmd;
		}

		public MCommandDesc GetCommandDesc(int i)
		{
			if (i < 0 || i >= CommandDescs.Count)
			{
				return null;
			}

			return CommandDescs.ElementAt(i).Value;
		}

		public MCommandDesc GetCommandDescByID(int id)
		{
			if (!CommandDescs.ContainsKey(id))
			{
				return null;
			}

			return CommandDescs[id];
		}

		public int GetCommandDescCount()
		{
			return CommandDescs.Count;
		}

		public int GetCommandQueueCount()
		{
			return CommandQueue.Count;
		}

		public void Initialize()
		{
			CommandQueue = new LinkedList<MCommand>();
		}
		public MCommand PeekCommand()
		{
			if (CommandQueue.Count == 0)
			{
				return null;
			}

			return CommandQueue.First.Value;
		}
		
		public bool Post(MCommand cmd)
		{
			bool checkRule = cmd.CheckRule();

			if (!checkRule)
			{
				return false;
			}

			CommandQueue.AddLast(cmd);

			return true;
		}
	}
}
