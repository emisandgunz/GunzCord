using System.Collections.Generic;

namespace GunzSharp.Commands.Parameters
{
	public class MCommandParameterDesc
	{
		public const int MAX_BLOB_SIZE = 0x100000;

		protected List<MCommandParamCondition> Conditions { get; set; }

		public string Description { get; protected set; }

		public MCommandParameterType Type { get; protected set; }

		public MCommandParameterDesc(MCommandParameterType commandParameterType, string description)
		{
			Type = commandParameterType;
			Description = description;

			Conditions = new List<MCommandParamCondition>();
		}

		public void AddCondition(MCommandParamCondition condition)
		{
			Conditions.Add(condition);
		}

		public MCommandParamCondition GetCondition(int n)
		{
			return Conditions[n];
		}

		public int GetConditionCount()
		{
			return Conditions != null ? Conditions.Count : 0;
		}

		public bool HasConditions()
		{
			return Conditions != null && Conditions.Count > 0;
		}
	}
}
