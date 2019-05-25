using GunzCord.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunzCord.Database
{
	public interface IGunzRepository
	{
		Task<IEnumerable<ServerStatus>> GetServerStatusAsync();
	}
}
