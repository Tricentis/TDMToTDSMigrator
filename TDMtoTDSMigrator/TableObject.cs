using System;
using System.Collections.Generic;
using System.Xml;

namespace TDMtoTDSMigrator
{
    public class TableObject
    {
        public string surrogate;
        public string typeId;
        public List<string[]> attributes;

        public TableObject()
        {
            this.attributes = new List<string[]>();
           
        }
        public TableObject(List<string[]> attributes)
        {
            this.attributes = attributes;
        }
        public TableObject(TableObject obj)
        {
            this.attributes = obj.GetAttributes();
            this.typeId = obj.GetTypeId();
            this.surrogate = obj.GetSurrogate();
        }
        public string GetTypeId(){
            return this.typeId;
        }
        public string GetType(string typeId , XmlNode MetaInfoType)
        {
            for (int i =0; i < MetaInfoType.ChildNodes.Count; i++)
            {
                if (MetaInfoType.ChildNodes[i].Attributes[0].Value == typeId)
                {
                    return MetaInfoType.ChildNodes[i].Attributes[1].Value;
                }
            }
            return "Type not found";
        }
        public void SetTypeId(string typeId)
        {
            this.typeId = typeId;
        }
        public void AddAttribute(string attNumber, string attValue)
        {

            this.attributes.Add(new string[] { attNumber, attValue } );
        }
        public List<string[]> GetAttributes() {
            return attributes;
        }
        public void SetSurrogate(string surrogate) => this.surrogate = surrogate;
        public string GetSurrogate()
        {
            return this.surrogate;
        }

    }
}
