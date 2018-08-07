using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TDMtoTDSMigrator
{
    public class StringAttribute
    {

        private readonly TdmDataDocument tdmDataSheet;
        public string ObjectId;
        public string AttributeId;

        public string AttributeName
        {
            get
            {
                if (AttributeId != null)
                {
                    return tdmDataSheet.FindAttributeName(AttributeId);
                }
                return "Not Found";
            }
        }

        public string CategoryName
        {
            get
            {
                if (AttributeId != null)
                {
                    return tdmDataSheet.FindCategoryName(tdmDataSheet.FindCategoryId(AttributeId));
                }
                return "Not Found";
            }
        }

        public string AttributeValue;

        public StringAttribute(TdmDataDocument tdmDataSheet, XmlNode stringAttribute)
        {
            this.tdmDataSheet = tdmDataSheet;
            ObjectId = stringAttribute.Attributes?[0].Value;
            AttributeId = stringAttribute.Attributes?[1].Value;
            AttributeValue = stringAttribute.Attributes?[2].Value;
        }
    }
}
