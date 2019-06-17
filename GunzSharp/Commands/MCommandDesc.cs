using GunzSharp.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunzSharp.Commands
{
	[Flags]
	public enum MCommandDescFlags
	{
		MCDT_NOTINITIALIZED = 0,
		MCDT_MACHINE2MACHINE = 1,
		MCDT_LOCAL = 2,
		MCDT_TICKSYNC = 4,
		MCDT_TICKASYNC = 8,
		MCDT_USER = 16,
		MCDT_ADMIN = 32,
		MCDT_PEER2PEER = 64,
		MCCT_NON_ENCRYPTED = 128,
		MCCT_HSHIELD_ENCRYPTED = 256
	}

	public class MCommandDesc
	{
		protected MCommandDescFlags Flag { get; set; }

		protected List<MCommandParameterDesc> ParamDescs { get; set; }

		public string Description { get; protected set; }

		public int ID { get; protected set; }

		public string Name { get; protected set; }

		public MCommandDesc(int id, string name, string description, MCommandDescFlags flag)
		{
			ID = id;
			Name = name;
			Description = description;
			Flag = flag;

			ParamDescs = new List<MCommandParameterDesc>();
		}

		public void AddParamDesc(MCommandParameterDesc parameterDesc)
		{
			ParamDescs.Add(parameterDesc);
		}

		public bool IsFlag(MCommandDescFlags flag)
		{
			return (Flag & flag) == flag;
		}

		public MCommandDesc Clone()
		{
			MCommandDesc newDesc = new MCommandDesc(ID, Name, Description, Flag);

			foreach (var paramDesc in ParamDescs)
			{
				newDesc.AddParamDesc(new MCommandParameterDesc(paramDesc.Type, paramDesc.Description));
			}

			return newDesc;
		}

		public MCommandParameterDesc GetParameterDesc(int i)
		{
			if (ParamDescs == null || i < 0 || i >= ParamDescs.Count)
			{
				return null;
			}

			return ParamDescs[i];
		}

		public int GetParameterDescCount()
		{
			return ParamDescs != null ? ParamDescs.Count : 0;
		}

		public MCommandParameterType GetParameterType(int i)
		{
			if (ParamDescs == null || i < 0 || i >= ParamDescs.Count)
			{
				return MCommandParameterType.MPT_END;
			}

			return ParamDescs[i].Type;
		}
		
	}
}
