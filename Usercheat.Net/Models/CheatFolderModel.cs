﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usercheat.Net.Models
{
    public class CheatFolderModel : CheatItemModel
    {
        public ObservableCollection<CheatCodeModel> Items { get; set; }
    }
}
