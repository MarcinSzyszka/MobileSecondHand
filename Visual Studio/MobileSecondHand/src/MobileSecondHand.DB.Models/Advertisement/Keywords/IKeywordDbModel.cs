﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.DB.Models.Advertisement.Keywords
{
    public interface IKeywordDbModel
    {
		int Id { get; set; }
		string Name { get; set; }
	}
}