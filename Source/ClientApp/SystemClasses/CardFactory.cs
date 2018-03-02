using ClientApp.SupportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientApp.Elements;

namespace ClientApp.SystemClasses
{
    public static class CardFactory
    {
        public static STabCard CreateTab(Guid CardID)
        {
            var result = new STabCard();

            return result;
        }
    }
}
