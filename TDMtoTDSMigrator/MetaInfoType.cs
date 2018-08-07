using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TDMtoTDSMigrator
{
    public class MetaInfoType
    {

        public string CategoryId;
        public string CategoryName;

        public MetaInfoType(XmlNode metaInfoType)
        {
            CategoryId = metaInfoType.Attributes?[0].Value;
            CategoryName = metaInfoType.Attributes?[1].Value;
        }
    }
}
