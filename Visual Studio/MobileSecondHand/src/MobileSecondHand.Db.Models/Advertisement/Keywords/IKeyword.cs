using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Db.Models.Advertisement.Keywords
{
    public interface IKeyword
    {
		int Id { get; set; }
		string Name { get; set; }
	}
}
