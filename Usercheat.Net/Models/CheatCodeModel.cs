using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usercheat.Net.Models
{
    public class CheatCodeModel : CheatItemModel
    {
        public ObservableCollection<int> Values { get; set; }
    }
}
